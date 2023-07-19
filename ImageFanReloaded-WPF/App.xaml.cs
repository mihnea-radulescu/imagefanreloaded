using System.Windows;

using ImageFanReloadedWPF.CommonTypes.Disc;
using ImageFanReloadedWPF.CommonTypes.ImageHandling;
using ImageFanReloadedWPF.Factories;
using ImageFanReloadedWPF.Infrastructure;
using ImageFanReloadedWPF.Presenters;
using ImageFanReloadedWPF.Views;

namespace ImageFanReloadedWPF
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
