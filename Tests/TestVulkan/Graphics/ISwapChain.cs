using Coplt.Mathematics;

namespace TestVulkan;

public interface ISwapChain
{
    public uint2 Size { get; }
    
    public void OnResize(uint2 size);
    public void Present();
    public void PresentNoWait();
    public void WaitFrameReady();
}
