using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;
using Assert = Xunit.Assert;

using Themis.Index.KdTree;
using Themis.Index.KdTree.TypeMath;

namespace Themis.Index.Tests.KdTree;

public class KdTreeTests
{
    const int Dimensions = 2;
    static readonly FloatMath TypeMath = new();

    static void AddTestNodes(KdTree<float, string> tree)
    {
        foreach (var node in TestItems.Nodes)
        {
            if (node.Point == null || node.Value == null) throw new Exception($"Cannot test node with null Point or Value");
            if (!tree.Add(node.Point, node.Value)) throw new Exception($"Failed to add Node to tree! Node: {node}");
        }
    }

    #region IKdTree<TKey, TValue> Constructor Tests
    [Fact]
    public void TreeDefaultStateTest()
    {
        var Tree = new KdTree<float, string>(Dimensions, TypeMath);
        Assert.Equal(Dimensions, Tree.Dimensions);
        Assert.Equal(0, Tree.Count);
        Assert.Equal(AddDuplicateBehavior.Error, Tree.AddDuplicateBehavior);
    }

    [Fact]
    public void TreeExplicitDuplicateBehaviourTest()
    {
        AddDuplicateBehavior BehaviourA = AddDuplicateBehavior.Error;
        AddDuplicateBehavior BehaviourB = AddDuplicateBehavior.Skip;

        var TreeA = new KdTree<float, string>(Dimensions, TypeMath, BehaviourA);
        var TreeB = new KdTree<float, string>(Dimensions, TypeMath, BehaviourB);

        Assert.Equal(Dimensions, TreeA.Dimensions);
        Assert.Equal(0, TreeA.Count);
        Assert.Equal(BehaviourA, TreeA.AddDuplicateBehavior);

        Assert.Equal(Dimensions, TreeB.Dimensions);
        Assert.Equal(0, TreeB.Count);
        Assert.Equal(BehaviourB, TreeB.AddDuplicateBehavior);
    }
    #endregion

    #region IKdTree<TKey, TValue> Add, Remove & Clear Tests
    [Fact]
    public void AddNodesAndCheckCountTest()
    {
        var Tree = new KdTree<float, string>(Dimensions, TypeMath);
        Assert.Equal(Dimensions, Tree.Dimensions);
        Assert.Equal(0, Tree.Count);

        AddTestNodes(Tree);

        Assert.Equal(TestItems.Nodes.Count, Tree.Count);
    }

    [Fact]
    public void AddDuplicatePointSkipTest()
    {
        var Tree = new KdTree<float, string>(Dimensions, TypeMath, AddDuplicateBehavior.Skip);
        Assert.Equal(AddDuplicateBehavior.Skip, Tree.AddDuplicateBehavior);

        AddTestNodes(Tree);

        //< Get the current count - then try to add a point with a duplicate position
        int ExpectedCount = Tree.Count;
        bool WasAdded = Tree.Add(TestItems.Nodes.First().Point, "DuPlIcAtEd, NeRd!");

        //< We expect the count to not change, and the addition to have returned false
        Assert.Equal(ExpectedCount, Tree.Count);
        Assert.False(WasAdded);
    }

    [Fact]
    public void AddDuplicatePointUpdateTest()
    {
        string ExpectedValue = "DuPlIcAtEd, NeRd!";

        var Tree = new KdTree<float, string>(Dimensions, TypeMath, AddDuplicateBehavior.Update);
        Assert.Equal(AddDuplicateBehavior.Update, Tree.AddDuplicateBehavior);

        AddTestNodes(Tree);

        //< Get the current count - then try to add a point with a duplicate position
        int ExpectedCount = Tree.Count;
        bool WasAdded = Tree.Add(TestItems.Nodes.First().Point, ExpectedValue);

        var Value = Tree.FindValueAt(TestItems.Nodes.First().Point);

        //< We expect the count to not change, and the addition to have returned false
        Assert.Equal(ExpectedCount, Tree.Count);
        Assert.Equal(ExpectedValue, Value);
        Assert.True(WasAdded);
    }

    [Fact]
    public void AddDuplicatePointErrorTest()
    {
        var Tree = new KdTree<float, string>(Dimensions, TypeMath, AddDuplicateBehavior.Error);
        Assert.Equal(AddDuplicateBehavior.Error, Tree.AddDuplicateBehavior);

        AddTestNodes(Tree);

        //< Get the current count - then try to add a point with a duplicate position
        int ExpectedCount = Tree.Count;

        //< We expect this thing to chuck a 'DuplicateNodeError' exception
        Assert.Throws<DuplicateNodeError>(() => Tree.Add(TestItems.Nodes.First().Point, "DuPlIcAtEd, NeRd!"));
    }

    [Fact]
    public void RemoveNodeTest()
    {
        var Tree = new KdTree<float, string>(Dimensions, TypeMath);

        AddTestNodes(Tree);

        var CurrentNodes = TestItems.Nodes.ToList();
        var IndicesToRemove = new HashSet<int>() { 0, 1 };
        var NodesToRemove = IndicesToRemove.Select(i => TestItems.Nodes[i]).ToArray();

        foreach (var nodeToRemove in NodesToRemove)
        {
            Tree.Remove(nodeToRemove.Point);
            CurrentNodes.Remove(nodeToRemove);

            Assert.Empty(Tree.FindValue(nodeToRemove.Value));
            Assert.Equal(default, Tree.FindValueAt(nodeToRemove.Point));

            foreach (var node in CurrentNodes)
            {
                Assert.Equal(node.Value, Tree.FindValueAt(node.Point));
                Assert.Equal(node.Point, Tree.FindValue(node.Value));
            }

            Assert.Equal(CurrentNodes.Count, Tree.Count);
        }
    }

