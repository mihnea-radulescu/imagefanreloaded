using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ImageMagick;
using ImageFanReloaded.Core.BaseTypes;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageCore.Implementation;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.ImageHandling.Extensions;

namespace ImageFanReloaded.ImageHandling;

public class EditableImage : DisposableBase, IEditableImage
{
	public EditableImage(string imageFilePath, int imageQualityLevel)
	{
		_imageQualityLevel = imageQualityLevel;

		MagickImageCollection? imageFramesToEdit = null;
		Bitmap? bitmapToDisplay = null;

		try
		{
			imageFramesToEdit = new MagickImageCollection(imageFilePath);

			if (imageFramesToEdit.Count > 1)
			{
				imageFramesToEdit.Coalesce();
			}
			else
			{
				imageFramesToEdit[0].AutoOrient();
			}

			SetImageQualityLevel(imageFramesToEdit);

			using var imageToDisplayStream = new MemoryStream();
			imageFramesToEdit[0].Write(imageToDisplayStream, MagickFormat.Jpg);
			imageToDisplayStream.Reset();

			bitmapToDisplay = new Bitmap(imageToDisplayStream);

			var imageSize = new ImageSize(
				bitmapToDisplay.Size.Width, bitmapToDisplay.Size.Height);
			var imageToDisplay = new Image(bitmapToDisplay, imageSize);

			_editableImageData = new EditableImageData(
				imageFramesToEdit, imageToDisplay, imageSize);

			_previousOperationsStack = new Stack<EditableImageData>();
			_revertedOperationsStack = new Stack<EditableImageData>();
		}
		catch
		{
			imageFramesToEdit?.Dispose();
			bitmapToDisplay?.Dispose();

			throw;
		}
	}

