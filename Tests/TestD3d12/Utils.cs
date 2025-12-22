using System.Diagnostics;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D12;

namespace TestD3d12;

public static class Utils
{
    #region HResult

    public static HResult AsHResult(this int result) => result;

    [StackTraceHidden]
    public static void TryThrow(this HResult result)
    {
        if (!result.IsSuccess) result.Throw();
    }

    [StackTraceHidden]
    public static void TryThrowHResult(this int result) => result.AsHResult().TryThrow();

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

    extension<T>(InlineArray3<T> array) where T : IDisposable
    {
        public void Dispose()
        {
            foreach (var item in array) item.Dispose();
        }
    }

    #endregion
    
    #region ID3D12Fence Wait

    public static unsafe void Wait(this ComPtr<ID3D12Fence> fence, ulong value, EventWaitHandle handle)
    {
        if (fence.GetCompletedValue() >= value) return;
        fence.SetEventOnCompletion(value, (void*)handle.SafeWaitHandle.DangerousGetHandle()).TryThrowHResult();
        handle.WaitOne();
    }

    public static unsafe ValueTask WaitAsync(this ComPtr<ID3D12Fence> fence, ulong value, EventWaitHandle handle)
    {
        if (fence.GetCompletedValue() >= value) return ValueTask.CompletedTask;
        fence.SetEventOnCompletion(value, (void*)handle.SafeWaitHandle.DangerousGetHandle()).TryThrowHResult();
        var tcs = new TaskCompletionSource();
        ThreadPool.RegisterWaitForSingleObject(handle, static (tcs, _) => { ((TaskCompletionSource)tcs!).SetResult(); }, tcs, TimeSpan.MaxValue, true);
        return new(tcs.Task);
    }

    #endregion
}
