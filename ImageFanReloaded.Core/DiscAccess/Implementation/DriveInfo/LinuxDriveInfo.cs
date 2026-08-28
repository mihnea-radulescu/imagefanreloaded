using System.Collections.Generic;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.DriveInfo;

public class LinuxDriveInfo : UnixBasedDriveInfoBase
{
	public LinuxDriveInfo(IGlobalParameters globalParameters)
		: base(globalParameters)
	{
		_supportedDrivePrefixes = ["/media/", "/mnt/", "/run/media/"];
	}

	protected override IReadOnlyList<string> SupportedDrivePrefixes
		=> _supportedDrivePrefixes;

	private readonly IReadOnlyList<string> _supportedDrivePrefixes;
}
