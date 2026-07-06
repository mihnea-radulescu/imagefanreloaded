using ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

namespace ImageFanReloaded.Core.ImageHandling.Factories.Implementation;

public class SaveFileImageFormatFactory : ISaveFileImageFormatFactory
{
	public SaveFileImageFormatFactory()
	{
		JpegSaveFileImageFormat = new JpegSaveFileImageFormat();
		WebpSaveFileImageFormat = new WebpSaveFileImageFormat();
		AvifSaveFileImageFormat = new AvifSaveFileImageFormat();
		JxlSaveFileImageFormat = new JxlSaveFileImageFormat();
		GifSaveFileImageFormat = new GifSaveFileImageFormat();
		PngSaveFileImageFormat = new PngSaveFileImageFormat();
		TiffSaveFileImageFormat = new TiffSaveFileImageFormat();
		BmpSaveFileImageFormat = new BmpSaveFileImageFormat();
	}

	public ISaveFileImageFormat JpegSaveFileImageFormat { get; }
	public ISaveFileImageFormat WebpSaveFileImageFormat { get; }
	public ISaveFileImageFormat AvifSaveFileImageFormat { get; }
	public ISaveFileImageFormat JxlSaveFileImageFormat { get; }
	public ISaveFileImageFormat GifSaveFileImageFormat { get; }
	public ISaveFileImageFormat PngSaveFileImageFormat { get; }
	public ISaveFileImageFormat TiffSaveFileImageFormat { get; }
	public ISaveFileImageFormat BmpSaveFileImageFormat { get; }
}
