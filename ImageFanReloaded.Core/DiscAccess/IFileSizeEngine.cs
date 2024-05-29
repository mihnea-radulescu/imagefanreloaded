namespace ImageFanReloaded.Core.DiscAccess;

public interface IFileSizeEngine
{
	decimal ConvertToKilobytes(long sizeInBytes);
	
	decimal ConvertToMegabytes(decimal sizeInKilobytes);
}
