namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class WindowsDriveInfo : IDriveInfo
{
	public bool IsSupportedDrive(string driveName) => true;
}
