//#define FLATPAK_BUILD

using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Controls.Factories;
using ImageFanReloaded.Core;
using ImageFanReloaded.Core.AboutInformation;
using ImageFanReloaded.Core.AboutInformation.Implementation;
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
using ImageFanReloaded.Core.OperatingSystem;
using ImageFanReloaded.Core.OperatingSystem.Implementation;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Settings.Implementation;
using ImageFanReloaded.Core.Synchronization;
using ImageFanReloaded.Core.Synchronization.Implementation;
using ImageFanReloaded.ImageHandling;
using ImageFanReloaded.ImageHandling.Factories;
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

	private IGlobalParameters _globalParameters = default!;
	private IFileSizeEngine _fileSizeEngine = default!;
	private IDiscQueryEngine _discQueryEngine = default!;
	private ITabOptionsFactory _tabOptionsFactory = default!;
	private IInputPathHandlerFactory _inputPathHandlerFactory = default!;
	private IInputPathHandler _commandLineArgsInputPathHandler = default!;

	private void BootstrapTypes()
	{
		IOperatingSystemSettings operatingSystemSettings = new OperatingSystemSettings();

		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

		_globalParameters = new GlobalParameters(operatingSystemSettings, imageResizer);

		IImageFileFactory imageFileFactory = new ImageFileFactory(_globalParameters, imageResizer);

		_fileSizeEngine = new FileSizeEngine();

		IDiscQueryEngineFactory discQueryEngineFactory = new DiscQueryEngineFactory(
			_globalParameters, _fileSizeEngine, imageFileFactory);
		_discQueryEngine = discQueryEngineFactory.GetDiscQueryEngine();

#if FLATPAK_BUILD
		_tabOptionsFactory = new FlatpakTabOptionsFactory();
#else
		_tabOptionsFactory = new DefaultTabOptionsFactory();
#endif

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
		var mainWindow = new MainWindow();
		_desktop.MainWindow = mainWindow;
		IScreenInformation screenInformation = new ScreenInformation(mainWindow);

		IEditableImageFactory editableImageFactory = new EditableImageFactory(_globalParameters);

		ISaveFileImageFormatFactory saveFileImageFormatFactory = new SaveFileImageFormatFactory();

		ISaveFileDialogFactory saveFileDialogFactory;

#if FLATPAK_BUILD
		saveFileDialogFactory = new FlatpakSaveFileDialogFactory();
#else
		saveFileDialogFactory = new DefaultSaveFileDialogFactory(mainWindow);
#endif

		IAsyncMutexFactory asyncMutexFactory = new AsyncMutexFactory();

		IMainView mainView = mainWindow;
		mainView.GlobalParameters = _globalParameters;
		mainView.TabOptionsFactory = _tabOptionsFactory;
		mainView.AsyncMutexFactory = asyncMutexFactory;

		IThumbnailInfoFactory thumbnailInfoFactory = new ThumbnailInfoFactory(_globalParameters);
		IFolderVisualStateFactory folderVisualStateFactory = new FolderVisualStateFactory(
			_globalParameters, _fileSizeEngine, thumbnailInfoFactory, _discQueryEngine);

		IImageViewFactory imageViewFactory = new ImageViewFactory(
			_globalParameters, screenInformation);

		IImageEditViewFactory imageEditViewFactory = new ImageEditViewFactory(
			_globalParameters,
			editableImageFactory,
			saveFileImageFormatFactory,
			saveFileDialogFactory);

		ITabOptionsViewFactory tabOptionsViewFactory = new TabOptionsViewFactory(_globalParameters);

		IAboutInformationProvider aboutInformationProvider = new AboutInformationProvider();
		IAboutViewFactory aboutViewFactory = new AboutViewFactory(
			aboutInformationProvider, _globalParameters);

		IImageInfoBuilder imageInfoBuilder = new ImageInfoBuilder();
		IImageInfoViewFactory imageInfoViewFactory = new ImageInfoViewFactory(
			_globalParameters, imageInfoBuilder);

		var mainViewPresenter = new MainViewPresenter(
			_discQueryEngine,
			folderVisualStateFactory,
			imageViewFactory,
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
		var imageWindow = new ImageWindow();
		_desktop.MainWindow = imageWindow;
		IScreenInformation screenInformation = new ScreenInformation(imageWindow);

		IImageView imageView = imageWindow;
		imageView.GlobalParameters = _globalParameters;

		imageView.ScreenInformation = screenInformation;

		ITabOptions tabOptions = _tabOptionsFactory.GetTabOptions();
		imageView.TabOptions = tabOptions;

		imageView.IsStandaloneView = true;

		imageView.ViewClosing += OnImageViewClosing;

		var imageViewPresenter = new ImageViewPresenter(
			_discQueryEngine,
			_commandLineArgsInputPathHandler,
			_globalParameters,
			tabOptions,
			imageView);

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
