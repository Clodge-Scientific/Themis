using System.Linq;

using Bogus;
using Xunit;
using Assert = Xunit.Assert;

using MathNet.Numerics.LinearAlgebra;

using Themis.Geometry;
using Themis.Geometry.Boundary;
using Themis.Geometry.Triangles;
using Themis.Index.QuadTree;

namespace Themis.Index.Tests.QuadTree;

public class QuadTreeTests
{
    const int Dimensions = 2;

    const double MinValue = -500.0;
    const double MaxValue = 500.0;

    Faker _Faker;

    public QuadTreeTests()
    {
        _Faker = new Faker();
    }

    #region Constructor Tests
    [Fact]
    public void BaseConstructorTest()
    {
        int maxItems = _Faker.Random.Int(4, 16);

        var TreeA = new QuadTree<Triangle>();
        var TreeB = new QuadTree<Triangle>(maxItems);

        Assert.Equal(QuadTree<Triangle>.DefaultMaxItemsPerNode, TreeA.MaxItemsPerNode);
        Assert.Equal(maxItems, TreeB.MaxItemsPerNode);

        //< Root QuadTreeNode should be null with the base constructor
        Assert.True(TreeA.IsRootNull);
        Assert.True(TreeB.IsRootNull);
        //< The RootEnvelope should also be null
        Assert.Null(TreeA.RootEnvelope);
        Assert.Null(TreeB.RootEnvelope);
        Assert.Null(TreeA.RootEnvelope);
        Assert.Null(TreeB.RootEnvelope);
    }

    [Fact]
    public void MinimaMaximaConstructorTest()
    {
        int maxItems = _Faker.Random.Int(4, 16);

        var TreeA = new QuadTree<Triangle>(MinValue, MinValue, MaxValue, MaxValue);
        var TreeB = new QuadTree<Triangle>(MinValue, MinValue, MaxValue, MaxValue, maxItems);

        Assert.Equal(QuadTree<Triangle>.DefaultMaxItemsPerNode, TreeA.MaxItemsPerNode);
        Assert.Equal(maxItems, TreeB.MaxItemsPerNode);

        //< Using the minima/maxima constructor should pre-populate the root QuadTreeNode
        Assert.False(TreeA.IsRootNull);
        Assert.False(TreeB.IsRootNull);
        Assert.NotNull(TreeA.RootEnvelope);
        Assert.NotNull(TreeB.RootEnvelope);
    }

    [Fact]
    public void IBoundingBoxConstructorTest()
    {
        int maxItems = _Faker.Random.Int(4, 16);

        var bb = BoundingBox.From(MinValue, MinValue, MaxValue, MaxValue);

        var TreeA = new QuadTree<Triangle>(bb);
        var TreeB = new QuadTree<Triangle>(bb, maxItems);

        Assert.Equal(QuadTree<Triangle>.DefaultMaxItemsPerNode, TreeA.MaxItemsPerNode);
        Assert.Equal(maxItems, TreeB.MaxItemsPerNode);

        //< Using the IBoundingBox constructor should pre-populate the root QuadTreeNode
        Assert.False(TreeA.IsRootNull);
        Assert.False(TreeB.IsRootNull);
        Assert.NotNull(TreeA.RootEnvelope);
        Assert.NotNull(TreeB.RootEnvelope);
    }
    #endregion

    #region IQuadTree Add & Remove Tests
    [Fact]
    public void AddItemByBoundingBoxTest()
    {
        var vec = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();
        var bb = BoundingBox.FromPoint(vec[0], vec[1], BoundingBox.SinglePointBuffer);

        int ExpectedCount = 1;
        var ExpectedItems = new[] { vec };

        var Tree = new QuadTree<Vector<double>>();
        Tree.Add(vec, bb);

        var ActualItems = Tree.Items;

        Assert.Equal(ExpectedCount, Tree.Count);
        Assert.Equal(ExpectedItems.Single(), ActualItems.Single());
    }

