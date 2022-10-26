using System;

using Bogus;
using Xunit;
using Assert = Xunit.Assert;

using Themis.Geometry.Lines;

using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry.Tests.Lines
{
    public class LineSegmentTests
    {
        const int Dimensions = 3;
        const int Decimals = 6;

        const double Epsilon = 1E-6;
        const double TestValue = -500.1;
        const double MinValue = -500.0;
        const double MaxValue = 500.0;

        static readonly Vector<double> Offset = new double[] { 1000.0, 1000.0, 0.0 }.ToVector();

        Faker _Faker;

        public LineSegmentTests()
        {
            _Faker = new Faker();
        }

        #region IEquatable & Constructor Tests
        [Fact]
        public void EqualsSuccessTest()
        {
            var A = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();
            var B = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();

            var segA = new LineSegment(A, B);
            var segB = new LineSegment(A, B);

            Assert.Equal(segA, segB);
        }

        [Fact]
        public void EqualsFailTest()
        {
            var A = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();
            var B = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();

            var segA = new LineSegment(A, B);

            var C = A.CloneModify(0, TestValue);
            var D = B.CloneModify(1, TestValue);

            var segB = new LineSegment(C, D);

            Assert.NotEqual(segA, segB);
        }

        [Fact]
        public void ConstructorDeepCopyTest()
        {
            var A = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();
            var B = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();

            var Seg = new LineSegment(A, B);

            A = A.CloneModify(0, TestValue);
            B = B.CloneModify(1, TestValue);

            Assert.NotEqual(Seg.A, A);
            Assert.NotEqual(Seg.B, B);
        }
        #endregion

        #region Unit Vector & Length Tests
        [Fact]
        public void LineSegmentLengthTest()
        {
            var A = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();
            var B = A + Offset;

            double ExpectedLength = (B - A).L2Norm();
            double ExpectedLength2D = (B.CloneModify(2, 0) - A.CloneModify(2, 0)).L2Norm();

            var Seg = new LineSegment(A, B);

            Assert.Equal(ExpectedLength, Seg.Length);
            Assert.Equal(ExpectedLength2D, Seg.Length2D);
        }

        [Fact]
        public void LineSegmentUnitVectorTest()
        {
            var A = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();
            var B = A + Offset;

            //< A Unit Vector must always have a vector magnitude of 1.0
            double ExpectedLength = 1.0;
            var ExpectedDirection = (B - A) / (B - A).L2Norm();

            var Seg = new LineSegment(A, B);
            var ActualUnit = Seg.Unit;
            var ActualDirDiff = (ExpectedDirection - ActualUnit).L2Norm();
            Assert.Equal(ExpectedLength, ActualUnit.L2Norm(), Decimals);
            Assert.True(ActualDirDiff < Epsilon);
        }
        #endregion

        #region ILineSegment Method Tests
        [Fact]
        public void GetStationTest()
        {
            var Origin = new double[] { 0.0, 0.0 }.ToVector();
            var A = Origin.CloneModify(0, 1.0);
            var B = Origin.CloneModify(1, 1.0);
            var V = new double[] { 0.5, 0.5 }.ToVector();

            var SegA = new LineSegment(Origin, A);
            var SegB = new LineSegment(Origin, B);

            double ExpectedStation = 0.5;
            double ActualStationA = SegA.GetStation(V);
            double ActualStationB = SegB.GetStation(V);
            
            Assert.Equal(ExpectedStation, ActualStationA);
            Assert.Equal(ExpectedStation, ActualStationB);
        }

        [Fact]
        public void DistanceToPointTest()
        {
            var Origin = new double[] { 0.0, 0.0, 0.0 }.ToVector();
            var A = Origin.CloneModify(0, 1.0);
            var B = Origin.CloneModify(1, 1.0);
            var V = new double[] { 0.5, 0.5, 0.0 }.ToVector();

            var SegA = new LineSegment(Origin, A);
            var SegB = new LineSegment(Origin, B);

            double ExpectedDistance = 0.5;
            double ActualDistanceA = SegA.DistanceToPoint(V);
            double ActualDistanceB = SegB.DistanceToPoint(V);

            Assert.Equal(ExpectedDistance, ActualDistanceA);
            Assert.Equal(ExpectedDistance, ActualDistanceB);
        }

        [Fact]
        public void GetClosestPointTest()
        {
            var A = new double[] { 0.0, 0.0, 0.0 }.ToVector();
            var B = A.CloneModify(1, 1.0);
            var Seg = new LineSegment(A, B);

            //< Three query vertices - one in the middle, and two before/after the segment
            var V1 = new double[] { 0.5, 1.5, 0.0 }.ToVector();
            var V2 = V1.CloneModify(1, 0.5); //< At (0.5, 0.5)
            var V3 = V1.CloneModify(1, -0.5);//< At (0.5, -0.5)

            //< We expect V1 & V3 to return nearest start/end vertex
            var ExpectedP1 = Seg.B;
            var ExpectedP2 = new double[] { 0.0, 0.5, 0.0 }.ToVector();
            var ExpectedP3 = Seg.A;

            var P1 = Seg.GetClosestPoint(V1);
            var P2 = Seg.GetClosestPoint(V2);
            var P3 = Seg.GetClosestPoint(V3);

            Assert.Equal(ExpectedP1, P1);
            Assert.Equal(ExpectedP2, P2);
            Assert.Equal(ExpectedP3, P3);
        }

        [Fact]
        public void ExtractPointTest()
        {
            var A = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();
            var B = A + Offset;

            var Seg = new LineSegment(A, B);

            var V1 = Seg.ExtractPoint(0.0);
            var V2 = Seg.ExtractPoint(Seg.Length / 2.0);
            var V3 = Seg.ExtractPoint(Seg.Length);

            var dV1 = (V1 - A).L2Norm();
            var dV2 = (V2 - A).L2Norm();
            var dV3 = (V3 - A).L2Norm();

            Assert.Equal(0.0, dV1, Decimals);
            Assert.Equal(Seg.Length / 2.0, dV2, Decimals);
            Assert.Equal(Seg.Length, dV3, Decimals);
        }
        #endregion
    }
}
