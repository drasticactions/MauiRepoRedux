using System.Diagnostics;
using Masonry;
using ObjCRuntime;
using VLCKit;

namespace DrasticVid;

[Register ("AppDelegate")]
public class AppDelegate : NSApplicationDelegate {
	public override void DidFinishLaunching (NSNotification notification)
	{
		var window = new VideoWindow (new CGRect (0, 0, 800, 600), NSWindowStyle.Titled | NSWindowStyle.FullSizeContentView |  NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Miniaturizable, NSBackingStore.Buffered, false);
		window.MakeKeyAndOrderFront(this);
		window.Center();
		window.PlayUrl("https://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4");
	}

	public override void WillTerminate (NSNotification notification)
	{
		// Insert code here to tear down your application
	}
}

public sealed class TrackingView : NSView
{
	private NSTimer? inactivityTimer;
	private const double InactivityTimeout = 10.0;
	private Action hideAction;
	private Action showAction;
	private bool controlsVisible = false;
	
	public TrackingView(Action hideAction, Action showAction, CGRect rect)
		: base(rect)
	{
		this.hideAction = hideAction;
		this.showAction = showAction;
		this.AddTrackingArea(new NSTrackingArea(rect, NSTrackingAreaOptions.MouseMoved | 
		                                              NSTrackingAreaOptions.ActiveInKeyWindow |
		                                              NSTrackingAreaOptions.MouseEnteredAndExited, this, null));
	}
	
	public override void MouseMoved(NSEvent theEvent)
	{
		
		if (!this.controlsVisible)
		{
			this.showAction();
			this.controlsVisible = true;
		}
		
		this.ResetTimer();
	}
	
	private void ResetTimer()
	{
		this.inactivityTimer?.Invalidate();
		this.inactivityTimer = NSTimer.CreateScheduledTimer(InactivityTimeout, false, (timer) =>
		{
			this.hideAction();
			this.controlsVisible = false;
		});
	}
}

public sealed class VideoWindow : NSWindow
{
	private NSView _view;
	private ControlView _controlsView;
	private VLCVideoView _videoView;
	private VLCMediaPlayer _mediaPlayer;
	private TrackingView _trackingView;
	
	public VideoWindow (CGRect contentRect, NSWindowStyle aStyle, NSBackingStore bufferingType, bool deferCreation) : base (contentRect, aStyle, bufferingType, deferCreation)
	{
		this.TitlebarAppearsTransparent = true;
		this.StyleMask |= NSWindowStyle.FullSizeContentView;
		this._view = new NSView (contentRect);
		this._view.WantsLayer = true;
		this._view.Layer!.BackgroundColor = NSColor.Black.CGColor;
		this._view.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
		this.ContentView = _view;
		
		this._videoView = new VLCVideoView() { BackColor = NSColor.Clear, FillScreen = true };
		this._mediaPlayer = new VLCMediaPlayer(this._videoView);
		this._videoView.Frame = contentRect;
		this._videoView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
		this._controlsView = new ControlView(this._mediaPlayer);
		
		this._view.AddSubview(this._videoView);
		
		this._view.AddSubview(this._controlsView);

		this._controlsView.MakeConstraints(make =>
		{
			make.Bottom.EqualTo(this._view.Bottom()).Offset(-10);
			make.Left.EqualTo(this._view.Left()).Offset(10);
			make.Right.EqualTo(this._view.Right()).Offset(-10);
			make.Height.EqualTo(NSObject.FromObject(100f));
		});
		
		this._trackingView = new TrackingView(this._controlsView.HideControls, this._controlsView.ShowControls, contentRect);
		this._view.AddSubview(this._trackingView);
		
		
		this.AcceptsMouseMovedEvents = true;
	}

