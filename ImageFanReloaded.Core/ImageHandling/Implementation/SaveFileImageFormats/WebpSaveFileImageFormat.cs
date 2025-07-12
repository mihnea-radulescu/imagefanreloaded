namespace ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

public class WebpSaveFileImageFormat : ISaveFileImageFormat
{
	public string Name => "WEBP";
	public string Extension => ".webp";

	public bool IsAnimationEnabled => true;
}
