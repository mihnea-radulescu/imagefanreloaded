using ImageFanReloaded.Core.DiscAccess.DriveInfo;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.DriveInfo;

public class WindowsDriveInfo : IDriveInfo
{
	public bool IsSupportedDrive(string driveName) => true;
}
