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
		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

		IGlobalParameters globalParameters = new GlobalParameters(
			imageResizeCalculator, imageResizer);

		IImageFileFactory imageFileFactory = new ImageFileFactory(globalParameters, imageResizer);
		IDiscQueryEngineFactory discQueryEngineFactory = new DiscQueryEngineFactory(
			globalParameters, imageFileFactory);
		IDiscQueryEngine discQueryEngine = discQueryEngineFactory.GetDiscQueryEngine();
		
		var mainWindow = new MainWindow();
		IMainView mainView = mainWindow;
		mainView.GlobalParameters = globalParameters;
		
		var desktop = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
		desktop.MainWindow = mainWindow;
		IScreenInformation screenInformation = new ScreenInformation(mainWindow);
		
		IImageViewFactory imageViewFactory = new ImageViewFactory(globalParameters, screenInformation);
		IDispatcher dispatcher = new Dispatcher(Avalonia.Threading.Dispatcher.UIThread);
		IThumbnailInfoFactory thumbnailInfoFactory = new ThumbnailInfoFactory(globalParameters, dispatcher);
		IFolderVisualStateFactory folderVisualStateFactory = new FolderVisualStateFactory(
			globalParameters, thumbnailInfoFactory, discQueryEngine, dispatcher);

		var mainPresenter = new MainPresenter(
			discQueryEngine,
			folderVisualStateFactory,
			imageViewFactory,
			mainView);

		mainView.AddFakeTabItem();
		mainView.Show();
	}
}