	private sealed class ControlView : NSView, IVLCMediaPlayerDelegate
	{
		private VLCMediaPlayer? _mediaPlayer;
		private NSButton _playButton;
		private PlaybackSlider _playbackSlider;
		private NSTextField timeElapsedLabel;
		private NSTextField totalTimeLabel;
		private VolumeSlider _volumeSlider;
		private NSButton _volumeButton;
		private const double FadeDuration = 0.3;
		
		public void HideControls()
		{
			NSAnimationContext.RunAnimation(context =>
			{
				this.InvokeOnMainThread(() =>
				{
					context.Duration = FadeDuration;
					((NSView)this.Animator).AlphaValue = 0;
				});
			});
		}
		
		public void ShowControls()
		{
			NSAnimationContext.RunAnimation(context =>
			{
				this.InvokeOnMainThread(() =>
				{
					context.Duration = FadeDuration;
					((NSView)this.Animator).AlphaValue = 1;
				});
			});
		}
		
		public ControlView (NativeHandle handle) : base(handle)
		{
		}

		public ControlView(VLCMediaPlayer mediaPlayer)
		{
			this.WantsLayer = true;
			this.Layer!.CornerRadius = 8;
			this.AlphaValue = 0;
			
			this._mediaPlayer = mediaPlayer;
			this._mediaPlayer.WeakDelegate = this;
			this._playButton = this.CreateTransportButton("play", "Play/Pause", NSControlSize.Large);
			this._volumeButton = this.CreateTransportButton("speaker.2", "Mute/Unmute", NSControlSize.Regular);
			
			this._playbackSlider = new PlaybackSlider(this._mediaPlayer)
			{
				MinValue = 0,
				MaxValue = 100,
				FloatValue = 100,
				AutoresizingMask = NSViewResizingMask.MinYMargin | NSViewResizingMask.MaxXMargin,
				ControlSize = NSControlSize.Regular
			};
			
			this._volumeSlider = new VolumeSlider(this._mediaPlayer)
			{
				MinValue = 0,
				MaxValue = 100,
				FloatValue = 100,
				AutoresizingMask = NSViewResizingMask.MinYMargin | NSViewResizingMask.MaxXMargin,
				ControlSize = NSControlSize.Regular
			};
			
			// Time Labels
			this.timeElapsedLabel = new NSTextField
			{
				TranslatesAutoresizingMaskIntoConstraints = false,
				StringValue = "00:00",
				Editable = false,
				Bordered = false,
				BackgroundColor = NSColor.Clear,
				TextColor = NSColor.White,
				Alignment = NSTextAlignment.Center
			};

			this.totalTimeLabel = new NSTextField
			{
				TranslatesAutoresizingMaskIntoConstraints = false,
				StringValue = "00:00",
				Editable = false,
				Bordered = false,
				BackgroundColor = NSColor.Clear,
				TextColor = NSColor.White,
				Alignment = NSTextAlignment.Center
			};
			
			this._playButton.Activated += this.PlayButton_Clicked;
			
			this.AddSubview(this._playButton);
			this.AddSubview(this._playbackSlider);
			this.AddSubview(this.timeElapsedLabel);
			this.AddSubview(this.totalTimeLabel);
			this.AddSubview(this._volumeSlider);
			this.AddSubview(this._volumeButton);
			
			this.timeElapsedLabel.MakeConstraints(n =>
			{
				n.Left.EqualTo(this.Left()).Offset(10);
				n.Top.EqualTo(this._playButton.Bottom()).Offset(10);
				n.Width.EqualTo(NSObject.FromObject(60));
				n.Height.EqualTo(NSObject.FromObject(20));
			});
			
			this.totalTimeLabel.MakeConstraints(n =>
			{
				n.Right.EqualTo(this.Right()).Offset(-10);
				n.Top.EqualTo(this._playButton.Bottom()).Offset(10);
				n.Width.EqualTo(NSObject.FromObject(60));
				n.Height.EqualTo(NSObject.FromObject(20));
			});
			
			this._playbackSlider.MakeConstraints(n =>
			{
				n.Left.EqualTo(this.timeElapsedLabel.Right()).Offset(10);
				n.Right.EqualTo(this.totalTimeLabel.Left()).Offset(-10);
				n.Top.EqualTo(this._playButton.Bottom()).Offset(10);
				n.CenterY.EqualTo(this.timeElapsedLabel.CenterY());
			});
			
			this._playButton.MakeConstraints(n =>
			{
				n.CenterX.EqualTo(this.CenterX());
				n.Top.EqualTo(this.Top()).Offset(10);
				n.Width.EqualTo(NSObject.FromObject(40));
				n.Height.EqualTo(NSObject.FromObject(40));
			});
			
			this._volumeButton.MakeConstraints(n =>
			{
				n.Left.EqualTo(this.Left()).Offset(10);
				n.CenterY.EqualTo(this._playButton.CenterY());
				n.Width.EqualTo(NSObject.FromObject(40));
			});
			
			this._volumeSlider.MakeConstraints(n =>
			{
				n.Left.EqualTo(this._volumeButton.Right()).Offset(10);
				n.CenterY.EqualTo(this._playButton.CenterY());
				n.Width.EqualTo(NSObject.FromObject(100));
			});
			
			this.AddTransparentBackground();
		}
		
