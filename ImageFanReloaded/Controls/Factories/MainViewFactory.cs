using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Synchronization;

namespace ImageFanReloaded.Controls.Factories;

public class MainViewFactory : IMainViewFactory
{
	public MainViewFactory(
		IGlobalParameters globalParameters,
		IMouseCursorFactory mouseCursorFactory,
		ISettingsFactory settingsFactory,
		IAsyncMutexFactory asyncMutexFactory)
	{
		_globalParameters = globalParameters;
		_mouseCursorFactory = mouseCursorFactory;
		_settingsFactory = settingsFactory;
		_asyncMutexFactory = asyncMutexFactory;
	}

	public IMainView GetMainView()
	{
		IMainView mainView = new MainWindow();

		mainView.GlobalParameters = _globalParameters;
		mainView.MouseCursorFactory = _mouseCursorFactory;
		mainView.SettingsFactory = _settingsFactory;
		mainView.AsyncMutexFactory = _asyncMutexFactory;

		return mainView;
	}

	private readonly IGlobalParameters _globalParameters;
	private readonly IMouseCursorFactory _mouseCursorFactory;
	private readonly ISettingsFactory _settingsFactory;
	private readonly IAsyncMutexFactory _asyncMutexFactory;
}
