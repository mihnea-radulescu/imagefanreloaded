using ImageFanReloaded.Controls.Dialogs;
using ImageFanReloaded.Core.Controls.Dialogs;
using ImageFanReloaded.Core.Controls.Factories;

namespace ImageFanReloaded.Controls.Factories;

public class FlatpakSaveFileDialogFactory : ISaveFileDialogFactory
{
	public ISaveFileDialog GetSaveFileDialog() => new FlatpakSaveFileDialog();
}