		private NSButton CreateTransportButton(string systemImageName, string toolTip, NSControlSize size = NSControlSize.Regular)
		{
			var button = new NSButton(CGRect.Empty)
			{
				BezelStyle = NSBezelStyle.Rounded,
				ToolTip = toolTip,
				ControlSize = size
			};

			button.Image = NSImage.GetSystemSymbol(systemImageName, null);

			button.ImagePosition = NSCellImagePosition.ImageOnly;
			return button;
		}
		
		private void AddTransparentBackground()
		{
			var visualEffectView = new NSVisualEffectView(this!.Bounds)
			{
				Material = NSVisualEffectMaterial.UnderWindowBackground,
				BlendingMode = NSVisualEffectBlendingMode.WithinWindow,
				State = NSVisualEffectState.Active,
				AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
				WantsLayer = true,
			};

			this.AddSubview(visualEffectView, NSWindowOrderingMode.Below, null);
		}
		
		public void MediaPlayerStateChanged(VLCMediaPlayerState newState)
		{
			this.InvokeOnMainThread(() =>
			{
				switch (newState)
				{
					case VLCMediaPlayerState.Paused:
					case VLCMediaPlayerState.Error:
					case VLCMediaPlayerState.Stopped:
					case VLCMediaPlayerState.Stopping:
						this._playButton.Enabled = true;
						this._playButton.Image = NSImage.GetSystemSymbol("play", null);
						break;
					case VLCMediaPlayerState.Opening:
						this._playButton.Enabled = false;
						break;
					case VLCMediaPlayerState.Buffering:
						break;
					case VLCMediaPlayerState.Playing:
						this._playButton.Enabled = true;
						this._playButton.Image = NSImage.GetSystemSymbol("pause", null);
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
				}
			});
		}

		public void MediaPlayerLengthChanged(long length)
		{
			this.InvokeOnMainThread(() =>
			{
				this.totalTimeLabel.StringValue = TimeSpan.FromMilliseconds(length).ToString(@"mm\:ss");
				this._playbackSlider.MaxValue = length;
			});
		}

		public void MediaPlayerTimeChanged(NSNotification aNotification)
		{
			this.InvokeOnMainThread(() =>
			{
				this.timeElapsedLabel.StringValue = this._mediaPlayer!.Time.StringValue;
				this._playbackSlider.FloatValue = (float)this._mediaPlayer!.Time.Value;
				//this._playbackSlider.FloatValue = (float)time / this._mediaPlayer.Length;
			});
		}

