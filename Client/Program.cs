using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("WebSocket Client Starting...");
            
            // Set up cancellation token for graceful shutdown
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Client shutdown initiated...");
            };
            
            // Create and connect the WebSocket client
            using (var client = new ClientWebSocket())
            {
                Uri serverUri = new Uri("ws://localhost:8080/");
                
                try
                {
                    Console.WriteLine($"Connecting to {serverUri}...");
                    await client.ConnectAsync(serverUri, cts.Token);
                    Console.WriteLine("Connected to the server!");
                    
                    // Start a task to receive messages
                    var receiveTask = ReceiveMessagesAsync(client, cts.Token);
                    
                    // Main loop for sending messages
                    while (client.State == WebSocketState.Open && !cts.Token.IsCancellationRequested)
                    {
                        Console.WriteLine("\nOptions:");
                        Console.WriteLine("1. Send ping");
                        Console.WriteLine("2. Send custom message");
                        Console.WriteLine("3. Exit");
                        Console.Write("Choose an option (1-3): ");
                        
                        var key = Console.ReadKey();
                        Console.WriteLine();
                        
                        if (key.KeyChar == '1')
                        {
                            await SendMessageAsync(client, "ping", cts.Token);
                        }
                        else if (key.KeyChar == '2')
                        {
                            Console.Write("Enter your message: ");
                            string message = Console.ReadLine();
                            await SendMessageAsync(client, message, cts.Token);
                        }
                        else if (key.KeyChar == '3')
                        {
                            break;
                        }
                    }
                    
                    // Close the WebSocket connection gracefully
                    if (client.State == WebSocketState.Open)
                    {
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, 
                            "Client shutting down", CancellationToken.None);
                    }
                    
                    // Wait for receive task to complete
                    await receiveTask;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Operation cancelled");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine("Client disconnected");
                }
            }
            
            Console.WriteLine("Client shutdown complete");
        }
        
        private static async Task SendMessageAsync(ClientWebSocket client, string message, CancellationToken ct)
        {
            try
            {
                Console.WriteLine($"Sending: {message}");
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                await client.SendAsync(new ArraySegment<byte>(messageBytes), 
                    WebSocketMessageType.Text, true, ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send error: {ex.Message}");
            }
        }
        
        private static async Task ReceiveMessagesAsync(ClientWebSocket client, CancellationToken ct)
        {
            byte[] buffer = new byte[1024];
            
            try
            {
                while (client.State == WebSocketState.Open && !ct.IsCancellationRequested)
                {
                    var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
                    
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        // Server initiated a close
                        await client.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Server requested close",
                            CancellationToken.None);
                        
                        Console.WriteLine("Server initiated disconnect");
                        break;
                    }
                    
                    // Process the received message
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(
                            buffer, 0, result.Count);
                        Console.WriteLine($"Received: {receivedMessage}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected exception when cancellation is requested
                Console.WriteLine("Receive operation cancelled");
            }
            catch (WebSocketException ex)
            {
                // WebSocket was closed abnormally
                Console.WriteLine($"WebSocket receive error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Unexpected error
                Console.WriteLine($"Receive error: {ex.Message}");
            }
        }
    }
}