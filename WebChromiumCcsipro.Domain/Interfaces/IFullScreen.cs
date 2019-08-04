using System.Runtime.CompilerServices;

namespace WebChromiumCcsipro.Domain.Interfaces
{
    public interface IFullScreen
    {
        bool FullScreenMode { get; }
        void FullScreenEnable();
        void FUllScreenDisable();
    }
}