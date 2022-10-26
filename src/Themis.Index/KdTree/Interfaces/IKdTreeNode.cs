namespace Themis.Index.KdTree;

public interface IKdTreeNode<TKey, TValue>
{
    TKey[]? Point { get; }
    TValue? Value { get; }

    bool IsLeaf { get; }
}