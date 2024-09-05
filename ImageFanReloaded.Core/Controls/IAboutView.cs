using System.Threading.Tasks;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IAboutView
{
    IGlobalParameters? GlobalParameters { get; set; }
    
    void SetAboutText(string text);

    Task ShowDialog(IMainView owner);
}
