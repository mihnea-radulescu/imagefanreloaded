using System;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IEditableImage : IDisposable
{
	IImage ImageToDisplay { get; }
	ImageSize ImageSize { get; }

	bool CanUndoLastEdit { get; }
	bool CanRedoLastEdit { get; }

	void UndoLastEdit();
	void RedoLastEdit();

	void RotateLeft();
	void RotateRight();

	void FlipHorizontally();
	void FlipVertically();

	void Contrast();
	void Gamma();
	void Enhance();
	void WhiteBalance();
	void ReduceNoise();
	void Sharpen();
	void Blur();

	void Grayscale();
	void Sepia();
	void Negative();
	void OilPaint();
	void Emboss();

	Task SaveImageWithSameFormat(string imageFilePath);
	Task SaveImageWithFormat(
		string imageFilePath, ISaveFileImageFormat saveFileImageFormat);

	void Crop(int topLeftPointX, int topLeftPointY, int width, int height);

	void DownsizeToPercentage(int percentage);
	void DownsizeToDimensions(int width, int height);
}
