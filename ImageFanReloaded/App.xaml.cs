using ImageFanReloaded.Factories;
using ImageFanReloaded.Presenters;
using System.Windows;

namespace ImageFanReloaded
{
    public partial class App
        : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainView = TypesFactoryResolver.TypesFactoryInstance.GetMainView();
            new MainPresenter(mainView);
            mainView.Show();
        }
    }
}
