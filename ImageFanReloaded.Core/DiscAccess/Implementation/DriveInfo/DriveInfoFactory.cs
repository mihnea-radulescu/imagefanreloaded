using ImageFanReloaded.Core.DiscAccess.DriveInfo;
using ImageFanReloaded.Core.Exceptions;
using ImageFanReloaded.Core.RuntimeEnvironment;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation.DriveInfo;

public class DriveInfoFactory : IDriveInfoFactory
{
	public DriveInfoFactory(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}

	public IDriveInfo GetDriveInfo()
	{
		if (_globalParameters.RuntimeEnvironmentType is
			RuntimeEnvironmentType.Linux or RuntimeEnvironmentType.LinuxFlatpak)
		{
			return new LinuxDriveInfo(_globalParameters);
		}

		if (_globalParameters.RuntimeEnvironmentType ==
			RuntimeEnvironmentType.Windows)
		{
			return new WindowsDriveInfo();
		}

		if (_globalParameters.RuntimeEnvironmentType ==
			RuntimeEnvironmentType.OtherUnix)
		{
			return new OtherUnixDriveInfo(_globalParameters);
		}

		throw new RuntimeEnvironmentNotSupportedException();
	}

	private readonly IGlobalParameters _globalParameters;
}
