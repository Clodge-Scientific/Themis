using System.Collections;

using Themis.Index.KdTree.TypeMath;

namespace Themis.Index.KdTree;

public class KdTree<TKey, TValue> : IKdTree<TKey, TValue>
{
    const int InitialDimension = -1;

    public int Count { get; private set; }
    public int Dimensions { get; }

    private KdTreeNode<TKey, TValue>? root = null;

    public ITypeMath<TKey> TypeMath { get; }
    public AddDuplicateBehavior AddDuplicateBehavior { get; } = AddDuplicateBehavior.Error;

    public KdTree(int dimensions, ITypeMath<TKey> typeMath)
    {
        this.Dimensions = dimensions;
        this.TypeMath = typeMath;
        this.Count = 0;
    }

    public KdTree(int dimensions, ITypeMath<TKey> typeMath, AddDuplicateBehavior addDuplicateBehavior)
        : this(dimensions, typeMath)
    {
        this.AddDuplicateBehavior = addDuplicateBehavior;
    }

    #region IKdTree<TKey, TValue> Add, Remove & Clear Methods
    public bool Add(IEnumerable<TKey> point, TValue value)
    {
        if (point == null) throw new ArgumentNullException(nameof(point));
        if (value == null) throw new ArgumentNullException(nameof(value));

        if (root == null) return GenerateNewRoot(point, value);

        var nodeToAdd = new KdTreeNode<TKey, TValue>(point, value);
        int dimension = InitialDimension;
        KdTreeNode<TKey, TValue> parent = root;

        while (true)
        {
            //< Increment the dimensional index we're currently searching in
            dimension = (dimension + 1) % Dimensions;

            //< Check if the node we're adding has the same HyperPoint as this node
            if (TypeMath.AreEqual(point, parent.Point))
            {
                return HandleDuplicate(parent, value);
            }

            //< Find which side this node sits relative to its parent at this dimensional level?
            int compare = TypeMath.Compare(point.ElementAt(dimension), parent.Point[dimension]);

            if (parent[compare] == null)
            {
                parent[compare] = nodeToAdd;
                break;
            }
            else
            {
                parent = parent[compare];
            }
        }

        //< New node was added, iterate count and return
        Count++;
        return true;
    }

    bool GenerateNewRoot(IEnumerable<TKey> point, TValue value)
    {
        root = new KdTreeNode<TKey, TValue>(point, value);
        Count++;
        return true;
    }

    bool HandleDuplicate(KdTreeNode<TKey, TValue> parent, TValue value)
    {
        switch (AddDuplicateBehavior)
        {
            case AddDuplicateBehavior.Skip:
                return false;
            case AddDuplicateBehavior.Update:
                parent.Value = value;
                return true;
            case AddDuplicateBehavior.Error:
                throw new DuplicateNodeError();
            default:
                throw new Exception("Unexpected AddDuplicateBehaviour - how did you do this?");
        }
    }

    public void Remove(IEnumerable<TKey> point)
    {
        if (root == null) return; //< Bail if tree is empty
        if (root.Point == null) throw new ArgumentNullException(nameof(root.Point), "Root KdTreeNode cannot have a null HyperPoint!");

        KdTreeNode<TKey, TValue> node;

        //< Handle removing root node
        if (TypeMath.AreEqual(point, root.Point))
        {
            node = root;
            root = null;
            Count--;
            ReAddChildNodes(node);
            return;
        }

        node = root;
        int dimension = InitialDimension;

        while (node != null)
        {
            dimension = (dimension + 1) % Dimensions;
            int compare = TypeMath.Compare(point.ElementAt(dimension), node.Point[dimension]);

            if (node[compare] == null) return; //< Can't find node - bail

            if (TypeMath.AreEqual(point, node[compare].Point))
            {
                var nodeToRemove = node[compare];
                node[compare] = null;
                Count--;
                ReAddChildNodes(nodeToRemove);
            }
            else
                node = node[compare];
        }
    }

