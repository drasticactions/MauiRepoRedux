using Microsoft.Extensions.Logging.Debug;

namespace ListsCollectionView;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new UIToolsWindow(new AppShell(), new DebugLoggerProvider().CreateLogger("UITools"));
	}
}
