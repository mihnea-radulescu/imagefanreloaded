using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ImageFanReloaded.Controls.Dialogs;
using ImageFanReloaded.Core.Controls.Dialogs;
using ImageFanReloaded.Core.Controls.Factories;

namespace ImageFanReloaded.Controls.Factories;

public class DefaultSaveFileDialogFactory : ISaveFileDialogFactory
{
	public DefaultSaveFileDialogFactory(Window window)
	{
		_storageProvider = window.StorageProvider;
	}

	public ISaveFileDialog GetSaveFileDialog() => new DefaultSaveFileDialog(_storageProvider);

	private readonly IStorageProvider _storageProvider;
}
