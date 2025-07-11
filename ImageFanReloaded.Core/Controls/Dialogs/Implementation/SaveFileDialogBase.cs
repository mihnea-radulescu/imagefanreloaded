using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Controls.Dialogs.Implementation;

public abstract class SaveFileDialogBase : ISaveFileDialog
{
	public abstract Task<string?> ShowDialog(string imageFileName, string imageFolderPath);

	#region Protected

	protected const string SaveFileDialogTitle = "Select image file";

	#endregion
}