    [Fact]
    public void ClearTreeTest()
    {
        var Tree = new KdTree<float, string>(Dimensions, TypeMath);

        AddTestNodes(Tree);

        Assert.Equal(TestItems.Nodes.Count, Tree.Count);

        Tree.Clear();

        Assert.Equal(0, Tree.Count);
    }
    #endregion

    #region IKdTree<TKey, TValue> Find & TryFind Tests
    [Fact]
    public void TryFindValueAtTest()
    {
        var Tree = new KdTree<float, string>(Dimensions, TypeMath);

        AddTestNodes(Tree);

        bool Found;
        string ActualValue;

        foreach (var node in TestItems.Nodes)
        {
            Found = Tree.TryFindValueAt(node.Point, out ActualValue);

            Assert.True(Found);
            Assert.Equal(node.Value, ActualValue);
        }

        Found = Tree.TryFindValueAt(new float[] { 3.14f, 1337 }, out ActualValue);
        Assert.False(Found);
        Assert.Equal(default, ActualValue);
    }

    [Fact]
    public void FindValueAtTest()
    {
        var Tree = new KdTree<float, string>(Dimensions, TypeMath);

        AddTestNodes(Tree);

        string ActualValue;

        foreach (var node in TestItems.Nodes)
        {
            ActualValue = Tree.FindValueAt(node.Point);
            Assert.Equal(node.Value, ActualValue);
        }

        ActualValue = Tree.FindValueAt(new float[] { 3.14f, 1337 });
        Assert.Equal(default, ActualValue);
    }

    [Fact]
    public void FindValueTest()
    {
        var Tree = new KdTree<float, string>(Dimensions, TypeMath);

        AddTestNodes(Tree);

        float[] ActualPoint;

        foreach (var node in TestItems.Nodes)
        {
            ActualPoint = Tree.FindValue(node.Value).ToArray();
            Assert.Equal(node.Point, ActualPoint);
        }

        ActualPoint = Tree.FindValue("Yer mum").ToArray();
        Assert.Empty(ActualPoint);
    }
    #endregion

    #region IKdTree<TKey, TValue> Radial & Nearest Neighbour Search Tests
    static void AddTestCities(KdTree<float, string> tree)
    {
        foreach (var city in TestItems.Cities)
        {
            if (!tree.Add(new float[] { city.Long, -city.Lat }, city.Address)) throw new Exception($"Failed to add City to tree! City: {city}");
        }
    }

    [Fact]
    public void NearestNeighbourTest()
    {
        var Tree = new KdTree<float, string>(Dimensions, TypeMath);
        var QueryPos = new float[] { TestItems.Toowoomba.Long, -TestItems.Toowoomba.Lat };

        AddTestCities(Tree);

        foreach (int numNeighs in Enumerable.Range(0, TestItems.Cities.Count + 1))
        {
            var ActualNeighs = Tree.GetNearestNeighbours(QueryPos, numNeighs).ToArray();

            var ExpectedNeighs = TestItems.Cities.OrderBy(c => c.DistanceFromToowoomba)
                                                 .Take(numNeighs)
                                                 .ToArray();

            Assert.Equal(numNeighs, ActualNeighs.Length);
            Assert.Equal(numNeighs, ExpectedNeighs.Length);

            foreach (int index in Enumerable.Range(0, ActualNeighs.Length))
            {
                Assert.Equal(ExpectedNeighs[index].Address, ActualNeighs[index].Value);
            }
        }
    }

    [Fact]
    public void RadialSearchTest()
    {
        var Tree = new KdTree<float, string>(Dimensions, TypeMath);
        var QueryPos = new float[] { TestItems.Toowoomba.Long, -TestItems.Toowoomba.Lat };

        AddTestCities(Tree);

        var ExpectedNeighs = TestItems.Cities.OrderBy(c => c.DistanceFromToowoomba).ToList();

        for (int i = 1; i < 100; i *= 2)
        {
            var ActualNeighs = Tree.RadialSearch(QueryPos, i).ToArray();

            foreach (int index in Enumerable.Range(0, ActualNeighs.Length))
            {
                Assert.Equal(ExpectedNeighs[index].Address, ActualNeighs[index].Value);
            }
        }
    }
    #endregion

    #region IEnumerable Tests
    [Fact]
    public void TestEnumerable()
    {
        var Tree = new KdTree<float, string>(Dimensions, TypeMath);

        AddTestNodes(Tree);

        int NodeCount = 0;
        foreach (var node in Tree)
        {
            var ActualNode = TestItems.Nodes.FirstOrDefault(n => n.Point.SequenceEqual(node.Point) && n.Value.Equals(node.Value));

            Assert.NotNull(ActualNode);
            NodeCount++;
        }

        Assert.Equal(TestItems.Nodes.Count, NodeCount);
    }
    #endregion
}