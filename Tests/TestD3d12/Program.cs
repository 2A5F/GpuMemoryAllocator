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

var window_options = WindowOptions.Default;
window_options.IsVisible = false;
window_options.Title = "Test D3d12";
window_options.Size = new Vector2D<int>(960, 540);
window_options.API = GraphicsAPI.None;
var window = Window.Create(window_options);
window.Load += OnLoad;
window.Update += OnUpdate;
window.Render += OnRender;

try
{
    window.Run();
}
finally
{
    swap_chain?.Dispose();
    ctx?.Dispose();
}

return;

void OnLoad()
{
    window.Center();
    input = window.CreateInput();

    Dxgi = DXGI.GetApi(window);
    ctx = new GraphicsContext(Dxgi, D3d12, true);
    swap_chain = new HwndSwapChain(ctx, window.Native!.Win32!.Value.Hwnd, new uint2((uint)window.Size.X, (uint)window.Size.Y));
    swap_chain.VSync = true;

    app = new App(window, ctx, swap_chain);
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
