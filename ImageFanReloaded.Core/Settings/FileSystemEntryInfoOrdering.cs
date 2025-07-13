using System;

namespace ImageFanReloaded.Core.Settings;

public enum FileSystemEntryInfoOrdering
{
	Name = 0,
	ModificationTime = 1
}

public static class FileSystemEntryInfoOrderingExtensions
{
	public static string GetDescription(this FileSystemEntryInfoOrdering fileSystemEntryInfoOrdering)
	{
		var description = fileSystemEntryInfoOrdering switch
		{
			FileSystemEntryInfoOrdering.Name => "Name",
			FileSystemEntryInfoOrdering.ModificationTime => "Modification time",

			_ => throw new NotSupportedException(
				$"Enum value {fileSystemEntryInfoOrdering} not supported.")
		};

		return description;
	}
}
