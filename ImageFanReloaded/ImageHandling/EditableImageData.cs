using Avalonia.Media.Imaging;
using ImageMagick;
using ImageFanReloaded.Core.BaseTypes;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.ImageHandling;

public class EditableImageData : DisposableBase
{
	public EditableImageData(
		MagickImageCollection imageFramesToEdit, Bitmap imageToDisplay, ImageSize imageSize)
	{
		_imageFramesToEdit = imageFramesToEdit;
		_imageToDisplay = imageToDisplay;
		_imageSize = imageSize;
	}

	public MagickImageCollection ImageFramesToEdit
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _imageFramesToEdit;
		}
	}

	public Bitmap ImageToDisplay
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _imageToDisplay;
		}
	}

	public ImageSize ImageSize
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _imageSize;
		}
	}

	#region Protected

	protected override void DisposeSpecific()
	{
		_imageFramesToEdit.Dispose();
		_imageToDisplay.Dispose();
	}

	#endregion

	#region Private

	private readonly MagickImageCollection _imageFramesToEdit;
	private readonly Bitmap _imageToDisplay;
	private readonly ImageSize _imageSize;
	
	#endregion
}
