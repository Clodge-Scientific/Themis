namespace Themis.Index;

public class InfiniteGrid<T>
{
    public double CellSize { get; } = double.NaN;

    private readonly Dictionary<long, Dictionary<long, T>> _map = new();

    public InfiniteGrid(double cellSize)
        => CellSize = cellSize;

    public InfiniteGrid<T> Add(T value, long xIdx, long yIdx)
    {
        if (!Contains(xIdx)) _map.Add(xIdx, new Dictionary<long, T>());

        if (!Contains(xIdx, yIdx))
            _map[xIdx].Add(yIdx, value);
        else
            _map[xIdx][yIdx] = value;

        return this;
    }

    public InfiniteGrid<T> Add(T value, double x, double y)
    {
        long xIdx = x.ToGridIndex(CellSize);
        long yIdx = y.ToGridIndex(CellSize);

        return Add(value, xIdx, yIdx);
    }

    public InfiniteGrid<T> Add(T value, IEnumerable<double> pos)
    {
        if (pos.Count() < 2) throw new ArgumentException($"Position must be at least 2D", nameof(pos));

        return Add(value, pos.ElementAt(0), pos.ElementAt(1));
    }

    public InfiniteGrid<T> Remove(long xIdx, long yIdx)
    {
        if (!Contains(xIdx, yIdx)) return this;

        //< Remove the item at the cell [xidx,yidx]
        _map[xIdx].Remove(yIdx);
        //< If we only have one value for the x-index, remove it as well
        if (_map[xIdx].Values.Count == 1) _map.Remove(xIdx);

        return this;
    }

    public InfiniteGrid<T> Remove(double x, double y)
    {
        long xIdx = x.ToGridIndex(CellSize);
        long yIdx = y.ToGridIndex(CellSize);

        return Remove(xIdx, yIdx);
    }

    public T Get(double x, double y)
    {
        long xIdx = x.ToGridIndex(CellSize);
        long yIdx = y.ToGridIndex(CellSize);

        return Get(xIdx, yIdx);
    }

    public T Get(long xIdx, long yIdx)
    {
        if (!Contains(xIdx, yIdx)) throw new IndexOutOfRangeException($"InfiniteGrid does not contain item for indices: [{xIdx},{yIdx}]");

        return _map[xIdx][yIdx];
    }

    public bool Contains(long xIdx)
        => _map.ContainsKey(xIdx);

    public bool Contains(long xIdx, long yIdx)
    {
        if (!_map.ContainsKey(xIdx)) return false;

        return _map[xIdx].ContainsKey(yIdx);
    }

    public bool Contains(double x)
    {
        long idx = x.ToGridIndex(CellSize);
        return Contains(idx);
    }

    public bool Contains(double x, double y)
    {
        long xIdx = x.ToGridIndex(CellSize);
        long yIdx = y.ToGridIndex(CellSize);
        return Contains(xIdx, yIdx);
    }
}

public static class InfiniteGridExtensions
{
    public static long ToGridIndex(this double val, double cellSize)
        => (long)Math.Floor(val / cellSize);

    public static double ToProjectedDimension(this double index, double cellSize)
        => index * cellSize;
}
