namespace Themis.Geometry.Index.KdTree.Interfaces
{
    public interface IKdTreeNode<TKey, TValue>
    {
        TKey[]? Point { get; }
        TValue? Value { get; }

        bool IsLeaf { get; }
    }
}
