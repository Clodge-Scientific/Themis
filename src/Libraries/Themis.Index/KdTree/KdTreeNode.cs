using System.Text;

namespace Themis.Index.KdTree;

public class KdTreeNode<TKey, TValue>(
    IEnumerable<TKey> point,
    TValue value)
    : IKdTreeNode<TKey, TValue>
{
    public TKey[] Point { get; set; } = [.. point];
    public TValue Value { get; set; } = value;

    public bool IsLeaf => LeftChild == null && RightChild == null;

    internal KdTreeNode<TKey, TValue>? LeftChild = null;
    internal KdTreeNode<TKey, TValue>? RightChild = null;

    internal KdTreeNode<TKey, TValue>? this[int compare]
    {
        get { return compare <= 0 ? LeftChild : RightChild; }
        set
        {
            if (compare <= 0) { LeftChild = value; }
            else { RightChild = value; }
        }
    }

    public override string ToString()
    {
        if (Point == null) return "NULL";

        var sb = new StringBuilder();

        foreach (int dim in Enumerable.Range(0, Point.Length)) { sb.Append($"{Point[dim]}\t"); }

        _ = (Value == null) ? sb.Append("NULL") : sb.Append(Value.ToString());

        return sb.ToString();
    }
}