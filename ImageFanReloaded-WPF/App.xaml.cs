using System.Windows;
using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.CommonTypes.Disc.Implementation;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.ImageHandling.Implementation;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Factories.Implementation;
using ImageFanReloaded.Infrastructure;
using ImageFanReloaded.Infrastructure.Implementation;
using ImageFanReloaded.Presenters;
using ImageFanReloaded.Views;
using ImageFanReloaded.Views.Implementation;

namespace ImageFanReloaded
{
    public partial class App
        : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
            IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

			IImageFileFactory imageFileFactory = new ImageFileFactory(imageResizer);
			IFileSystemEntryComparer fileSystemEntryComparer = new FileSystemEntryComparer();
            IDiscQueryEngine discQueryEngine = new DiscQueryEngine(imageFileFactory, fileSystemEntryComparer);

            var mainWindow = new MainWindow();
			IScreenInformation screenInformation = new ScreenInformation(mainWindow);
            IImageViewFactory imageViewFactory = new ImageViewFactory(screenInformation);

            IVisualActionDispatcher visualActionDispatcher = new VisualActionDispatcher(Dispatcher);
            IFolderVisualStateFactory folderVisualStateFactory = new FolderVisualStateFactory();

            new MainPresenter(
                discQueryEngine,
                visualActionDispatcher,
                folderVisualStateFactory,
                imageViewFactory,
                mainWindow);

			mainWindow.AddFakeTabItem();
			mainWindow.AddContentTabItem();

			mainWindow.Show();
		}
    }
}
