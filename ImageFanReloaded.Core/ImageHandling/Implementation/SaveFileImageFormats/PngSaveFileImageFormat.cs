namespace
	ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

public class PngSaveFileImageFormat : ISaveFileImageFormat
{
	public string Name => "PNG";
	public string Extension => ".png";

	public bool IsAnimationEnabled => false;
}
