namespace ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

public class JpegSaveFileImageFormat : ISaveFileImageFormat
{
	public string Name => "JPEG";
	public string Extension => ".jpg";

	public bool IsAnimationEnabled => false;
}
