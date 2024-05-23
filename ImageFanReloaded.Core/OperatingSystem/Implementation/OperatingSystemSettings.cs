namespace ImageFanReloaded.Core.OperatingSystem.Implementation;

public class OperatingSystemSettings : IOperatingSystemSettings
{
	public OperatingSystemSettings()
	{
		_isWindows = System.OperatingSystem.IsWindows();
		_isLinux = System.OperatingSystem.IsLinux();
		_isMacOS = System.OperatingSystem.IsMacOS();
	}

	public bool IsWindows => _isWindows;
	public bool IsLinux => _isLinux;
	public bool IsMacOS => _isMacOS;
	
	#region Private
	
	private readonly bool _isWindows;
	private readonly bool _isLinux;
	private readonly bool _isMacOS;
	
	#endregion
}
