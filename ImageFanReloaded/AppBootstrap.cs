using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Controls.Factories;
using ImageFanReloaded.Core;
using ImageFanReloaded.Core.AboutInfo;
using ImageFanReloaded.Core.AboutInfo.Implementation;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.Controls.Factories.Implementation;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.ImageHandling.Factories.Implementation;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.OperatingSystem;
using ImageFanReloaded.Core.OperatingSystem.Implementation;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Settings.Implementation;
using ImageFanReloaded.Core.Synchronization;
using ImageFanReloaded.Core.Synchronization.Implementation;
using ImageFanReloaded.ImageHandling;
using ImageFanReloaded.ImageHandling.Factories;
using ImageFanReloaded.Mouse;
using ImageFanReloaded.Settings;

namespace ImageFanReloaded;

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
	private IEnvironmentSettings _environmentSettings = null!;
	private IMouseCursorFactory _mouseCursorFactory = null!;
	private ITabOptionsFactory _tabOptionsFactory = null!;
	private IImageViewFactory _imageViewFactory = null!;
	private IInputPathHandlerFactory _inputPathHandlerFactory = null!;
	private IInputPathHandler _commandLineArgsInputPathHandler = null!;

	private void BootstrapTypes()
	{
		IOperatingSystemSettings operatingSystemSettings = new OperatingSystemSettings();

		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

		_globalParameters = new GlobalParameters(operatingSystemSettings, imageResizer);

		_fileSizeEngine = new FileSizeEngine();
		IImageFileFactory imageFileFactory = new ImageFileFactory(
			_globalParameters, imageResizer, _fileSizeEngine);

		IDiscQueryEngineFactory discQueryEngineFactory = new DiscQueryEngineFactory(
			_globalParameters, imageFileFactory, _fileSizeEngine);
		_discQueryEngine = discQueryEngineFactory.GetDiscQueryEngine();

		_environmentSettings = new EnvironmentSettings();

		if (_environmentSettings.IsInsideFlatpakContainer)
		{
			_mouseCursorFactory = new FlatpakMouseCursorFactory();
			_tabOptionsFactory = new FlatpakTabOptionsFactory();
		}
		else
		{
			_mouseCursorFactory = new DefaultMouseCursorFactory();
			_tabOptionsFactory = new DefaultTabOptionsFactory();
		}

		IScreenInfo screenInfo = new ScreenInfo();
		_imageViewFactory = new ImageViewFactory(_globalParameters, _mouseCursorFactory, screenInfo);

		_inputPathHandlerFactory = new InputPathHandlerFactory(_globalParameters, _discQueryEngine);
		string? commandLineArgsInputPath = GetCommandLineArgsInputPath();
		_commandLineArgsInputPathHandler = _inputPathHandlerFactory.GetInputPathHandler(
			commandLineArgsInputPath);
	}

	private string? GetCommandLineArgsInputPath()
	{
		var args = _desktop.Args!;
		var inputPath = args.Length > 0 ? args[0] : null;

		return inputPath;
	}

	private bool IsMainViewAccess()
		=> _commandLineArgsInputPathHandler.InputPathType != InputPathType.File;

	private void ShowMainView()
	{
		IAsyncMutexFactory asyncMutexFactory = new AsyncMutexFactory();
		IMainViewFactory mainViewFactory = new MainViewFactory(
			_globalParameters, _mouseCursorFactory, _tabOptionsFactory, asyncMutexFactory);

		IMainView mainView = mainViewFactory.GetMainView();

		var mainWindow = (Window)mainView;
		_desktop.MainWindow = mainWindow;

		IEditableImageFactory editableImageFactory = new EditableImageFactory(_globalParameters);

		ISaveFileImageFormatFactory saveFileImageFormatFactory = new SaveFileImageFormatFactory();

		ISaveFileDialogFactory saveFileDialogFactory;

		if (_environmentSettings.IsInsideFlatpakContainer)
		{
			saveFileDialogFactory = new FlatpakSaveFileDialogFactory(mainWindow);
		}
		else
		{
			saveFileDialogFactory = new DefaultSaveFileDialogFactory(mainWindow);
		}

		IThumbnailInfoFactory thumbnailInfoFactory = new ThumbnailInfoFactory(_globalParameters);
		IFolderVisualStateFactory folderVisualStateFactory = new FolderVisualStateFactory(
			_globalParameters, _fileSizeEngine, thumbnailInfoFactory, _discQueryEngine);

		IImageEditViewFactory imageEditViewFactory = new ImageEditViewFactory(
			_globalParameters,
			_mouseCursorFactory,
			editableImageFactory,
			saveFileImageFormatFactory,
			saveFileDialogFactory);

		ITabOptionsViewFactory tabOptionsViewFactory = new TabOptionsViewFactory(_globalParameters);

		IAboutInfoProvider aboutInfoProvider = new AboutInfoProvider();
		IAboutViewFactory aboutViewFactory = new AboutViewFactory(
			aboutInfoProvider, _globalParameters);

		IImageInfoBuilder imageInfoBuilder = new ImageInfoBuilder(_globalParameters);
		IImageInfoViewFactory imageInfoViewFactory = new ImageInfoViewFactory(
			_globalParameters, imageInfoBuilder);

		var mainViewPresenter = new MainViewPresenter(
			_discQueryEngine,
			folderVisualStateFactory,
			_imageViewFactory,
			imageInfoViewFactory,
			imageEditViewFactory,
			tabOptionsViewFactory,
			aboutViewFactory,
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
		ITabOptions tabOptions = _tabOptionsFactory.GetTabOptions();
		IImageView imageView = _imageViewFactory.GetImageView(tabOptions);

		imageView.IsStandaloneView = true;
		imageView.ViewClosing += OnImageViewClosing;

		var imageViewPresenter = new ImageViewPresenter(
			_discQueryEngine,
			_commandLineArgsInputPathHandler,
			_globalParameters,
			imageView);

		await imageViewPresenter.SetUpAccessToImages();

		var imageWindow = (Window)imageView;
		_desktop.MainWindow = imageWindow;

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
