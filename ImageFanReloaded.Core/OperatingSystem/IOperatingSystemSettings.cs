namespace ImageFanReloaded.Core.OperatingSystem;

public interface IOperatingSystemSettings
{
	bool IsWindows { get; }
	bool IsLinux { get; }
	bool IsMacOS { get; }
}
