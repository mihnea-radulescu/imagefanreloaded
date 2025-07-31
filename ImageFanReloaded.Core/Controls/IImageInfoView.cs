using System.Threading.Tasks;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IImageInfoView
{
	IGlobalParameters? GlobalParameters { get; set; }

	void SetImageInfoText(string text);

	Task ShowDialog(IMainView owner);
}
