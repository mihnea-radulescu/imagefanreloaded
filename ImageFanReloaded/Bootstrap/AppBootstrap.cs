using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Core;
using ImageFanReloaded.Core.AboutInformation;
using ImageFanReloaded.Core.AboutInformation.Implementation;
using ImageFanReloaded.Core.Bootstrap;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Implementation;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.OperatingSystem;
using ImageFanReloaded.Core.OperatingSystem.Implementation;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Settings.Implementation;
using ImageFanReloaded.Core.Synchronization;
using ImageFanReloaded.Core.Synchronization.Implementation;
using ImageFanReloaded.ImageHandling;
using ImageFanReloaded.Settings;

namespace ImageFanReloaded.Bootstrap;

public class AppBootstrap : IAppBootstrap
{
	public AppBootstrap(IClassicDesktopStyleApplicationLifetime desktop)
	{
		_desktop = desktop;
	}
	
	public async Task BootstrapApplication()
	{
		BootstrapTypes();

		if (IsMainViewAccess())
		{
			ShowMainView();
		}
		else
		{
			await ShowImageView();
		}
	}

	#region Private
	
	private readonly IClassicDesktopStyleApplicationLifetime _desktop;
	
	private IGlobalParameters _globalParameters = null!;
	private IFileSizeEngine _fileSizeEngine = null!;
	private IDiscQueryEngine _discQueryEngine = null!;
	private ITabOptionsFactory _tabOptionsFactory = null!;
	private IInputPathHandlerFactory _inputPathHandlerFactory = null!;
	private IInputPathHandler _commandLineArgsInputPathHandler = null!;
	
	private void BootstrapTypes()
	{
		IOperatingSystemSettings operatingSystemSettings = new OperatingSystemSettings();
		
		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);
		
		_globalParameters = new GlobalParameters(operatingSystemSettings, imageResizer);
		
		_fileSizeEngine = new FileSizeEngine();
		
		IImageFileFactory imageFileFactory = new ImageFileFactory(_globalParameters, imageResizer);
		IDiscQueryEngineFactory discQueryEngineFactory = new DiscQueryEngineFactory(
			_globalParameters, _fileSizeEngine, imageFileFactory);
		_discQueryEngine = discQueryEngineFactory.GetDiscQueryEngine();

		_tabOptionsFactory = new TabOptionsFactory();

		_inputPathHandlerFactory = new InputPathHandlerFactory(_globalParameters, _discQueryEngine);
		string? commandLineArgsInputPath = GetCommandLineArgsInputPath();
		_commandLineArgsInputPathHandler = _inputPathHandlerFactory.GetInputPathHandler(commandLineArgsInputPath);
	}
	
	private string? GetCommandLineArgsInputPath()
	{
		var args = _desktop.Args!;
		var inputPath = args.Length > 0 ? args[0] : null;

		return inputPath;
	}
	
	private bool IsMainViewAccess() => _commandLineArgsInputPathHandler.InputPathType != InputPathType.File;
	
	private void ShowMainView()
	{
		IFolderChangedMutexFactory folderChangedMutexFactory = new FolderChangedMutexFactory();
		
		var mainWindow = new MainWindow();
		_desktop.MainWindow = mainWindow;
		IScreenInformation screenInformation = new ScreenInformation(mainWindow);

		IMainView mainView = mainWindow;
		mainView.GlobalParameters = _globalParameters;
		mainView.TabOptionsFactory = _tabOptionsFactory;
		mainView.FolderChangedMutexFactory = folderChangedMutexFactory;

		IThumbnailInfoFactory thumbnailInfoFactory = new ThumbnailInfoFactory(_globalParameters);
		IFolderVisualStateFactory folderVisualStateFactory = new FolderVisualStateFactory(
			_globalParameters, _fileSizeEngine, thumbnailInfoFactory, _discQueryEngine);

		IImageViewFactory imageViewFactory = new ImageViewFactory(_globalParameters, screenInformation);
		IAboutInformationProvider aboutInformationProvider = new AboutInformationProvider();

		IAboutViewFactory aboutViewFactory = new AboutViewFactory(aboutInformationProvider, _globalParameters);
		ITabOptionsViewFactory tabOptionsViewFactory = new TabOptionsViewFactory(_globalParameters);

		var mainViewPresenter = new MainViewPresenter(
			_discQueryEngine,
			folderVisualStateFactory,
			imageViewFactory,
			aboutViewFactory,
			tabOptionsViewFactory,
			_inputPathHandlerFactory,
			_commandLineArgsInputPathHandler,
			mainView);

		mainView.AddFakeTabItem();
		mainView.AddContentTabItem();
		mainView.RegisterTabControlEvents();

		mainView.Show();
	}
	
	private async Task ShowImageView()
	{
		var imageWindow = new ImageWindow();
		_desktop.MainWindow = imageWindow;
		IScreenInformation screenInformation = new ScreenInformation(imageWindow);

		IImageView imageView = imageWindow;
		imageView.GlobalParameters = _globalParameters;
		imageView.ScreenInformation = screenInformation;

		ITabOptions tabOptions = _tabOptionsFactory.GetTabOptions();
		imageView.TabOptions = tabOptions;

		imageView.ViewClosing += OnImageViewClosing;

		var imageViewPresenter = new ImageViewPresenter(
			_discQueryEngine, _commandLineArgsInputPathHandler, _globalParameters, imageView);
			
		await imageViewPresenter.SetUpAccessToImages();
		
		imageView.Show();
	}

	private void OnImageViewClosing(object? sender, ImageViewClosingEventArgs e)
	{
		var shouldShowMainView = e.ShowMainView;

		if (shouldShowMainView)
		{
			ShowMainView();
		}
	}
	
	#endregion
}
