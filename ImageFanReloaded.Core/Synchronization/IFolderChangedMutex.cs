using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Synchronization;

public interface IFolderChangedMutex
{
	Task Wait();

	void Signal();
}
