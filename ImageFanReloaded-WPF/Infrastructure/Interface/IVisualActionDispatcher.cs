using System;

namespace ImageFanReloadedWPF.Infrastructure.Interface
{
    public interface IVisualActionDispatcher
    {
        void Invoke(Action callback);
    }
}