	public IImage ImageToDisplay
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _editableImageData.ImageToDisplay;
		}
	}

	public ImageSize ImageSize
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _editableImageData.ImageSize;
		}
	}

	public bool CanUndoLastEdit
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _previousOperationsStack.Any();
		}
	}

	public bool CanRedoLastEdit
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _revertedOperationsStack.Any();
		}
	}

	public void UndoLastEdit()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		if (!CanUndoLastEdit)
		{
			return;
		}

		_revertedOperationsStack.Push(_editableImageData);
		_editableImageData = _previousOperationsStack.Pop();
	}

	public void RedoLastEdit()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		if (!CanRedoLastEdit)
		{
			return;
		}

		_previousOperationsStack.Push(_editableImageData);
		_editableImageData = _revertedOperationsStack.Pop();
	}

	public void RotateLeft()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.Rotate(270));
	}

	public void RotateRight()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.Rotate(90));
	}

	public void FlipHorizontally()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.Flop());
	}

	public void FlipVertically()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.Flip());
	}

	public void Contrast()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.Contrast());
	}

	public void Gamma()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.AutoGamma());
	}

	public void Enhance()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.Enhance());
	}

	public void WhiteBalance()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.WhiteBalance());
	}

	public void ReduceNoise()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.ReduceNoise());
	}

	public void Sharpen()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.Sharpen());
	}

	public void Blur()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.Blur());
	}

	public void Grayscale()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.Grayscale());
	}

	public void Sepia()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.SepiaTone());
	}

	public void Negative()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.Negate());
	}

	public void OilPaint()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.OilPaint());
	}

	public void Emboss()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(anImageFrame => anImageFrame.Emboss());
	}

	public async Task SaveImageWithSameFormat(string imageFilePath)
	{
		ThrowObjectDisposedExceptionIfNecessary();

		SetImageQualityLevel(CurrentImage);

		await CurrentImage.WriteAsync(imageFilePath);
	}

	public async Task SaveImageWithFormat(
		string imageFilePath, ISaveFileImageFormat saveFileImageFormat)
	{
		ThrowObjectDisposedExceptionIfNecessary();

		SetImageQualityLevel(CurrentImage);

		await saveFileImageFormat.SaveImage(CurrentImage, imageFilePath);
	}

	public void Crop(
		int topLeftPointX, int topLeftPointY, int width, int height)
	{
		ThrowObjectDisposedExceptionIfNecessary();

		var imageFrameCropGeometry =
			new MagickGeometry(
				$"{width}x{height}+{topLeftPointX}+{topLeftPointY}")
			{
				IgnoreAspectRatio = true
			};

		CreateTransformedImage(anImageFrame
			=> anImageFrame.Crop(imageFrameCropGeometry));
	}

	public void DownsizeToPercentage(int percentage)
	{
		ThrowObjectDisposedExceptionIfNecessary();

		if (percentage is < 1 or >= 100)
		{
			throw new ArgumentOutOfRangeException(nameof(percentage));
		}

		CreateTransformedImage(anImageFrame
			=> anImageFrame.Resize(new Percentage(percentage)));
	}

	public void DownsizeToDimensions(int width, int height)
	{
		ThrowObjectDisposedExceptionIfNecessary();

		if (width < 1 || width >= _editableImageData.ImageSize.Width)
		{
			throw new ArgumentOutOfRangeException(nameof(width));
		}

		if (height < 1 || height >= _editableImageData.ImageSize.Height)
		{
			throw new ArgumentOutOfRangeException(nameof(height));
		}

		CreateTransformedImage(anImageFrame
			=> anImageFrame.Resize((uint)width, (uint)height));
	}

	protected override void DisposeSpecific()
	{
		_editableImageData.Dispose();

		foreach (var editableImageData in _previousOperationsStack)
		{
			editableImageData.Dispose();
		}

		foreach (var editableImageData in _revertedOperationsStack)
		{
			editableImageData.Dispose();
		}
	}

	private readonly int _imageQualityLevel;

	private EditableImageData _editableImageData;

	private readonly Stack<EditableImageData> _previousOperationsStack;
	private readonly Stack<EditableImageData> _revertedOperationsStack;

	private MagickImageCollection CurrentImage
		=> _editableImageData.ImageFramesToEdit;

	private void CreateTransformedImage(
		Action<IMagickImage> transformImageFrameAction)
	{
		var transformedImageFramesToEdit = CopyImage(CurrentImage);

		try
		{
			foreach (var aTransformedImageFrameToEdit in
						 transformedImageFramesToEdit)
			{
				transformImageFrameAction(aTransformedImageFrameToEdit);
			}
		}
		catch
		{
			transformedImageFramesToEdit.Dispose();

			throw;
		}

		using var imageToDisplayStream = new MemoryStream();
		transformedImageFramesToEdit[0].Write(
			imageToDisplayStream, MagickFormat.Jpg);
		imageToDisplayStream.Reset();

		var transformedBitmapToDisplay = new Bitmap(imageToDisplayStream);

		var transformedImageSize = new ImageSize(
			transformedBitmapToDisplay.Size.Width,
			transformedBitmapToDisplay.Size.Height);
		var transformedImageToDisplay = new Image(
			transformedBitmapToDisplay, transformedImageSize);

		var transformedEditableImageData = new EditableImageData(
			transformedImageFramesToEdit,
			transformedImageToDisplay,
			transformedImageSize);

		_previousOperationsStack.Push(_editableImageData);
		_revertedOperationsStack.Clear();

		_editableImageData = transformedEditableImageData;
	}

	private static MagickImageCollection CopyImage(
		MagickImageCollection sourceImageFrames)
	{
		var magickFormat = sourceImageFrames[0].Format;

		using var copyImageFramesStream = new MemoryStream();
		sourceImageFrames.Write(copyImageFramesStream);
		copyImageFramesStream.Reset();

		var destinationImageFrames = new MagickImageCollection(
			copyImageFramesStream, magickFormat);

		return destinationImageFrames;
	}

	private void SetImageQualityLevel(MagickImageCollection image)
	{
		var imageFrameQualityLevel = (uint)_imageQualityLevel;

		foreach (var anImageFrame in image)
		{
			anImageFrame.Quality = imageFrameQualityLevel;
		}
	}
}
