using Coplt.Mathematics;
using Silk.NET.Direct3D12;
using Silk.NET.DXGI;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using TestD3d12;

// ReSharper disable AccessToDisposedClosure

Utils.InitLogger();

IInputContext input = null!;

DXGI Dxgi = null!;
var D3d12 = D3D12.GetApi()!;
GraphicsContext ctx = null!;
HwndSwapChain swap_chain = null!;

ulong frame_count = 0;

App app = null!;

var window = Window.Create(WindowOptions.Default with
{
    IsVisible = false,
    Title = "Test D3d12",
    Size = new(960, 540),
    API = GraphicsAPI.None,
});
window.Load += OnLoad;
window.Update += OnUpdate;
window.Render += OnRender;
window.Resize += OnResize;

try
{
    window.Run();
}
finally
{
    app?.Dispose();
    swap_chain?.Dispose();
    ctx?.Dispose();
}

return;

void OnLoad()
{
    window.Center();
    input = window.CreateInput();

    Dxgi = DXGI.GetApi(window);
    ctx = new(Dxgi, D3d12, true);
    swap_chain = new(ctx, window.Native!.Win32!.Value.Hwnd, new((uint)window.Size.X, (uint)window.Size.Y));
    swap_chain.VSync = true;

    app = new(window, ctx, swap_chain);
    foreach (var t in input.Keyboards) t.KeyDown += app.OnKeyDown;

    app.OnLoad();
}

void OnUpdate(double delta_time)
{
    frame_count++;
    if (frame_count > 1)
    {
        swap_chain.WaitFrameReady();
        ctx.ReadyNextFrameNoWait();
    }

    app.OnUpdate(delta_time);
}

void OnRender(double delta_time)
{
    ctx.CommandList.FrameStart();

    app.OnRender(delta_time);

    ctx.CommandList.FrameEnd();

    ctx.SubmitNotEnd();
    swap_chain.PresentNoWait();

    if (frame_count == 1)
    {
        window.IsVisible = true;
    }
}

void OnResize(Vector2D<int> size)
{
    swap_chain.OnResize(new((uint)size.X, (uint)size.Y));
}
