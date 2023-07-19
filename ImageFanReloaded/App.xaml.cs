using System.Windows;

using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Infrastructure;
using ImageFanReloaded.Presenters;
using ImageFanReloaded.Views;

namespace ImageFanReloaded
{
    public partial class App
        : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var imageFileFactory = new ImageFileFactory();
            var imageResizer = new ImageResizer();
            var fileSystemEntryComparer = new FileSystemEntryComparer();
            var discQueryEngine = new DiscQueryEngine(imageFileFactory, imageResizer, fileSystemEntryComparer);

            var imageViewFactory = new ImageViewFactory();
            var mainView = new MainView(imageViewFactory);

            var visualActionDispatcher = new VisualActionDispatcher(Dispatcher);
            var folderVisualStateFactory = new FolderVisualStateFactory();

            new MainPresenter(discQueryEngine, mainView, visualActionDispatcher, folderVisualStateFactory);
            mainView.Show();
        }
    }
}
