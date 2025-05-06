using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketServer
{
    class Program
    {
        // Collection to store active client connections
        private static readonly ConcurrentDictionary<string, WebSocket> _clients = new ConcurrentDictionary<string, WebSocket>();
        
        static async Task Main(string[] args)
        {
            // Configure and start the HTTP listener
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:8080/");
            httpListener.Start();
            
            Console.WriteLine("WebSocket server started at http://localhost:8080/");
            Console.WriteLine("Press Ctrl+C to stop the server");
            
            // Set up cancellation token for graceful shutdown
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Server shutdown initiated...");
            };
            
            try
            {
                // Main server loop
                while (!cts.Token.IsCancellationRequested)
                {
                    var context = await httpListener.GetContextAsync();
                    
                    if (context.Request.IsWebSocketRequest)
                    {
                        // Handle incoming WebSocket connection
                        ProcessWebSocketRequest(context, cts.Token);
                    }
                    else
                    {
                        // Return 400 for non-WebSocket requests
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
            }
            finally
            {
                // Clean up resources
                httpListener.Stop();
                
                // Close all client connections
                foreach (var client in _clients.Values)
                {
                    if (client.State == WebSocketState.Open)
                    {
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, 
                            "Server shutting down", CancellationToken.None);
                    }
                }
                
                Console.WriteLine("Server shutdown complete");
            }
        }
        
        private static async void ProcessWebSocketRequest(HttpListenerContext context, CancellationToken ct)
        {
            // Accept the WebSocket connection
            WebSocketContext webSocketContext = null;
            try
            {
                webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                var webSocket = webSocketContext.WebSocket;
                
                // Generate a unique ID for this client
                string clientId = Guid.NewGuid().ToString();
                
                // Add client to the collection
                _clients.TryAdd(clientId, webSocket);
                Console.WriteLine($"Client connected: {clientId}");
                
                // Handle the WebSocket connection
                await HandleWebSocketConnection(clientId, webSocket, ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
        }
        
        private static async Task HandleWebSocketConnection(string clientId, WebSocket webSocket, CancellationToken ct)
        {
            var buffer = new byte[1024];
            
            try
            {
                while (webSocket.State == WebSocketState.Open && !ct.IsCancellationRequested)
                {
                    // Create a linked cancellation token to handle both server shutdown and receive timeout
                    using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    timeoutCts.CancelAfter(TimeSpan.FromMinutes(2)); // 2-minute receive timeout
                    
                    // Receive a message from the client
                    var receiveResult = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), timeoutCts.Token);
                    
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        // Client initiated a close
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Client requested close",
                            CancellationToken.None);
                        
                        Console.WriteLine($"Client disconnected: {clientId}");
                        break;
                    }
                    
                    // Process the received message
                    if (receiveResult.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(
                            buffer, 0, receiveResult.Count);
                        Console.WriteLine($"Received from {clientId}: {receivedMessage}");
                        
                        // Check for ping request
                        if (receivedMessage.Equals("ping", StringComparison.OrdinalIgnoreCase))
                        {
                            // Send pong response
                            string response = "pong";
                            Console.WriteLine($"Sending to {clientId}: {response}");
                            
                            var responseBuffer = Encoding.UTF8.GetBytes(response);
                            await webSocket.SendAsync(
                                new ArraySegment<byte>(responseBuffer),
                                WebSocketMessageType.Text,
                                true,
                                CancellationToken.None);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected exception when cancellation is requested
                Console.WriteLine($"Connection with {clientId} closed due to timeout or server shutdown");
            }
            catch (WebSocketException ex)
            {
                // WebSocket was closed abnormally
                Console.WriteLine($"WebSocket error for {clientId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Unexpected error
                Console.WriteLine($"Error handling connection for {clientId}: {ex.Message}");
            }
            finally
            {
                // Remove client from the collection
                _clients.TryRemove(clientId, out _);
                
                // Ensure the WebSocket is closed
                if (webSocket.State != WebSocketState.Closed)
                {
                    try
                    {
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.InternalServerError,
                            "Connection closed due to error",
                            CancellationToken.None);
                    }
                    catch
                    {
                        // Ignore any errors during close
                    }
                }
                
                Console.WriteLine($"Client removed: {clientId}");
            }
        }
    }
}