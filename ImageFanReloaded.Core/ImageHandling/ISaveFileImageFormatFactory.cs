namespace ImageFanReloaded.Core.ImageHandling;

public interface ISaveFileImageFormatFactory
{
	ISaveFileImageFormat JpegSaveFileImageFormat { get; }
	ISaveFileImageFormat GifSaveFileImageFormat { get; }
	ISaveFileImageFormat PngSaveFileImageFormat { get; }
	ISaveFileImageFormat WebpSaveFileImageFormat { get; }
	ISaveFileImageFormat TiffSaveFileImageFormat { get; }
	ISaveFileImageFormat BmpSaveFileImageFormat { get; }
}
