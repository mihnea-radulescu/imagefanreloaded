namespace ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

public class PngSaveFileImageFormat : ISaveFileImageFormat
{
	public string Extension => ".png";

	public bool IsAnimationEnabled => false;
}
