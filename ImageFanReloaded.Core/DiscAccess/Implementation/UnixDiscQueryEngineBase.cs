using System;
using System.Collections.Generic;
using System.Linq;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public abstract class UnixDiscQueryEngineBase : DiscQueryEngineBase
{
	#region Protected
	
	protected UnixDiscQueryEngineBase(
		IGlobalParameters globalParameters,
		IFileSizeEngine fileSizeEngine,
		IImageFileFactory imageFileFactory)
		: base(globalParameters, fileSizeEngine, imageFileFactory)
	{
		_nameComparison = globalParameters.NameComparer.ToStringComparison();
	}

	protected override bool IsSupportedDrive(string driveName)
	{
		if (driveName.Equals(RootPath, _nameComparison))
		{
			return true;
		}

		var isAllowedDrivePrefix = AllowedDrivePrefixes.Any(
			aSupportedDrivePrefix => driveName.StartsWith(aSupportedDrivePrefix, _nameComparison));

		var isDisallowedDriveFragment = DisallowedDriveFragments.Any(
			aDisallowedDriveFragment => driveName.Contains(aDisallowedDriveFragment, _nameComparison));

		var isSupportedDrive = isAllowedDrivePrefix && !isDisallowedDriveFragment;

		return isSupportedDrive;
	}

	protected abstract IReadOnlyList<string> AllowedDrivePrefixes { get; }
	protected abstract IReadOnlyList<string> DisallowedDriveFragments { get; }
	
	#endregion

	#region Private

	private const string RootPath = "/";
	
	private readonly StringComparison _nameComparison;

	#endregion
}
