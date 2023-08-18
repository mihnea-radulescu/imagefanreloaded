using System;

namespace ImageFanReloaded.Infrastructure
{
    public interface IVisualActionDispatcher
    {
        void Invoke(Action callback);
    }
}
