using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Core;
using ImageFanReloaded.Core.AboutInformation;
using ImageFanReloaded.Core.AboutInformation.Implementation;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Implementation;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.OperatingSystem;
using ImageFanReloaded.Core.OperatingSystem.Implementation;
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

	    IOperatingSystemSettings operatingSystemSettings = new OperatingSystemSettings();
	    IAboutInformationProvider aboutInformationProvider = new AboutInformationProvider();
	    
	    IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

		IGlobalParameters globalParameters = new GlobalParameters(
			operatingSystemSettings, aboutInformationProvider, imageResizer);
		IFileSizeEngine fileSizeEngine = new FileSizeEngine();

		IImageFileFactory imageFileFactory = new ImageFileFactory(globalParameters, imageResizer);
		IDiscQueryEngineFactory discQueryEngineFactory = new DiscQueryEngineFactory(
			globalParameters, fileSizeEngine, imageFileFactory);
		IDiscQueryEngine discQueryEngine = discQueryEngineFactory.GetDiscQueryEngine();
		
		IInputPathContainer inputPathContainer = new InputPathContainer(
			globalParameters, discQueryEngine, inputPath);
		
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
			globalParameters,
			mainView);

		mainView.AddFakeTabItem();
		mainView.Show();

		if (IsImageViewAccess(inputPathContainer))
		{
			var imageWindow = new ImageWindow();
			
			IImageView imageView = imageWindow;
			imageView.GlobalParameters = globalParameters;
			imageView.ScreenInformation = screenInformation;

			var imageViewPresenter = new ImageViewPresenter(
				discQueryEngine, inputPathContainer, globalParameters, imageView);
			
			await imageViewPresenter.SetUpAccess();
	    
			await imageView.ShowDialog(mainView);
		}
	}

    #region Private
    
    private static bool IsImageViewAccess(IInputPathContainer inputPathContainer)
		=> inputPathContainer.InputPathType == InputPathType.File;
    
    #endregion
}
