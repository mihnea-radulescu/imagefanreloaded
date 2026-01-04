using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ImageFanReloaded.Controls.Dialogs;
using ImageFanReloaded.Core.Controls.Dialogs;
using ImageFanReloaded.Core.Controls.Factories;

namespace ImageFanReloaded.Controls.Factories;

public class FlatpakSaveFileDialogFactory : ISaveFileDialogFactory
{
	public FlatpakSaveFileDialogFactory(Window window)
	{
		_storageProvider = window.StorageProvider;
	}

	public ISaveFileDialog GetSaveFileDialog() => new FlatpakSaveFileDialog(_storageProvider);

	private readonly IStorageProvider _storageProvider;
}
