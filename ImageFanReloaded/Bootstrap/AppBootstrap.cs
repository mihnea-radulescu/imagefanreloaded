using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Core;
using ImageFanReloaded.Core.AboutInformation;
using ImageFanReloaded.Core.AboutInformation.Implementation;
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
    private IInputPathContainer _inputPathContainer = null!;
    
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
		
	    string? inputPath = GetInputPath();
	    _inputPathContainer = new InputPathContainer(_globalParameters, _discQueryEngine, inputPath);
    }
    
    private string? GetInputPath()
    {
	    var args = _desktop.Args!;
	    var inputPath = args.Length > 0 ? args[0] : null;

	    return inputPath;
    }
    
    private bool IsMainViewAccess() => _inputPathContainer.InputPathType != InputPathType.File;
    
    private void ShowMainView()
    {
	    IFolderChangedMutexFactory folderChangedMutexFactory = new FolderChangedMutexFactory();
		
	    var mainWindow = new MainWindow();
	    _desktop.MainWindow = mainWindow;
	    IScreenInformation screenInformation = new ScreenInformation(mainWindow);
			
	    IMainView mainView = mainWindow;
	    mainView.GlobalParameters = _globalParameters;
	    mainView.FolderChangedMutexFactory = folderChangedMutexFactory;
		
	    IImageViewFactory imageViewFactory = new ImageViewFactory(_globalParameters, screenInformation);
	    IThumbnailInfoFactory thumbnailInfoFactory = new ThumbnailInfoFactory(_globalParameters);
	    IFolderVisualStateFactory folderVisualStateFactory = new FolderVisualStateFactory(
		    _globalParameters, _fileSizeEngine, thumbnailInfoFactory, _discQueryEngine);

	    IAboutInformationProvider aboutInformationProvider = new AboutInformationProvider();
	    IAboutViewFactory aboutViewFactory = new AboutViewFactory(aboutInformationProvider, _globalParameters);

	    var mainViewPresenter = new MainViewPresenter(
		    _discQueryEngine,
		    folderVisualStateFactory,
		    imageViewFactory,
		    _inputPathContainer,
		    _globalParameters,
		    aboutViewFactory,
		    mainView);

	    mainView.AddFakeTabItem();
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
	    
	    imageView.ViewClosing += OnImageViewClosing;

	    var imageViewPresenter = new ImageViewPresenter(
		    _discQueryEngine, _inputPathContainer, _globalParameters, imageView);
			
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
