using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageFanReloaded.Core.Controls.Dialogs;

namespace ImageFanReloaded.Controls.Dialogs;

public abstract class SaveFileDialogBase : ISaveFileDialog
{
	static SaveFileDialogBase()
	{
		SupportedFileTypes = GetSupportedFileTypes();
	}

	public async Task<string?> ShowDialog(
		string imageFileName, string imageFolderPath, string saveFileDialogTitle)
	{
		var imageStorageFolder = await GetStorageFolderFromPath(imageFolderPath);

		var imageFileSaveOptions = new FilePickerSaveOptions
		{
			FileTypeChoices = SupportedFileTypes,
			ShowOverwritePrompt = true,
			SuggestedFileName = imageFileName,
			SuggestedStartLocation = imageStorageFolder,
			Title = saveFileDialogTitle
		};

		var imageToSaveStorageFile = await _storageProvider.SaveFilePickerAsync(imageFileSaveOptions);

		var imageToSaveFilePath = imageToSaveStorageFile?.Path.LocalPath;
		return imageToSaveFilePath;
	}

	public abstract bool ShouldAlwaysRefreshSaveFolder { get; }

	#region Protected

	protected SaveFileDialogBase(IStorageProvider storageProvider)
	{
		_storageProvider = storageProvider;
	}

	#endregion

	#region Private

	private static readonly IReadOnlyList<FilePickerFileType> SupportedFileTypes;

	private readonly IStorageProvider _storageProvider;

	private async Task<IStorageFolder?> GetStorageFolderFromPath(string folderPath)
		=> await _storageProvider.TryGetFolderFromPathAsync(folderPath);

	private static IReadOnlyList<FilePickerFileType> GetSupportedFileTypes()
	{
		var supportedFileTypes = new List<FilePickerFileType>
		{
			new FilePickerFileType("All files")
			{
				Patterns = [ "*.*" ]
			}
		};

		return supportedFileTypes;
	}

	#endregion
}
