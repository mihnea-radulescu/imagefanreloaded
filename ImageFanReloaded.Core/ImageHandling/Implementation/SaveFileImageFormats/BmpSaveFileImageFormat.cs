namespace
	ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

public class BmpSaveFileImageFormat : ISaveFileImageFormat
{
	public string Name => "BMP";
	public string Extension => ".bmp";

	public bool IsAnimationEnabled => false;
}
