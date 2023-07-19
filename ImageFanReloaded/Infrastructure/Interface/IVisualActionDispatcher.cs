using System;

namespace ImageFanReloaded.Infrastructure.Interface
{
    public interface IVisualActionDispatcher
    {
        void Invoke(Action callback);
    }
}
