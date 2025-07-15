using ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

namespace ImageFanReloaded.Core.ImageHandling.Factories.Implementation;

public class SaveFileImageFormatFactory : ISaveFileImageFormatFactory
{
	public SaveFileImageFormatFactory()
	{
		JpegSaveFileImageFormat = new JpegSaveFileImageFormat();
		GifSaveFileImageFormat = new GifSaveFileImageFormat();
		PngSaveFileImageFormat = new PngSaveFileImageFormat();
		WebpSaveFileImageFormat = new WebpSaveFileImageFormat();
		TiffSaveFileImageFormat = new TiffSaveFileImageFormat();
		BmpSaveFileImageFormat = new BmpSaveFileImageFormat();
	}

	public ISaveFileImageFormat JpegSaveFileImageFormat { get; }
	public ISaveFileImageFormat GifSaveFileImageFormat { get; }
	public ISaveFileImageFormat PngSaveFileImageFormat { get; }
	public ISaveFileImageFormat WebpSaveFileImageFormat { get; }
	public ISaveFileImageFormat TiffSaveFileImageFormat { get; }
	public ISaveFileImageFormat BmpSaveFileImageFormat { get; }
}
