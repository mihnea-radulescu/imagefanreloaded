namespace ImageFanReloaded.Core.OperatingSystem.Implementation;

public class OperatingSystemSettings : IOperatingSystemSettings
{
	public OperatingSystemSettings()
	{
		IsLinux = System.OperatingSystem.IsLinux();
		IsWindows = System.OperatingSystem.IsWindows();
		IsMacOS = System.OperatingSystem.IsMacOS();
	}

	public bool IsLinux { get; }
	public bool IsWindows { get; }
	public bool IsMacOS { get; }
}