    [Fact]
    public void AddItemByMinimaMaximaTest()
    {
        var vec = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();
        var bb = BoundingBox.FromPoint(vec[0], vec[1], BoundingBox.SinglePointBuffer);

        int ExpectedCount = 1;
        var ExpectedItems = new[] { vec };

        var Tree = new QuadTree<Vector<double>>();
        Tree.Add(vec, bb.MinX, bb.MinY, bb.MaxX, bb.MaxY);

        var ActualItems = Tree.Items;

        Assert.Equal(ExpectedCount, Tree.Count);
        Assert.Equal(ExpectedItems.Single(), ActualItems.Single());
    }

    [Fact]
    public void RemoveItemTest()
    {
        var VecA = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();
        var VecB = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();
        var BoundsA = BoundingBox.FromPoint(VecA[0], VecA[1], BoundingBox.SinglePointBuffer);
        var BoundsB = BoundingBox.FromPoint(VecB[0], VecB[1], BoundingBox.SinglePointBuffer);

        int ExpectedCount = 1;
        var ExpectedItems = new[] { VecB };

        //< Add VecA and VecB to the new QuadTree<T>
        var Tree = new QuadTree<Vector<double>>();
        Tree.Add(VecA, BoundsA);
        Tree.Add(VecB, BoundsB);

        //< Remove VecA from the QuadTree<T>
        Tree.Remove(VecA);

        //< Get all currently contained items
        var ActualItems = Tree.Items;

        //< Expect that the only returned item is VecB
        Assert.Equal(ExpectedCount, Tree.Count);
        Assert.Equal(ExpectedItems.Single(), ActualItems.Single());
    }
    #endregion

    #region IQuadTree Query Methods
    static (Triangle ABC, Triangle DEF) GetTestTriangles()
    {
        //< First Triangle ABC
        var A = new double[] { 0.0, 0.0, 1.0 }.ToVector();
        var B = new double[] { 0.0, 1.0, 1.0 }.ToVector();
        var C = new double[] { 1.0, 1.0, 1.0 }.ToVector();

        //< Second Triangle DEF
        var D = new double[] { 2.0, 0.0, -1.0 }.ToVector();
        var E = new double[] { 2.0, 2.0, -1.0 }.ToVector();
        var F = new double[] { 4.0, 2.0, -1.0 }.ToVector();

        return new(new Triangle(new[] { A, B, C }), new Triangle(new[] { D, E, F }));
    }

    [Fact]
    public void QueryDistinctByPointTest()
    {
        int ExpectedCount = 2;
        int ExpectedQueryCount = 1;

        var (TriangleA, TriangleB) = GetTestTriangles();

        var Tree = new QuadTree<Triangle>();
        Tree.Add(TriangleA, TriangleA.Envelope);
        Tree.Add(TriangleB, TriangleB.Envelope);

        Assert.Equal(ExpectedCount, Tree.Count);

        var P1 = new double[] { 0.75, 0.75 }.ToVector();
        var P2 = new double[] { 2.75, 0.75 }.ToVector();

        var QueryA = Tree.QueryDistinct(P1[0], P1[1]).ToArray();
        var QueryB = Tree.QueryDistinct(P2[0], P2[1]).ToArray();

        Assert.Equal(ExpectedQueryCount, QueryA.Length);
        Assert.Equal(ExpectedQueryCount, QueryB.Length);

        Assert.Equal(TriangleA, QueryA.Single());
        Assert.Equal(TriangleB, QueryB.Single());
    }

    [Fact]
    public void QueryDistinctByBoundingBoxTest()
    {
        int ExpectedCount = 2;
        int ExpectedQueryCount = 1;

        var (TriangleA, TriangleB) = GetTestTriangles();

        var Tree = new QuadTree<Triangle>();
        Tree.Add(TriangleA, TriangleA.Envelope);
        Tree.Add(TriangleB, TriangleB.Envelope);

        Assert.Equal(ExpectedCount, Tree.Count);

        var QueryA = Tree.QueryDistinct(TriangleA.Envelope).ToArray();
        var QueryB = Tree.QueryDistinct(TriangleB.Envelope).ToArray();

        Assert.Equal(ExpectedQueryCount, QueryA.Length);
        Assert.Equal(ExpectedQueryCount, QueryB.Length);

        Assert.Equal(TriangleA, QueryA.Single());
        Assert.Equal(TriangleB, QueryB.Single());
    }
    #endregion
}