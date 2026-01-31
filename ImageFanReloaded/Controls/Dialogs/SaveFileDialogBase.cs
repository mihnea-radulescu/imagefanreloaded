using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageFanReloaded.Core.Controls.Dialogs;

namespace ImageFanReloaded.Controls.Dialogs;

public abstract class SaveFileDialogBase : ISaveFileDialog
{
	public async Task<string?> ShowDialog(
		string imageFileName,
		string imageFolderPath,
		string saveFileDialogTitle)
	{
		var imageStorageFolder = await GetStorageFolderFromPath(
			imageFolderPath);

		var imageFileSaveOptions = new FilePickerSaveOptions
		{
			FileTypeChoices = SupportedFileTypes,
			ShowOverwritePrompt = true,
			SuggestedFileName = imageFileName,
			SuggestedStartLocation = imageStorageFolder,
			Title = saveFileDialogTitle
		};

		var imageToSaveStorageFile = await _storageProvider.SaveFilePickerAsync(
			imageFileSaveOptions);

		var imageToSaveFilePath = imageToSaveStorageFile?.Path.LocalPath;
		return imageToSaveFilePath;
	}

	public abstract bool ShouldAlwaysRefreshSaveFolder { get; }

	protected SaveFileDialogBase(IStorageProvider storageProvider)
	{
		_storageProvider = storageProvider;
	}

	private static readonly IReadOnlyList<FilePickerFileType>
		SupportedFileTypes =
		[
			new("All files")
			{
				Patterns = ["*.*"]
			}
		];

	private readonly IStorageProvider _storageProvider;

	private async Task<IStorageFolder?> GetStorageFolderFromPath(
		string folderPath)
			=> await _storageProvider.TryGetFolderFromPathAsync(folderPath);
}
