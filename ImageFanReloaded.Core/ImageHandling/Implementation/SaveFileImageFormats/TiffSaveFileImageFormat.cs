namespace ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

public class TiffSaveFileImageFormat : ISaveFileImageFormat
{
	public string Extension => ".tif";

	public bool IsAnimationEnabled => false;
}
