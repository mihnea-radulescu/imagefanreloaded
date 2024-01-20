using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.ImageHandling.Implementation;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Factories.Implementation;
using ImageFanReloaded.Infrastructure;
using ImageFanReloaded.Infrastructure.Implementation;
using ImageFanReloaded.Presenters;
using ImageFanReloaded.Views;
using ImageFanReloaded.Views.Implementation;

namespace ImageFanReloaded;

public partial class App
    : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

		IImageFileFactory imageFileFactory = new ImageFileFactory(imageResizer);
		IDiscQueryEngineFactory discQueryEngineFactory =
			new DiscQueryEngineFactory(imageFileFactory);
		IDiscQueryEngine discQueryEngine = discQueryEngineFactory.GetDiscQueryEngine();

		var mainWindow = new MainWindow();
		IScreenInformation screenInformation = new ScreenInformation(mainWindow);
		IImageViewFactory imageViewFactory = new ImageViewFactory(screenInformation);

		var desktop = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime;
		desktop.MainWindow = mainWindow;

		IVisualActionDispatcher visualActionDispatcher = new VisualActionDispatcher(Dispatcher.UIThread);
		IFolderVisualStateFactory folderVisualStateFactory = new FolderVisualStateFactory();

		new MainPresenter(
			discQueryEngine,
			visualActionDispatcher,
			folderVisualStateFactory,
			imageViewFactory,
			mainWindow);

		mainWindow.AddFakeTabItem();

		mainWindow.Show();
	}
}
