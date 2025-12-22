using Silk.NET.Direct3D12;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace TestD3d12;

public unsafe class App(IWindow window, GraphicsContext ctx, HwndSwapChain swap_chain)
{
    public void OnUpdate(double delta_time) { }

    public void OnRender(double delta_time)
    {
        ctx.CommandList.Barrier([
            new TextureBarrier(
                BarrierSync.None, BarrierSync.RenderTarget,
                BarrierAccess.NoAccess, BarrierAccess.RenderTarget,
                BarrierLayout.Common, BarrierLayout.RenderTarget,
                swap_chain.Ptr)
        ]);
        ctx.CommandList.ClearRenderTargetView(swap_chain.CurrentRtv, 1);
        ctx.CommandList.Barrier([
            new TextureBarrier(
                BarrierSync.RenderTarget, BarrierSync.None,
                BarrierAccess.RenderTarget, BarrierAccess.NoAccess,
                BarrierLayout.RenderTarget, BarrierLayout.Common,
                swap_chain.Ptr)
        ]);
    }

    public void OnKeyDown(IKeyboard keyboard, Key key, int key_code) { }
}