    void ReAddChildNodes(KdTreeNode<TKey, TValue> removedNode)
    {
        if (removedNode.IsLeaf) return;

        //< The use of two Queues seems redundant - but we do it to attempt to maintain the order the nodes were originally added
        var nodesToAdd = new Queue<KdTreeNode<TKey, TValue>>();
        var nodesToAddQ = new Queue<KdTreeNode<TKey, TValue>>();

        if (removedNode.LeftChild != null) nodesToAddQ.Enqueue(removedNode.LeftChild);
        if (removedNode.RightChild != null) nodesToAddQ.Enqueue(removedNode.RightChild);

        while (nodesToAddQ.Count > 0)
        {
            var nodeToAdd = nodesToAddQ.Dequeue();
            nodesToAdd.Enqueue(nodeToAdd);

            for (int side = -1; side <= 1; side += 2)
            {
                if (nodeToAdd[side] != null)
                {
                    nodesToAddQ.Enqueue(nodeToAdd[side]);
                    nodeToAdd[side] = null;
                }
            }
        }

        while (nodesToAdd.Count > 0)
        {
            var nodeToAdd = nodesToAdd.Dequeue();
            Count--;

            if (nodeToAdd.Point == null || nodeToAdd.Value == null) throw new NullReferenceException("Cannot re-add node with null Point or Value!");
            Add(nodeToAdd.Point, nodeToAdd.Value);
        }
    }

    public void Clear()
    {
        if (root != null)
        {
            RemoveChildNodes(root);
            Count = 0;
        }
    }

    void RemoveChildNodes(KdTreeNode<TKey, TValue> node)
    {
        for (var side = -1; side <= 1; side += 2)
        {
            if (node[side] != null)
            {
                RemoveChildNodes(node[side]);
                node[side] = null;
            }
        }
    }
    #endregion

    #region IKdTree<TKey, TValue> Balancing Methods
    public void Balance()
    {
        if (root == null) return;

        //< Populate a list of all currently stored KdTreeNodes
        var nodes = new List<KdTreeNode<TKey, TValue>>();
        AddNodesToList(root, nodes);

        //< Clear the current tree structure
        Clear();

        //< Re-add all nodes in a balanced state
        AddNodesBalanced(nodes.ToArray(), 0, 0, nodes.Count - 1);
    }

    void AddNodesToList(KdTreeNode<TKey, TValue> node, List<KdTreeNode<TKey, TValue>> nodes)
    {
        if (node == null) return;

        nodes.Add(node);
        for (var side = -1; side <= 1; side += 2)
        {
            if (node[side] != null)
            {
                AddNodesToList(node[side], nodes);
                node[side] = null;
            }
        }
    }

    void SortNodesArray(KdTreeNode<TKey, TValue>[] nodes, int byDimension, int fromIndex, int toIndex)
    {
        for (var index = fromIndex + 1; index <= toIndex; index++)
        {
            var newIndex = index;

            while (true)
            {
                var a = nodes[newIndex - 1];
                var b = nodes[newIndex];
                if (TypeMath.Compare(b.Point[byDimension], a.Point[byDimension]) < 0)
                {
                    nodes[newIndex - 1] = b;
                    nodes[newIndex] = a;
                }
                else
                    break;
            }
        }
    }

    void AddNodesBalanced(KdTreeNode<TKey, TValue>[] nodes, int byDimension, int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex)
        {
            Add(nodes[fromIndex].Point, nodes[fromIndex].Value);
            nodes[fromIndex] = null;
            return;
        }

        //< Sort the incoming nodes between [fromIndex, toIndex]
        SortNodesArray(nodes, byDimension, fromIndex, toIndex);
        //< Calculate the mid-point split index
        int midIndex = fromIndex + (int)Math.Round((toIndex - fromIndex + 1) / 2.0) - 1;

        //< Add the mid-point first
        Add(nodes[midIndex].Point, nodes[midIndex].Value);
        nodes[midIndex] = null;

        //< Calculate the next dimensional index
        int nextDimension = (byDimension + 1) % Dimensions;

