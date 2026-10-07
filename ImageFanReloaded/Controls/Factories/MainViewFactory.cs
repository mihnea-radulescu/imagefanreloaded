using Avalonia.Controls;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Synchronization;
using ImageFanReloaded.RuntimeEnvironment;

namespace ImageFanReloaded.Controls.Factories;

public class MainViewFactory : IMainViewFactory
{
	public MainViewFactory(
		IGlobalParameters globalParameters,
		IMouseCursorFactory mouseCursorFactory,
		ISettingsFactory settingsFactory,
		IAsyncMutexFactory asyncMutexFactory,
		IImageIndexCalculator imageIndexCalculator)
	{
		_globalParameters = globalParameters;
		_mouseCursorFactory = mouseCursorFactory;
		_settingsFactory = settingsFactory;
		_asyncMutexFactory = asyncMutexFactory;
		_imageIndexCalculator = imageIndexCalculator;
	}

	public IMainView GetMainView()
	{
		IMainView mainView = new MainWindow();

		mainView.GlobalParameters = _globalParameters;
		mainView.MouseCursorFactory = _mouseCursorFactory;
		mainView.SettingsFactory = _settingsFactory;
		mainView.AsyncMutexFactory = _asyncMutexFactory;
		mainView.ImageIndexCalculator = _imageIndexCalculator;

		var mainWindow = (Window)mainView;
		mainView.AccentColorChangePublisher =
			new AccentColorChangePublisher(mainWindow);

		return mainView;
	}

	private readonly IGlobalParameters _globalParameters;
	private readonly IMouseCursorFactory _mouseCursorFactory;
	private readonly ISettingsFactory _settingsFactory;
	private readonly IAsyncMutexFactory _asyncMutexFactory;
	private readonly IImageIndexCalculator _imageIndexCalculator;
}
