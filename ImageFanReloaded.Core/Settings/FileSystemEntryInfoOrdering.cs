using System;

namespace ImageFanReloaded.Core.Settings;

public enum FileSystemEntryInfoOrdering
{
	Name = 0,
	LastModificationTime = 1,
	RandomShuffle = 2
}

public static class FileSystemEntryInfoOrderingExtensions
{
	extension(FileSystemEntryInfoOrdering fileSystemEntryInfoOrdering)
	{
		public string Description
		{
			get
			{
				var description = fileSystemEntryInfoOrdering switch
				{
					FileSystemEntryInfoOrdering.Name => "Name",
					FileSystemEntryInfoOrdering.LastModificationTime
						=> "Last modification time",
					FileSystemEntryInfoOrdering.RandomShuffle
						=> "Random shuffle",

					_ => throw new NotSupportedException(
						$"Enum value {fileSystemEntryInfoOrdering} not supported.")
				};

				return description;
			}
		}
	}
}
