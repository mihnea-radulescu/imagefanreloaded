using System;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IImageEditView
{
	IGlobalParameters? GlobalParameters { get; set; }
	ISaveFileDialogFactory? SaveFileDialogFactory { get; set; }
	ImageFileData? ImageFileData { get; set; }

	IContentTabItem? ContentTabItem { get; set; }

	Task LoadImage();

	event EventHandler<ContentTabItemEventArgs>? ImageChanged;
	event EventHandler<ContentTabItemEventArgs>? FolderChanged;

	Task ShowDialog(IMainView owner);
}
