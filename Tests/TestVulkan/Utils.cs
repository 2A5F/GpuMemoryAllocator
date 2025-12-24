using System.Diagnostics;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace TestVulkan;

public static unsafe class Utils
{
    #region Chain

    public static BaseInStructure* ChainStart<T>(T* cur) where T: unmanaged, IChainable => (BaseInStructure*)cur;
    public static void Chain<T>(ref BaseInStructure* p_cur, T* cur) where T: unmanaged, IChainable, IStructuredType
    {
        var c = (BaseInStructure*)cur;
        p_cur->PNext = c;
        p_cur = c;
        p_cur->SType = cur->StructureType();
    }

    #endregion

    #region Literal

    public static byte* Lit(ReadOnlySpan<byte> str) => (byte*)Unsafe.AsPointer(in str.GetPinnableReference());

    #endregion

    #region HResult

    [StackTraceHidden]
    public static void TryThrow(this Result result)
    {
        if (result is not (Result.Success or Result.SuboptimalKhr)) throw new($"{result}");
    }

    #endregion

    #region InitLogger

    internal static void InitLogger()
    {
        if (File.Exists("./logs/latest.log"))
        {
            try
            {
                var time = File.GetCreationTime("./logs/latest.log");
                var time_name = $"{time:yyyy-MM-dd}";
                var max_count = Directory.GetFiles("./logs/")
                    .Where(static n => Path.GetExtension(n) == ".log")
                    .Select(static n => Path.GetFileName(n))
                    .Where(n => n.StartsWith(time_name))
                    .Select(n => n.Substring(time_name.Length))
                    .Select(static n => (n, i: n.IndexOf('.')))
                    .Where(static a => a.i > 1)
                    .Select(static a => (s: uint.TryParse(a.n.Substring(1, a.i - 1), out var n), n))
                    .Where(static a => a.s)
                    .OrderByDescending(static a => a.n)
                    .Select(static a => a.n)
                    .FirstOrDefault();
                var count = max_count + 1;
                File.Move("./logs/latest.log", $"./logs/{time_name}_{count}.log");
            }
            catch (Exception e)
            {
                Log.Error(e, "");
            }
        }
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .Enrich.WithExceptionDetails()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Async(c => c.File("./logs/latest.log"))
            .CreateLogger();
    }

    #endregion

    #region Dispose for InlineArray

    extension<T>(InlineArray3<T> array) where T: IDisposable
    {
        public void Dispose()
        {
            foreach (var item in array) item.Dispose();
        }
    }

    #endregion

    #region DecodeApiVersion

    public static string DecodeVersion(uint version)
    {
        var variant = version >> 29;
        var major = (version >> 22) & 0x7F;
        var minor = (version >> 12) & 0x3FF;
        var patch = version & 0xFFF;
        return $"{major}.{minor}.{patch}.{variant}";
    }
    public static (uint, uint, uint) SplitVersion(uint version)
    {
        var major = (version >> 22) & 0x7F;
        var minor = (version >> 12) & 0x3FF;
        var patch = version & 0xFFF;
        return (major, minor, patch);
    }

    #endregion

    #region FenceWait

    public static void TimelineSingle(GraphicsContext ctx, Semaphore fence, ulong value)
    {
        SemaphoreSignalInfo info = new()
        {
            SType = StructureType.SemaphoreSignalInfo,
            Semaphore = fence,
            Value = value,
        };
        ctx.Vk.SignalSemaphore(ctx.Device, &info).TryThrow();
    }

    public static void TimelineWait(GraphicsContext ctx, Semaphore fence, ulong value)
    {
        ulong cur_value;
        ctx.Vk.GetSemaphoreCounterValue(ctx.Device, fence, &cur_value).TryThrow();
        if (cur_value >= value) return;
        SemaphoreWaitInfo info = new()
        {
            SType = StructureType.SemaphoreWaitInfo,
            SemaphoreCount = 1,
            PSemaphores = &fence,
            PValues = &value,
        };
        ctx.Vk.WaitSemaphores(ctx.Device, &info, ulong.MaxValue).TryThrow();
    }

    #endregion
}
