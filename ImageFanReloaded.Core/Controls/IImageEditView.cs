using System;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IImageEditView
{
	IGlobalParameters? GlobalParameters { get; set; }

	IMouseCursorFactory? MouseCursorFactory { get; set; }
	IEditableImageFactory? EditableImageFactory { get; set; }
	ISaveFileImageFormatFactory? SaveFileImageFormatFactory { get; set; }
	ISaveFileDialogFactory? SaveFileDialogFactory { get; set; }

	StaticImageFileData? StaticImageFileData { get; set; }

	IContentTabItem? ContentTabItem { get; set; }

	event EventHandler<ContentTabItemEventArgs>? ImageFileOverwritten;
	event EventHandler<ContentTabItemEventArgs>? FolderContentChanged;

	Task ShowDialog(IMainView owner);
}
