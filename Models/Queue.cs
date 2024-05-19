namespace ExecutesPlugin.Models;

// FIFO Queue
public class ExecutesQueue<T> where T : class
{
    private readonly List<T> _queue = new();

    public ExecutesQueue() {}

    public void Enqueue(T item)
    {
        _queue.Add(item);
    }

    public void EnqueuePriority(T item)
    {
        _queue.Insert(0, item);
    }

    public T GetNext()
    {
        return _queue.First();
    }

    public bool Drop(T item)
    {
        if (_queue.Contains(item))
        {
            _queue.Remove(item);
            return true;
        }

        return false;
    }

    public bool IsEmpty()
    {
        return _queue.Count == 0;
    }
}