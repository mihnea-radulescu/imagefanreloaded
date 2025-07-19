using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls.Factories;

public class ImageEditViewFactory : IImageEditViewFactory
{
	public ImageEditViewFactory(
		IGlobalParameters globalParameters,
		IMouseCursorFactory mouseCursorFactory,
		IEditableImageFactory editableImageFactory,
		ISaveFileImageFormatFactory saveFileImageFormatFactory,
		ISaveFileDialogFactory saveFileDialogFactory)
	{
		_globalParameters = globalParameters;

		_mouseCursorFactory = mouseCursorFactory;
		_editableImageFactory = editableImageFactory;
		_saveFileImageFormatFactory = saveFileImageFormatFactory;
		_saveFileDialogFactory = saveFileDialogFactory;
	}

	public IImageEditView GetImageEditView(IContentTabItem contentTabItem, IImageFile imageFile)
	{
		IImageEditView imageEditView = new ImageEditWindow();

		imageEditView.GlobalParameters = _globalParameters;

		imageEditView.MouseCursorFactory = _mouseCursorFactory;
		imageEditView.EditableImageFactory = _editableImageFactory;
		imageEditView.SaveFileImageFormatFactory = _saveFileImageFormatFactory;
		imageEditView.SaveFileDialogFactory = _saveFileDialogFactory;

		imageEditView.ImageFileData = imageFile.ImageFileData;

		imageEditView.ContentTabItem = contentTabItem;

		return imageEditView;
	}

	#region Private

	private readonly IGlobalParameters _globalParameters;

	private readonly IMouseCursorFactory _mouseCursorFactory;
	private readonly IEditableImageFactory _editableImageFactory;
	private readonly ISaveFileImageFormatFactory _saveFileImageFormatFactory;
	private readonly ISaveFileDialogFactory _saveFileDialogFactory;

	#endregion
}
