namespace TestD3d12;

public interface IGpuRecyclable
{
    public int CurrentFrame { get; }
    public void Recycle();
}

public abstract class AGpuRecyclable<T>(GpuRecyclablePool<T> Pool) : IGpuRecyclable
    where T : AGpuRecyclable<T>
{
    public GpuRecyclablePool<T> Pool { get; } = Pool;

    public int CurrentFrame { get; internal set; }
    protected abstract T Recycle();

    void IGpuRecyclable.Recycle() => Pool.Recycle(Recycle());
}

public class GpuRecyclablePool<T>(IGpuRecyclablePoolSource Source) where T : AGpuRecyclable<T>
{
    public IGpuRecyclablePoolSource Source { get; } = Source;
    internal readonly Queue<T> m_pool_queue = new();
    internal readonly Lock m_lock = new();

    internal void Recycle(T pack)
    {
        using var _ = m_lock.EnterScope();
        m_pool_queue.Enqueue(pack);
    }

    public void Return(T pack)
    {
        pack.CurrentFrame = Source.CurrentFrame;
        Source.RegRecycle(pack);
    }

    public T? Rent() => m_pool_queue.TryDequeue(out var r) ? r : null;
}

public interface IGpuRecyclablePoolSource
{
    public int CurrentFrame { get; }
    public void RegRecycle(IGpuRecyclable item);
}
