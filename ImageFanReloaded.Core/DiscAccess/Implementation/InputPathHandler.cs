using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.DiscAccess.Implementation.Extensions;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class InputPathHandler : IInputPathHandler
{
	public InputPathHandler(
		IGlobalParameters globalParameters,
		ITabOptions tabOptions,
		string? inputPath)
	{
		_globalParameters = globalParameters;
		_tabOptions = tabOptions;

		InputPathType = InputPathType.NotSet;

		if (!string.IsNullOrEmpty(inputPath) && Path.Exists(inputPath))
		{
			if (Directory.Exists(inputPath))
			{
				InputPathType = InputPathType.Folder;

				FolderPath = Path.GetFullPath(inputPath);
			}
			else if (File.Exists(inputPath))
			{
				try
				{
					var inputPathFileInfo = new FileInfo(inputPath);

					if (IsImageFile(inputPathFileInfo))
					{
						InputPathType = InputPathType.ImageFile;

						FolderPath = inputPathFileInfo.DirectoryName;
						FilePath = inputPathFileInfo.FullName;
					}
					else if (IsZipArchive(inputPathFileInfo))
					{
						InputPathType = InputPathType.ZipArchive;

						FolderPath = inputPathFileInfo.FullName;
					}
				}
				catch
				{
				}
			}
		}
	}

	public InputPathType InputPathType { get; }

	public string? FolderPath { get; }
	public string? FilePath { get; }

	public bool CanHandleInputPath() => InputPathType != InputPathType.NotSet;

	public async Task<IFileSystemEntryInfo?> GetMatchingFileSystemEntryInfo(
		IReadOnlyList<IFileSystemEntryInfo> folders)
			=> await Task.Run(() =>
				{
					var nameComparison =
						_globalParameters.NameComparer.ToStringComparison();

					var matchingFileSystemEntryInfo = folders
						.Where(aFolder => FolderPath!.StartsWithPath(
							aFolder.Path, DirectorySeparator, nameComparison))
						.Select(aFolder => new
						{
							Folder = aFolder,
							aFolder.Path.Length
						})
						.OrderByDescending(aFolderWithLength =>
							aFolderWithLength.Length)
						.Select(aFolderWithLength => aFolderWithLength.Folder)
						.FirstOrDefault();

					return matchingFileSystemEntryInfo;
				});

	private const string ZipArchiveExtension = ".zip";

	private static readonly string DirectorySeparator =
		Path.DirectorySeparatorChar.ToString();

	private readonly IGlobalParameters _globalParameters;
	private readonly ITabOptions _tabOptions;

	private bool IsImageFile(FileInfo inputPathFileInfo)
		=> _globalParameters.ImageFileExtensions.Contains(
			inputPathFileInfo.Extension);

	private bool IsZipArchive(FileInfo inputPathFileInfo)
		=> _tabOptions.ZipArchivesEnabled &&
		   string.Equals(
			   inputPathFileInfo.Extension,
			   ZipArchiveExtension,
			   _globalParameters.FileExtensionComparison);
}
