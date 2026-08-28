using System;
using System.Collections.Generic;
using System.Linq;
using ImageFanReloaded.Core.DiscAccess.DriveInfo;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.DriveInfo;

public abstract class UnixBasedDriveInfoBase : IDriveInfo
{
	protected UnixBasedDriveInfoBase(IGlobalParameters globalParameters)
	{
		_nameComparison = globalParameters.NameComparer.ToStringComparison();
	}

	public bool IsSupportedDrive(string driveName)
	{
		if (driveName.Equals(RootPath, _nameComparison))
		{
			return true;
		}

		var isSupportedDrive = SupportedDrivePrefixes.Any(
			aSupportedDrivePrefix => driveName.StartsWith(
				aSupportedDrivePrefix, _nameComparison));

		return isSupportedDrive;
	}

	protected abstract IReadOnlyList<string> SupportedDrivePrefixes { get; }

	private const string RootPath = "/";

	private readonly StringComparison _nameComparison;
}
