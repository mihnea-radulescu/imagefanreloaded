using System.Threading.Tasks;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface ITabOptionsView
{
    IGlobalParameters? GlobalParameters { get; set; }
    
    Task ShowDialog(IMainView owner);
}
