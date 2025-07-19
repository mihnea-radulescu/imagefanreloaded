using Avalonia.Platform.Storage;

namespace ImageFanReloaded.Controls.Dialogs;

public class DefaultSaveFileDialog : SaveFileDialogBase
{
	public DefaultSaveFileDialog(IStorageProvider storageProvider)
		: base(storageProvider)
	{
	}

	public override bool ShouldAlwaysRefreshSaveFolder => false;
}