		void PlayButton_Clicked(object? sender, EventArgs e)
		{
			switch (this._mediaPlayer.State)
			{
				case VLCMediaPlayerState.Buffering:
				case VLCMediaPlayerState.Playing:
					this._mediaPlayer.Pause();
					break;
				case VLCMediaPlayerState.Paused:
					this._mediaPlayer.Play();
					break;
				case VLCMediaPlayerState.Stopped:
					break;
				case VLCMediaPlayerState.Stopping:
					break;
				case VLCMediaPlayerState.Opening:
					break;
				case VLCMediaPlayerState.Error:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	private sealed class PlaybackSlider : NSSlider
	{
		private VLCMediaPlayer mediaPlayer;
		
		public PlaybackSlider(VLCMediaPlayer mediaPlayer)
		{
			this.mediaPlayer = mediaPlayer;
			this.MinValue = 0;
			this.MaxValue = 100;
			this.FloatValue = 0;
			this.AutoresizingMask = NSViewResizingMask.MinYMargin | NSViewResizingMask.MaxXMargin;
			this.ControlSize = NSControlSize.Regular;
			this.Action = new ObjCRuntime.Selector("SliderValueChanged:");
			this.Target = this;
		}
		
		[Export("SliderValueChanged:")]
		void ValueChanged(NSObject sender)
		{
			var sliderControl = sender as NSSlider;
			var currentEvent = NSApplication.SharedApplication.CurrentEvent;

			if (sliderControl == null || currentEvent == null)
				return;

			switch (currentEvent.Type)
			{
				case NSEventType.LeftMouseDown:
				case NSEventType.RightMouseDown:
					Debug.WriteLine("slider value started changing");
					break;

				case NSEventType.LeftMouseUp:
				case NSEventType.RightMouseUp:
					Debug.WriteLine($"slider value stopped changing: {sliderControl.DoubleValue}");
					this.mediaPlayer.Time = new VLCTime(sliderControl.FloatValue);
					break;

				case NSEventType.LeftMouseDragged:
				case NSEventType.RightMouseDragged:
					Debug.WriteLine($"slider value changed: {sliderControl.DoubleValue}");
					break;
			}
		}
	}

	private sealed class VolumeSlider : NSSlider
	{
		private VLCMediaPlayer mediaPlayer;
		
		public VolumeSlider(VLCMediaPlayer mediaPlayer)
		{
			this.mediaPlayer = mediaPlayer;
			this.MinValue = 0;
			this.MaxValue = 100;
			this.FloatValue = 100;
			this.AutoresizingMask = NSViewResizingMask.MinYMargin | NSViewResizingMask.MaxXMargin;
			this.ControlSize = NSControlSize.Regular;
			this.Action = new ObjCRuntime.Selector(nameof(ValueChanged));
			this.Target = this;
		}
		
		[Export(nameof(ValueChanged))]
		void ValueChanged()
		{
			if (this.mediaPlayer.Audio is not null)
			{
				this.mediaPlayer.Audio.Volume = (int)this.FloatValue;
			}
		}
	}
	
	public void PlayLocalFile(string filePath)
	{
		this._mediaPlayer.Media = new VLCMedia(filePath);
		this._mediaPlayer.Play();
	}
	
	public void PlayUrl(Uri url)
		=> PlayUrl(new NSUrl(url.ToString()));

	public void PlayUrl(string url)
		=> PlayUrl(new NSUrl(url));

	private void PlayUrl(NSUrl url)
	{
		this._mediaPlayer.Media = new VLCMedia(url);
		this._mediaPlayer.Play();
	}

	private void AddTransparentBackground()
	{
		var visualEffectView = new NSVisualEffectView(this.ContentView!.Bounds)
		{
			Material = NSVisualEffectMaterial.UnderWindowBackground,
			BlendingMode = NSVisualEffectBlendingMode.BehindWindow,
			State = NSVisualEffectState.Active,
			AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
			WantsLayer = true,
		};

		this.ContentView.AddSubview(visualEffectView, NSWindowOrderingMode.Below, null);
	}
}
