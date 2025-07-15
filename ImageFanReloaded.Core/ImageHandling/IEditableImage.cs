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

	void Blur();
	void Sharpen();
	void ReduceNoise();
	void Enhance();
	void WhiteBalance();
	void Emboss();
	void Grayscale();
	void Negative();
	void Sepia();
	void OilPaint();

	Task SaveImageWithSameFormat(string imageFilePath);
	Task SaveImageWithFormat(string imageFilePath, ISaveFileImageFormat saveFileImageFormat);

	void DownsizeToPercentage(int percentage);
	void DownsizeToDimensions(int width, int height);
}
