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

            IImageFileFactory imageFileFactory = new ImageFileFactory();

            IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
            IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

            IFileSystemEntryComparer fileSystemEntryComparer =
                new FileSystemEntryComparer();
            IDiscQueryEngine discQueryEngine = 
                new DiscQueryEngine(imageFileFactory, imageResizer, fileSystemEntryComparer);

            var mainWindow = new MainWindow();
			IScreenInformation screenInformation = new ScreenInformation(mainWindow);
            IImageViewFactory imageViewFactory = new ImageViewFactory(screenInformation);
            mainWindow.ImageViewFactory = imageViewFactory;

            IVisualActionDispatcher visualActionDispatcher = new VisualActionDispatcher(Dispatcher);
            IFolderVisualStateFactory folderVisualStateFactory = new FolderVisualStateFactory();

            new MainPresenter(discQueryEngine, mainWindow, visualActionDispatcher, folderVisualStateFactory);
            mainWindow.Show();
        }
    }
}
