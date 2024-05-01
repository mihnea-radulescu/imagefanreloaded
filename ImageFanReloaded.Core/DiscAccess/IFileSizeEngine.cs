namespace ImageFanReloaded.Core.DiscAccess;

public interface IFileSizeEngine
{
	decimal ConvertToKilobytes(long sizeInBytes);
	
	int ConvertToMegabytes(decimal sizeInKilobytes);
}
