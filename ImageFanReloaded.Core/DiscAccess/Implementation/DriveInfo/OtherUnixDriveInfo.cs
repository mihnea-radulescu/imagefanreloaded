using System.Collections.Generic;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.DriveInfo;

public class OtherUnixDriveInfo : UnixBasedDriveInfoBase
{
	public OtherUnixDriveInfo(IGlobalParameters globalParameters)
		: base(globalParameters)
	{
		_supportedDrivePrefixes = ["/media/", "/mnt/", "/Volumes/"];
	}

	protected override IReadOnlyList<string> SupportedDrivePrefixes
		=> _supportedDrivePrefixes;

	private readonly IReadOnlyList<string> _supportedDrivePrefixes;
}
