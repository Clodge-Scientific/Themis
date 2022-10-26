using System;
using System.Linq;

using Bogus;
using Xunit;
using Assert = Xunit.Assert;

using Themis.Geometry.Triangles;

using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry.Tests.Triangles
{
    public class TriangleTests
    {
        const int Dimensions = 3;
        const int Decimals = 6;

        const int ExpectedEdges = 3;
        const int ExpectedVertices = 3;

        const double Epsilon = 1E-6;
        const double MinValue = -500.0;
        const double MaxValue = 500.0;

        Faker _Faker;

        public TriangleTests()
        {
            _Faker = new Faker();
        }

        Vector<double>[] GenerateVertices(int count)
        {
            return Enumerable.Range(0, count)
                             .Select(i => _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector())
                             .ToArray();
        }

        #region IEquatable & Constructor Tests
        [Fact]
        public void EqualsSuccessTest()
        {
            var verts = GenerateVertices(ExpectedVertices);

            var TriangleA = new Triangle(verts);
            var TriangleB = new Triangle(verts);

            Assert.Equal(TriangleA, TriangleB);
            Assert.Equal(TriangleA, TriangleB as object);
        }

        [Fact]
        public void EqualsFailTest()
        {
            var verts = GenerateVertices(ExpectedVertices);

            var TriangleA = new Triangle(verts);

            verts = verts.Select(v => v * 1.1).ToArray();
            var TriangleB = new Triangle(verts);

            Assert.NotEqual(TriangleA, TriangleB);
            Assert.NotEqual(TriangleA, TriangleB as object);
        }

        [Fact]
        public void EdgeAndVertexCountTest()
        {
            var verts = GenerateVertices(ExpectedVertices);

            var TriangleA = new Triangle(verts);

            Assert.Equal(ExpectedVertices, TriangleA.Vertices.Count);
            Assert.Equal(ExpectedEdges, TriangleA.Edges.Count);
        }

        [Fact]
        public void TriangleBoundingBoxTest()
        {
            var verts = GenerateVertices(ExpectedVertices);

            double ExpectedMinX = verts.Min(v => v[0]);
            double ExpectedMinY = verts.Min(v => v[1]);
            double ExpectedMaxX = verts.Max(v => v[0]);
            double ExpectedMaxY = verts.Max(v => v[1]);

            var TriangleA = new Triangle(verts);

            //< 2D bounds should match expected values
            Assert.Equal(ExpectedMinX, TriangleA.Envelope.MinX);
            Assert.Equal(ExpectedMinY, TriangleA.Envelope.MinY);
            Assert.Equal(ExpectedMaxX, TriangleA.Envelope.MaxX);
            Assert.Equal(ExpectedMaxY, TriangleA.Envelope.MaxY);
            //< Elevation minima/maxima should remain NaN as un-used here
            Assert.True(double.IsNaN(TriangleA.Envelope.MinZ));
            Assert.True(double.IsNaN(TriangleA.Envelope.MaxZ));
        }
        #endregion

        #region Containment & Elevation Tests
        [Fact]
        public void TriangleContainsPointTest()
        {
            //< Forming Triangle (0, 0, 0) -> (1, 0, 0) -> (0, 1, 0)
            var A = new double[] { 0.0, 0.0, 0.0 }.ToVector();
            var B = A.CloneModify(0, 1.0);
            var C = A.CloneModify(1, 1.0);

            var verts = new Vector<double>[] { A, B, C };
            var Triangle = new Triangle(verts);

            var P1 = new double[] { 1.0, 1.0, 0.0 }.ToVector();
            var P2 = new double[] { -1.0, 0.0, 0.0 }.ToVector();
            var P3 = new double[] { 0.25, 0.25, 1337.0 }.ToVector();

            //< These vertices are both outside the known 2D Triangle bounds, should be false
            Assert.False(Triangle.Contains(P1));
            Assert.False(Triangle.Contains(P1[0], P1[1]));
            Assert.False(Triangle.Contains(P2));
            Assert.False(Triangle.Contains(P2[0], P2[1]));
            //< (0.25, 0.25, ZZZZ) is within the known 2D Triangle bounds, should be true
            Assert.True(Triangle.Contains(P3));
            Assert.True(Triangle.Contains(P3[0], P3[1]));
        }

        [Fact]
        public void SampleElevationFlatSurfaceTest()
        {
            double ExpectedZ = 1.0;

            //< Forming Triangle (0, 0, 1) -> (1, 0, 1) -> (0, 1, 1)
            var A = new double[] { 0.0, 0.0, ExpectedZ }.ToVector();
            var B = A.CloneModify(0, 1.0);
            var C = A.CloneModify(1, 1.0);

            var verts = new Vector<double>[] { A, B, C };
            var Triangle = new Triangle(verts);

            var POI = new double[] { 0.25, 0.25, 1337.0 }.ToVector();
            double ActualZ = Triangle.GetZ(POI);

            Assert.Equal(ExpectedZ, ActualZ);
        }

        [Fact]
        public void SampleElevationComplexSurfaceTest()
        {
            double ExpectedZ1 = 0.25;
            double ExpectedZ2 = 0.25;
            double ExpectedZ3 = 0.75;

            //< Forming Triangle (0, 0, 0) -> (1, 0, 1) -> (0, 1, 0)
            var A = new double[] { 0.0, 0.0, 0.0 }.ToVector();
            var B = new double[] { 1.0, 0.0, 1.0 }.ToVector();
            var C = new double[] { 0.0, 1.0, 0.0 }.ToVector();

            var verts = new Vector<double>[] { A, B, C };
            var Triangle = new Triangle(verts);

            //< Randomized POI elevations to show it doesn't matter what the POI's Z is
            var P1 = new double[] { 0.25, 0.25, _Faker.Random.Double(MinValue, MaxValue) }.ToVector();
            var P2 = new double[] { 0.25, 0.75, _Faker.Random.Double(MinValue, MaxValue) }.ToVector();
            var P3 = new double[] { 0.75, 0.25, _Faker.Random.Double(MinValue, MaxValue) }.ToVector();

            Assert.Equal(ExpectedZ1, Triangle.GetZ(P1), Decimals);
            Assert.Equal(ExpectedZ2, Triangle.GetZ(P2), Decimals);
            Assert.Equal(ExpectedZ3, Triangle.GetZ(P3), Decimals);

            Assert.Equal(ExpectedZ1, Triangle.GetZ(P1[0], P1[1]), Decimals);
            Assert.Equal(ExpectedZ2, Triangle.GetZ(P2[0], P2[1]), Decimals);
            Assert.Equal(ExpectedZ3, Triangle.GetZ(P3[0], P3[1]), Decimals);
        }
        #endregion
    }
}
