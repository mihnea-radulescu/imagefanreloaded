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
		ITabOptionsFactory tabOptionsFactory,
		IAsyncMutexFactory asyncMutexFactory)
	{
		_globalParameters = globalParameters;
		_mouseCursorFactory = mouseCursorFactory;
		_tabOptionsFactory = tabOptionsFactory;
		_asyncMutexFactory = asyncMutexFactory;
	}

	public IMainView GetMainView()
	{
		IMainView mainView = new MainWindow();

		mainView.GlobalParameters = _globalParameters;
		mainView.MouseCursorFactory = _mouseCursorFactory;
		mainView.TabOptionsFactory = _tabOptionsFactory;
		mainView.AsyncMutexFactory = _asyncMutexFactory;

		return mainView;
	}

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IMouseCursorFactory _mouseCursorFactory;
	private readonly ITabOptionsFactory _tabOptionsFactory;
	private readonly IAsyncMutexFactory _asyncMutexFactory;

	#endregion
}
