// ReSharper disable AccessToDisposedClosure

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using TestVulkan;

Utils.InitLogger();

IInputContext input = null!;

var Vk = Silk.NET.Vulkan.Vk.GetApi()!;
GraphicsContext ctx = null!;
SwapChain swap_chain = null!;

ulong frame_count = 0;

App app = null!;

var window = Window.Create(WindowOptions.DefaultVulkan with
{
    IsVisible = false,
    Title = "Test Vulkan",
    Size = new(960, 540),
    API = new(ContextAPI.Vulkan, new(1, 3)),
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
    swap_chain?.Dispose();
    ctx?.Dispose();
}

return;

void OnLoad()
{
    window.Center();
    input = window.CreateInput();
    if (window.VkSurface is null) throw new NotSupportedException("Platform not support vulkan");
    ctx = new(Vk, window.VkSurface, true);
    swap_chain = new(ctx, window, new((uint)window.Size.X, (uint)window.Size.Y));

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
