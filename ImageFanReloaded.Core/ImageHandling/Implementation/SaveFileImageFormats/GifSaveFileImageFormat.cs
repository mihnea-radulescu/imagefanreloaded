namespace
	ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

public class GifSaveFileImageFormat : ISaveFileImageFormat
{
	public string Name => "GIF";
	public string Extension => ".gif";

	public bool IsAnimationEnabled => true;
}
