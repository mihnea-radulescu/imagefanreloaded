using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Synchronization;

public interface IFolderChangedEventHandle
{
	Task WaitOne();

	Task Set();
}
