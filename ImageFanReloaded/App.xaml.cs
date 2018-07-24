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

            var imageViewFactory = new ImageViewFactory();
            var imageFileFactory = new ImageFileFactory();
            var imageResizer = new ImageResizer();
            var discQueryEngine = new DiscQueryEngine(imageFileFactory, imageResizer);

            var mainView = new MainView(imageViewFactory);

            var visualActionDispatcher = new VisualActionDispatcher(Dispatcher);
            var folderVisualStateFactory = new FolderVisualStateFactory();

            new MainPresenter(discQueryEngine, mainView, visualActionDispatcher, folderVisualStateFactory);
            mainView.Show();
        }
    }
}