        //< Recurse on either side of the mid-point
        if (fromIndex < midIndex) AddNodesBalanced(nodes, nextDimension, fromIndex, midIndex - 1);
        if (toIndex > midIndex) AddNodesBalanced(nodes, nextDimension, midIndex + 1, toIndex);
    }
    #endregion

    #region IKdTree<TKey, TValue> Find & TryFind Methods
    public bool TryFindValue(TValue value, out IEnumerable<TKey> point)
    {
        if (root == null)
        {
            point = null;
            return false;
        }

        //< Want to use a FIFO collection of nodes to search
        var nodesToSearch = new Queue<KdTreeNode<TKey, TValue>>();
        nodesToSearch.Enqueue(root);

        while (nodesToSearch.Count > 0)
        {
            var nodeToSearch = nodesToSearch.Dequeue();
            if (nodeToSearch.Value.Equals(value))
            {
                point = nodeToSearch.Point;
                return true;
            }

            for (int side = -1; side <= 1; side += 2)
            {
                var child = nodeToSearch[side];
                if (child != null) nodesToSearch.Enqueue(child);
            }
        }

        point = null;
        return false;
    }

    public bool TryFindValueAt(IEnumerable<TKey> point, out TValue value)
    {
        var parent = root;
        int dimension = InitialDimension;
        while (true)
        {
            if (parent == null)
            {
                value = default;
                return false;
            }
            else if (TypeMath.AreEqual(point, parent.Point))
            {
                value = parent.Value;
                return true;
            }

            //< No hit - keep searching
            dimension = (dimension + 1) % Dimensions;
            int compare = TypeMath.Compare(point.ElementAt(dimension), parent.Point[dimension]);
            parent = parent[compare];
        }
    }

    public IEnumerable<TKey> FindValue(TValue value)
    {
        if (TryFindValue(value, out IEnumerable<TKey> point))
        {
            return point;
        }
        else return Array.Empty<TKey>(); //< Not found, return empty array
    }

    public TValue FindValueAt(IEnumerable<TKey> point)
    {
        if (TryFindValueAt(point, out TValue value))
        {
            return value;
        }
        else return default;
    }
    #endregion

    #region IKdTree<TKey, TValue> Radial & Nearest Neighbour Searches
    public IEnumerable<KdTreeNode<TKey, TValue>> RadialSearch(IEnumerable<TKey> point, TKey radius)
    {
        var nearestNeighs = new NearestNeighbourList<KdTreeNode<TKey, TValue>, TKey>(TypeMath);
        return RadialSearch(point.ToArray(), radius, nearestNeighs);
    }

    public IEnumerable<KdTreeNode<TKey, TValue>> RadialSearch(IEnumerable<TKey> point, TKey radius, int count)
    {
        var nearestNeighs = new NearestNeighbourList<KdTreeNode<TKey, TValue>, TKey>(TypeMath, count);
        return RadialSearch(point.ToArray(), radius, nearestNeighs);
    }

    IEnumerable<KdTreeNode<TKey, TValue>> RadialSearch(TKey[] point, TKey radius,
                                                        NearestNeighbourList<KdTreeNode<TKey, TValue>, TKey> nearestNeighs)
    {
        if (root == null) return Array.Empty<KdTreeNode<TKey, TValue>>();

        AddNearestNeighbours(root, point, InfiniteHyperRect(), 0, nearestNeighs, TypeMath.Multiply(radius, radius));

        return GetNearestNeighbourArray(nearestNeighs);
    }

    HyperRectangle<TKey> InfiniteHyperRect()
    {
        return HyperRectangle<TKey>.Infinite(Dimensions, TypeMath);
    }

    //< TODO :: Add explanation of search pattern / algorithm to wiki and reference it
    void AddNearestNeighbours(KdTreeNode<TKey, TValue> node, TKey[] target, HyperRectangle<TKey> rect, int depth,
                                NearestNeighbourList<KdTreeNode<TKey, TValue>, TKey> nearestNeighs, TKey maxSearchRadiusSquared)
    {
        if (node == null) return;

        int dimension = depth % Dimensions; //< Calculate the current dimension

        //< Split the input HyperRectangle into two sub-rects along the current node's current dimensional value
        var leftRect = rect.Clone();
        leftRect.MaximumPoint[dimension] = node.Point[dimension];
        var rightRect = rect.Clone();
        rightRect.MinimumPoint[dimension] = node.Point[dimension];

        //< Assess which side of the current node the target lies on
        int compare = TypeMath.Compare(target.ElementAt(dimension), node.Point[dimension]);

        var nearestRect = compare <= 0 ? leftRect : rightRect;
        var furthestRect = compare <= 0 ? rightRect : leftRect;

        var nearestNode = compare <= 0 ? node.LeftChild : node.RightChild;
        var furthestNode = compare <= 0 ? node.RightChild : node.LeftChild;

        if (nearestNode != null)
        {
            AddNearestNeighbours(nearestNode, target, nearestRect, depth + 1, nearestNeighs, maxSearchRadiusSquared);
        }

        TKey[] closestPointInFurthestRect = furthestRect.GetClosestPoint(target, TypeMath);
        TKey distanceToTargetSquared = TypeMath.DistanceSquaredBetweenPoints(closestPointInFurthestRect, target);

        if (furthestNode != null && TypeMath.Compare(distanceToTargetSquared, maxSearchRadiusSquared) <= 0)
        {
            //< If we're at capacity, but this next node is closer we need to replace the current further in the NN list
            bool atCapacityButCloserThanFurthest = nearestNeighs.IsAtCapacity && TypeMath.Compare(distanceToTargetSquared, nearestNeighs.GetFurthestDistance()) < 0;
            //< If not at capacity, we're just gonna add all NN anyway
            if (!nearestNeighs.IsAtCapacity || atCapacityButCloserThanFurthest)
            {
                AddNearestNeighbours(furthestNode, target, furthestRect, depth + 1, nearestNeighs, maxSearchRadiusSquared);
            }
        }

        //< If within the search radius, add the current node to the NN list
        distanceToTargetSquared = TypeMath.DistanceSquaredBetweenPoints(node.Point, target);
        if (TypeMath.Compare(distanceToTargetSquared, maxSearchRadiusSquared) <= 0)
        {
            nearestNeighs.Add(node, distanceToTargetSquared);
        }
    }

    static KdTreeNode<TKey, TValue>[] GetNearestNeighbourArray(NearestNeighbourList<KdTreeNode<TKey, TValue>, TKey> nearestNeighs)
    {
        int count = nearestNeighs.Count;
        var neighArray = new KdTreeNode<TKey, TValue>[count];

        foreach (int i in Enumerable.Range(0, count)) { neighArray[count - i - 1] = nearestNeighs.RemoveFurthest(); }

        return neighArray;
    }

    public IEnumerable<KdTreeNode<TKey, TValue>> GetNearestNeighbours(IEnumerable<TKey> point, int count = int.MaxValue)
    {
        if (count < 0) throw new ArgumentException("Number of neighbours cannot be negative!");
        if (count == 0 || root == null) return Array.Empty<KdTreeNode<TKey, TValue>>();
        if (count > Count) count = Count;

        var nearestNeighs = new NearestNeighbourList<KdTreeNode<TKey, TValue>, TKey>(TypeMath, count);
        var rect = InfiniteHyperRect();

        AddNearestNeighbours(root, point.ToArray(), rect, 0, nearestNeighs, TypeMath.MaxValue);

        return GetNearestNeighbourArray(nearestNeighs);
    }
    #endregion

    #region IEnumerable Methods
    public IEnumerator<KdTreeNode<TKey, TValue>> GetEnumerator()
    {
        var left = new Stack<KdTreeNode<TKey, TValue>>();
        var right = new Stack<KdTreeNode<TKey, TValue>>();

        void addChildren(KdTreeNode<TKey, TValue> node)
        {
            if (node.LeftChild != null) left.Push(node.LeftChild);
            if (node.RightChild != null) right.Push(node.RightChild);
        }

        if (root != null)
        {
            yield return root;

            addChildren(root);

            while (true)
            {
                if (left.Any())
                {
                    var item = left.Pop();
                    addChildren(item);
                    yield return item;
                }
                else if (right.Any())
                {
                    var item = right.Pop();
                    addChildren(item);
                    yield return item;
                }
                else break;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion
}