using System.Linq;
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

    public override void OnFrameworkInitializationCompleted()
    {
	    var args = ((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!).Args!;
	    var inputPath = args.Any() ? args[0] : null;
	    
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

		IFolderChangedEventHandleFactory folderChangedEventHandleFactory = new FolderChangedEventHandleFactory();
		
		var mainWindow = new MainWindow();
		IMainView mainView = mainWindow;
		mainView.GlobalParameters = globalParameters;
		mainView.FolderChangedEventHandleFactory = folderChangedEventHandleFactory;
		
		var desktop = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
		desktop.MainWindow = mainWindow;
		IScreenInformation screenInformation = new ScreenInformation(mainWindow);
		
		IImageViewFactory imageViewFactory = new ImageViewFactory(globalParameters, screenInformation);
		IThumbnailInfoFactory thumbnailInfoFactory = new ThumbnailInfoFactory(globalParameters);
		IFolderVisualStateFactory folderVisualStateFactory = new FolderVisualStateFactory(
			globalParameters, fileSizeEngine, thumbnailInfoFactory, discQueryEngine);

		var mainPresenter = new MainPresenter(
			discQueryEngine,
			folderVisualStateFactory,
			imageViewFactory,
			mainView,
			inputPathContainer);

		mainView.AddFakeTabItem();
		mainView.Show();
	}
}
