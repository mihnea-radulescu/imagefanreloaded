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

public abstract class AppBootstrapBase : IAppBootstrap
{
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
	
	#region Protected
	
	protected AppBootstrapBase(IClassicDesktopStyleApplicationLifetime desktop)
	{
		Desktop = desktop;
	}
	
	protected readonly IClassicDesktopStyleApplicationLifetime Desktop;

	protected abstract string? GetInputPath();
	protected abstract IAppSettings GetAppSettings();
	
	#endregion

    #region Private
    
    private IGlobalParameters _globalParameters = null!;
    private IFileSizeEngine _fileSizeEngine = null!;
    private IDiscQueryEngine _discQueryEngine = null!;
    private IInputPathContainer _inputPathContainer = null!;
    
    private void BootstrapTypes()
    {
	    string? inputPath = GetInputPath();
	    
	    IOperatingSystemSettings operatingSystemSettings = new OperatingSystemSettings();
	    IAboutInformationProvider aboutInformationProvider = new AboutInformationProvider();
	    
	    IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
	    IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

	    IAppSettings appSettings = GetAppSettings();

	    _globalParameters = new GlobalParameters(
		    operatingSystemSettings, aboutInformationProvider, imageResizer, appSettings);
	    
	    _fileSizeEngine = new FileSizeEngine();
	    
	    IImageFileFactory imageFileFactory = new ImageFileFactory(_globalParameters, imageResizer);
	    IDiscQueryEngineFactory discQueryEngineFactory = new DiscQueryEngineFactory(
		    _globalParameters, _fileSizeEngine, imageFileFactory);
	    _discQueryEngine = discQueryEngineFactory.GetDiscQueryEngine();
		
	    _inputPathContainer = new InputPathContainer(_globalParameters, _discQueryEngine, inputPath);
    }
    
    private bool IsMainViewAccess() => _inputPathContainer.InputPathType != InputPathType.File;
    
    private void ShowMainView()
    {
	    IFolderChangedMutexFactory folderChangedMutexFactory = new FolderChangedMutexFactory();
		
	    var mainWindow = new MainWindow();
	    Desktop.MainWindow = mainWindow;
	    IScreenInformation screenInformation = new ScreenInformation(mainWindow);
			
	    IMainView mainView = mainWindow;
	    mainView.GlobalParameters = _globalParameters;
	    mainView.FolderChangedMutexFactory = folderChangedMutexFactory;
		
	    IImageViewFactory imageViewFactory = new ImageViewFactory(_globalParameters, screenInformation);
	    IThumbnailInfoFactory thumbnailInfoFactory = new ThumbnailInfoFactory(_globalParameters);
	    IFolderVisualStateFactory folderVisualStateFactory = new FolderVisualStateFactory(
		    _globalParameters, _fileSizeEngine, thumbnailInfoFactory, _discQueryEngine);

	    var mainViewPresenter = new MainViewPresenter(
		    _discQueryEngine,
		    folderVisualStateFactory,
		    imageViewFactory,
		    _inputPathContainer,
		    _globalParameters,
		    mainView);

	    mainView.AddFakeTabItem();
	    mainView.Show();
    }
    
    private async Task ShowImageView()
    {
	    var imageWindow = new ImageWindow();
	    Desktop.MainWindow = imageWindow;
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
