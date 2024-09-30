namespace ImageFanReloaded.Core.Settings;

public enum FileSystemEntryInfoOrdering
{
    NameAscending = 0,
    LastModificationTimeDescending = 1
}

public static class FileSystemEntryInfoOrderingExtensions
{
    public static string GetDescription(this FileSystemEntryInfoOrdering fileSystemEntryInfoOrdering)
    {
        var description = fileSystemEntryInfoOrdering switch
        {
            FileSystemEntryInfoOrdering.LastModificationTimeDescending => "Last modification time descending",

            _ => "Name ascending"
        };

        return description;
    }
}
