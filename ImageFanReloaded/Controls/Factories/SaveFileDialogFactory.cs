using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ImageFanReloaded.Controls.Dialogs;
using ImageFanReloaded.Core.Controls.Dialogs;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.RuntimeEnvironment;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls.Factories;

public class SaveFileDialogFactory : ISaveFileDialogFactory
{
	public SaveFileDialogFactory(
		IGlobalParameters globalParameters, Window window)
	{
		_globalParameters = globalParameters;
		_storageProvider = window.StorageProvider;
	}

	public ISaveFileDialog GetSaveFileDialog()
	{
		if (_globalParameters.RuntimeEnvironmentType is
			RuntimeEnvironmentType.LinuxFlatpak)
		{
			return new FlatpakSaveFileDialog(_storageProvider);
		}

		return new DefaultSaveFileDialog(_storageProvider);
	}

	private readonly IGlobalParameters _globalParameters;
	private readonly IStorageProvider _storageProvider;
}
