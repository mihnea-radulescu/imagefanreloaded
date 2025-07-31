namespace ImageFanReloaded.Core.DiscAccess;

public interface IInputPathHandlerFactory
{
	IInputPathHandler GetInputPathHandler(string? inputPath);
}
