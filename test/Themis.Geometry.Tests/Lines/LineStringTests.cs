using System.Linq;

using Bogus;
using Xunit;
using Assert = Xunit.Assert;

using Themis.Geometry.Lines;

using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry.Tests.Lines
{
    public class LineStringTests
    {
        const int Dimensions = 3;

        const double MinValue = -500.0;
        const double MaxValue = 500.0;

        Faker _Faker;

        public LineStringTests()
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
        public void VertexAndSegmentCountTest()
        {
            int ExpectedVerticesA = 4;
            int ExpectedVerticesB = 100;

            int ExpectedSegmentsA = ExpectedVerticesA - 1;
            int ExpectedSegmentsB = ExpectedVerticesB - 1;

            var vertsA = GenerateVertices(ExpectedVerticesA);
            var vertsB = GenerateVertices(ExpectedVerticesB);

            var LineStringA = new LineString(vertsA);
            var LineStringB = new LineString(vertsB);

            Assert.Equal(ExpectedVerticesA, LineStringA.Vertices.Count);
            Assert.Equal(ExpectedVerticesB, LineStringB.Vertices.Count);

            Assert.Equal(ExpectedSegmentsA, LineStringA.Segments.Count);
            Assert.Equal(ExpectedSegmentsB, LineStringB.Segments.Count);
        }

        [Fact]
        public void ZeroLengthSegmentRemovalTest()
        {
            int ExpectedSegments = 2;

            var vertsA = GenerateVertices(3);
            //< Inserting a duplicated vertex in the LineString - resulting in a zero-length LineSegment
            var vertsB = new[] { vertsA[0], vertsA[1], vertsA[1], vertsA[2] };

            var LineStringA = new LineString(vertsA);
            var LineStringB = new LineString(vertsB);

            //< Both LineStrings should have the same Segment count as we expect the zero-length to be filtered
            Assert.Equal(ExpectedSegments, LineStringA.Segments.Count);
            Assert.Equal(ExpectedSegments, LineStringB.Segments.Count);
        }

        [Fact]
        public void EqualsSuccessTest()
        {
            var verts = GenerateVertices(3);

            var LineStringA = new LineString(verts);
            var LineStringB = new LineString(verts);

            Assert.Equal(LineStringA, LineStringB);
            Assert.Equal(LineStringA, LineStringB as object);
        }

        [Fact]
        public void EqualsFailTest()
        {
            var verts = GenerateVertices(3);

            var LineStringA = new LineString(verts);

            verts = verts.Select(v => v * 1.1).ToArray();
            var LineStringB = new LineString(verts);

            Assert.NotEqual(LineStringA, LineStringB);
            Assert.NotEqual(LineStringA, LineStringB as object);
        }
        #endregion

        #region Length & ILineString Tests
        [Fact]
        public void LineStringLengthTest()
        {
            //< Expectation is that 2D length ignores the last (1, 1, 0) -> (1, 1, 1) LineSegment
            double ExpectedLength2D = 2.0;
            double ExpectedLength3D = 3.0;

            //< Forming LineString (0, 0, 0) -> (1, 0, 0) -> (1, 1, 0) -> (1, 1, 1)
            var A = new double[] { 0.0, 0.0, 0.0 }.ToVector();
            var B = new double[] { 1.0, 0.0, 0.0 }.ToVector();
            var C = new double[] { 1.0, 1.0, 0.0 }.ToVector();
            var D = new double[] { 1.0, 1.0, 1.0 }.ToVector();

            var verts = new[] { A, B, C, D };
            var LineString = new LineString(verts);

            double ActualLength2D = LineString.Length2D;
            double ActualLength3D = LineString.Length3D;

            Assert.Equal(ExpectedLength2D, ActualLength2D);
            Assert.Equal(ExpectedLength3D, ActualLength3D);
            Assert.NotEqual(ActualLength3D, ActualLength2D);
        }

        [Fact]
        public void GetClosestPointTest()
        {
            var ExpectedP1 = new double[] { 1.0, 1.0, 0.5 };
            var ExpectedP2 = new double[] { 1.0, 0.5, 0.0 };

            var V1 = new double[] { 1.5, 1.0, 0.5 }.ToVector();
            var V2 = new double[] { 1.5, 0.5, 0.5 }.ToVector();

            //< Forming LineString (0, 0, 0) -> (1, 0, 0) -> (1, 1, 0) -> (1, 1, 1)
            var A = new double[] { 0.0, 0.0, 0.0 }.ToVector();
            var B = new double[] { 1.0, 0.0, 0.0 }.ToVector();
            var C = new double[] { 1.0, 1.0, 0.0 }.ToVector();
            var D = new double[] { 1.0, 1.0, 1.0 }.ToVector();

            var verts = new[] { A, B, C, D };
            var LineString = new LineString(verts);

            var P1 = LineString.GetClosestPoint(V1);
            var P2 = LineString.GetClosestPoint(V2);

            Assert.Equal(ExpectedP1, P1);
            Assert.Equal(ExpectedP2, P2);
        }
        #endregion
    }
}
