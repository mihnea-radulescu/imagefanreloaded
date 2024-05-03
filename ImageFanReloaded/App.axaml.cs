using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Core;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Implementation;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Synchronization;
using ImageFanReloaded.Core.Synchronization.Implementation;
using ImageFanReloaded.Global;
using ImageFanReloaded.ImageHandling;

namespace ImageFanReloaded;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
	    var desktop = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
	    var args = desktop.Args!;
	    var inputPath = args.Any() ? args.First() : null;
	    
	    IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

		IGlobalParameters globalParameters = new GlobalParameters(imageResizeCalculator, imageResizer);
		IFileSizeEngine fileSizeEngine = new FileSizeEngine();

		IImageFileFactory imageFileFactory = new ImageFileFactory(globalParameters, imageResizer);
		IDiscQueryEngineFactory discQueryEngineFactory = new DiscQueryEngineFactory(
			globalParameters, fileSizeEngine, imageFileFactory);
		IDiscQueryEngine discQueryEngine = discQueryEngineFactory.GetDiscQueryEngine();
		
		IInputPathContainer inputPathContainer = new InputPathContainer(
			globalParameters, discQueryEngine, inputPath);

		if (IsMainViewAccess(inputPathContainer))
		{
			BootstrapMainViewAccess(desktop, globalParameters, fileSizeEngine, discQueryEngine, inputPathContainer);
		}
		else
		{
			await BootstrapImageViewAccess(desktop, globalParameters, discQueryEngine, inputPathContainer);
		}
	}

    #region Private
    
    private static bool IsMainViewAccess(IInputPathContainer inputPathContainer)
		=> inputPathContainer.InputPathType != InputPathType.File;
    
    private static void BootstrapMainViewAccess(
	    IClassicDesktopStyleApplicationLifetime desktop,
	    IGlobalParameters globalParameters,
	    IFileSizeEngine fileSizeEngine,
	    IDiscQueryEngine discQueryEngine,
	    IInputPathContainer inputPathContainer)
    {
	    IFolderChangedEventHandleFactory folderChangedEventHandleFactory = new FolderChangedEventHandleFactory();
		
	    var mainWindow = new MainWindow();
	    desktop.MainWindow = mainWindow;
	    IScreenInformation screenInformation = new ScreenInformation(mainWindow);
			
	    IMainView mainView = mainWindow;
	    mainView.GlobalParameters = globalParameters;
	    mainView.FolderChangedEventHandleFactory = folderChangedEventHandleFactory;
		
	    IImageViewFactory imageViewFactory = new ImageViewFactory(globalParameters, screenInformation);
	    IThumbnailInfoFactory thumbnailInfoFactory = new ThumbnailInfoFactory(globalParameters);
	    IFolderVisualStateFactory folderVisualStateFactory = new FolderVisualStateFactory(
		    globalParameters, fileSizeEngine, thumbnailInfoFactory, discQueryEngine);

	    var mainViewPresenter = new MainViewPresenter(
		    discQueryEngine,
		    folderVisualStateFactory,
		    imageViewFactory,
		    inputPathContainer,
		    mainView);

	    mainView.AddFakeTabItem();
	    mainView.Show();
    }
    
    private static async Task BootstrapImageViewAccess(
	    IClassicDesktopStyleApplicationLifetime desktop,
	    IGlobalParameters globalParameters,
	    IDiscQueryEngine discQueryEngine,
	    IInputPathContainer inputPathContainer)
    {
	    var imageWindow = new ImageWindow();
	    desktop.MainWindow = imageWindow;
	    IScreenInformation screenInformation = new ScreenInformation(imageWindow);
			
	    IImageView imageView = imageWindow;
	    imageView.GlobalParameters = globalParameters;
	    imageView.ScreenInformation = screenInformation;

	    var imageViewPresenter = new ImageViewPresenter(
		    discQueryEngine,
		    inputPathContainer,
		    globalParameters,
		    imageView);

	    await imageViewPresenter.SetUpAccess();
	    
	    imageView.Show();
    }
    
    #endregion
}
