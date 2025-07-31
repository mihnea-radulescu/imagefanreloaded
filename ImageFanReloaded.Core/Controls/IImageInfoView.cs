using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IImageInfoView
{
	IGlobalParameters? GlobalParameters { get; set; }
	IImageInfoBuilder? ImageInfoBuilder { get; set; }

	IImageFile? ImageFile { get; set; }

	Task ShowDialog(IMainView owner);
}
