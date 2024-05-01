using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;

namespace ImageFanReloaded.ImageHandling;

public class ImageFile : ImageFileBase
{
    public ImageFile(
	    IGlobalParameters globalParameters,
	    IImageResizer imageResizer,
	    string imageFileName,
	    string imageFilePath,
	    decimal sizeOnDiscInKilobytes)
		: base(globalParameters, imageResizer, imageFileName, imageFilePath, sizeOnDiscInKilobytes)
    {
    }

    #region Protected
    
    protected override IImage GetImageFromDisc(string imageFilePath)
    {
	    var bitmap = new Bitmap(ImageFilePath);
	    var bitmapSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);
	    
	    var image = new Image(bitmap, bitmapSize);
	    return image;
    }
    
    #endregion
}
