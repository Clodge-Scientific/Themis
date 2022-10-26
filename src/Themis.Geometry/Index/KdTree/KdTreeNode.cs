using System.Text;

using Themis.Geometry.Index.KdTree.Interfaces;

namespace Themis.Geometry.Index.KdTree
{
    public class KdTreeNode<TKey, TValue> : IKdTreeNode<TKey, TValue>
    {
        public TKey[]? Point { get; set; }
        public TValue? Value { get; set; } = default;

        public bool IsLeaf => LeftChild == null && RightChild == null;

        internal KdTreeNode<TKey, TValue>? LeftChild = null;
        internal KdTreeNode<TKey, TValue>? RightChild = null;

        public KdTreeNode() { }

        public KdTreeNode(IEnumerable<TKey> point, TValue value)
        {
            Point = point.ToArray();
            Value = value;
        }

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
}
