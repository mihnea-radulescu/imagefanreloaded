using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageFanReloaded.Core.Controls.Dialogs.Implementation;

namespace ImageFanReloaded.Controls.Dialogs;

public class DefaultSaveFileDialog : SaveFileDialogBase
{
	static DefaultSaveFileDialog()
	{
		SupportedFileTypes = GetSupportedFileTypes();
	}

	public DefaultSaveFileDialog(IStorageProvider storageProvider)
	{
		_storageProvider = storageProvider;
	}

	public override async Task<string?> ShowDialog(string imageFileName, string imageFolderPath)
	{
		var imageStorageFolder = await GetStorageFolderFromPath(imageFolderPath);

		var imageFileSaveOptions = new FilePickerSaveOptions
		{
			FileTypeChoices = SupportedFileTypes,
			ShowOverwritePrompt = true,
			SuggestedFileName = imageFileName,
			SuggestedStartLocation = imageStorageFolder,
			Title = SaveFileDialogTitle
		};

		var imageToSaveStorageFile = await _storageProvider.SaveFilePickerAsync(imageFileSaveOptions);

		var imageToSaveFilePath = imageToSaveStorageFile?.Path.LocalPath;
		return imageToSaveFilePath;
	}

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
