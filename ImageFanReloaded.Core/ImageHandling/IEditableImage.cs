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

	void DownsizeToPercentage(int percentage);
	void DownsizeToDimensions(int width, int height);

	Task SaveImageWithSameFormat(string imageFilePath);
	Task SaveImageWithFormat(string imageFilePath, ISaveFileImageFormat saveFileImageFormat);
}
