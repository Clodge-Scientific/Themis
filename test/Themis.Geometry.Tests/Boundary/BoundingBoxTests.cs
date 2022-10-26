using System;

using Bogus;
using Xunit;
using Assert = Xunit.Assert;

using Themis.Geometry.Boundary;

namespace Themis.Geometry.Tests.Boundary
{
    public class BoundingBoxTests
    {
        const double MinValue = 0.0;
        const double MaxValue = 1000.0;
        const double Range = 500.0;

        Faker<BoundingBox> _Faker2D;
        Faker<BoundingBox> _Faker3D;

        public BoundingBoxTests()
        {
            #region Faker Instantiation
            _Faker2D = new Faker<BoundingBox>()
                .RuleFor(f => f.MinX, f => f.Random.Double(MinValue, MinValue + Range))
                .RuleFor(f => f.MinY, f => f.Random.Double(MinValue, MinValue + Range))
                .RuleFor(f => f.MinZ, f => double.NaN)
                .RuleFor(f => f.MaxX, f => f.Random.Double(MaxValue, MaxValue + Range))
                .RuleFor(f => f.MaxY, f => f.Random.Double(MaxValue, MaxValue + Range))
                .RuleFor(f => f.MaxZ, f => double.NaN);

            _Faker3D = new Faker<BoundingBox>()
                .RuleFor(f => f.MinX, f => f.Random.Double(MinValue, MinValue + Range))
                .RuleFor(f => f.MinY, f => f.Random.Double(MinValue, MinValue + Range))
                .RuleFor(f => f.MinZ, f => f.Random.Double(MinValue, MinValue + Range))
                .RuleFor(f => f.MaxX, f => f.Random.Double(MaxValue, MaxValue + Range))
                .RuleFor(f => f.MaxY, f => f.Random.Double(MaxValue, MaxValue + Range))
                .RuleFor(f => f.MaxZ, f => f.Random.Double(MaxValue, MaxValue + Range));
            #endregion
        }

        #region Equality & Field Tests
        [Fact]
        public void EqualsSuccessTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = _Faker3D.Generate();

            var input = new
            {
                Value2D = Target2D as object,
                Value3D = Target3D as object
            };

            Assert.True(Target2D.Equals(input.Value2D));
            Assert.True(Target3D.Equals(input.Value3D));
        }

        [Fact]
        public void EqualsFailMinX()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = _Faker3D.Generate();

            var input = new
            {
                Value2D = BoundingBox.From(Target2D).WithMinima(Target2D.MinX - 1, Target2D.MinY) as object,
                Value3D = BoundingBox.From(Target3D).WithMinima(Target3D.MinX - 1, Target3D.MinY, Target3D.MinZ) as object,
            };

            Assert.False(Target2D.Equals(input.Value2D));
            Assert.False(Target3D.Equals(input.Value3D));
        }

        [Fact]
        public void EqualsFailMinY()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = _Faker3D.Generate();

            var input = new
            {
                Value2D = BoundingBox.From(Target2D).WithMinima(Target2D.MinX, Target2D.MinY + 1),
                Value3D = BoundingBox.From(Target3D).WithMinima(Target3D.MinX, Target3D.MinY + 1, Target3D.MinZ)
            };

            Assert.False(Target2D.Equals(input.Value2D));
            Assert.False(Target3D.Equals(input.Value3D));
        }

        [Fact]
        public void EqualsFailMinZ()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = _Faker3D.Generate();

            var input = new
            {
                Value2D = BoundingBox.From(Target2D).WithMinima(Target2D.MinX, Target2D.MinY, Target2D.MinZ + 1),
                Value3D = BoundingBox.From(Target3D).WithMinima(Target3D.MinX, Target3D.MinY, Target3D.MinZ + 1)
            };

            //< This should resolve to true as the 2D BB has Z of NaN and (NaN + 1 = NaN)
            Assert.True(Target2D.Equals(input.Value2D));
            //< This should resolve to false as the 3D BB has a rational value, and so will change
            Assert.False(Target3D.Equals(input.Value3D));
        }

        [Fact]
        public void EqualsFailMaxX()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = _Faker3D.Generate();

            var input = new
            {
                Value2D = BoundingBox.From(Target2D).WithMaxima(Target2D.MaxX - 1, Target2D.MaxY) as object,
                Value3D = BoundingBox.From(Target3D).WithMaxima(Target3D.MaxX - 1, Target3D.MaxY, Target3D.MaxZ) as object,
            };

            Assert.False(Target2D.Equals(input.Value2D));
            Assert.False(Target3D.Equals(input.Value3D));
        }

        [Fact]
        public void EqualsFailMaxY()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = _Faker3D.Generate();

            var input = new
            {
                Value2D = BoundingBox.From(Target2D).WithMaxima(Target2D.MaxX, Target2D.MaxY + 1),
                Value3D = BoundingBox.From(Target3D).WithMaxima(Target3D.MaxX, Target3D.MaxY + 1, Target3D.MaxZ)
            };

            Assert.False(Target2D.Equals(input.Value2D));
            Assert.False(Target3D.Equals(input.Value3D));
        }

        [Fact]
        public void EqualsFailMaxZ()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = _Faker3D.Generate();

            var input = new
            {
                Value2D = BoundingBox.From(Target2D).WithMaxima(Target2D.MaxX, Target2D.MaxY, Target2D.MaxZ + 1),
                Value3D = BoundingBox.From(Target3D).WithMaxima(Target3D.MaxX, Target3D.MaxY, Target3D.MaxZ + 1)
            };

            //< This should resolve to true as the 2D BB has Z of NaN and (NaN + 1 = NaN)
            Assert.True(Target2D.Equals(input.Value2D));
            //< This should resolve to false as the 3D BB has a rational value, and so will change
            Assert.False(Target3D.Equals(input.Value3D));
        }
        #endregion

        #region Fluent Interface & Factory Method Tests
        [Fact]
        public void WithOtherBoundingBoxMinimaTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            var Actual2D = new BoundingBox().WithMinima(Target2D);
            var Actual3D = new BoundingBox().WithMinima(Target3D);

            Assert.Equal(Actual2D.MinX, Target2D.MinX);
            Assert.Equal(Actual2D.MinY, Target2D.MinY);
            Assert.Equal(Actual2D.MinZ, Target2D.MinZ);

            Assert.Equal(Actual3D.MinX, Target3D.MinX);
            Assert.Equal(Actual3D.MinY, Target3D.MinY);
            Assert.Equal(Actual3D.MinZ, Target3D.MinZ);
        }

        [Fact]
        public void WithOtherBoundingBoxMaximaTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            var Actual2D = new BoundingBox().WithMaxima(Target2D);
            var Actual3D = new BoundingBox().WithMaxima(Target3D);

            Assert.Equal(Actual2D.MaxX, Target2D.MaxX);
            Assert.Equal(Actual2D.MaxY, Target2D.MaxY);
            Assert.Equal(Actual2D.MaxZ, Target2D.MaxZ);

            Assert.Equal(Actual3D.MaxX, Target3D.MaxX);
            Assert.Equal(Actual3D.MaxY, Target3D.MaxY);
            Assert.Equal(Actual3D.MaxZ, Target3D.MaxZ);
        }

        [Fact]
        public void From2DPointTest()
        {
            double X = 5.0;
            double Y = 5.0;
            double Buffer = 10.0;

            var Actual2D = BoundingBox.FromPoint(X, Y, Buffer);

            Assert.Equal(Actual2D.Width, Buffer);
            Assert.Equal(Actual2D.Height, Buffer);
            Assert.True(double.IsNaN(Actual2D.Depth));

            Assert.Equal(Actual2D.CentroidX, X);
            Assert.Equal(Actual2D.CentroidY, Y);
            Assert.True(double.IsNaN(Actual2D.CentroidZ));
        }

        [Fact]
        public void From3DPointTest()
        {
            double X = 5.0;
            double Y = 5.0;
            double Z = 5.0;
            double Buffer = 10.0;

            var Actual3D = BoundingBox.FromPoint(X, Y, Z, Buffer);

            Assert.Equal(Actual3D.Width, Buffer);
            Assert.Equal(Actual3D.Height, Buffer);
            Assert.Equal(Actual3D.Depth, Buffer);

            Assert.Equal(Actual3D.CentroidX, X);
            Assert.Equal(Actual3D.CentroidY, Y);
            Assert.Equal(Actual3D.CentroidZ, Z);
        }

        [Fact]
        public void From2DMinimaMaximaTest()
        {
            double MinX = 0.0; double MaxX = 5.0;
            double MinY = 0.0; double MaxY = 10.0;

            var Actual = BoundingBox.From(MinX, MinY, MaxX, MaxY);

            Assert.Equal(MinX, Actual.MinX);
            Assert.Equal(MinY, Actual.MinY);
            Assert.True(double.IsNaN(Actual.MinZ));
            Assert.Equal(MaxX, Actual.MaxX);
            Assert.Equal(MaxY, Actual.MaxY);
            Assert.True(double.IsNaN(Actual.MaxZ));
        }

        [Fact]
        public void From3DMinimaMaximaTest()
        {
            double MinX = 0.0; double MaxX = 5.0;
            double MinY = 0.0; double MaxY = 10.0;
            double MinZ = -5.0; double MaxZ = 25.0;

            var Actual = BoundingBox.From(MinX, MinY, MinZ, MaxX, MaxY, MaxZ);

            Assert.Equal(MinX, Actual.MinX);
            Assert.Equal(MinY, Actual.MinY);
            Assert.Equal(MinZ, Actual.MinZ);
            Assert.Equal(MaxX, Actual.MaxX);
            Assert.Equal(MaxY, Actual.MaxY);
            Assert.Equal(MaxZ, Actual.MaxZ);
        }

        [Fact]
        public void BufferExistingTest()
        {
            double Buffer = 5.0;

            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            var Actual2D = Target2D.Buffer(Buffer);
            var Actual3D = Target3D.Buffer(Buffer);

            Assert.Equal(Actual2D.MinX, Target2D.MinX - Buffer);
            Assert.Equal(Actual2D.MinY, Target2D.MinY - Buffer);
            Assert.Equal(Actual2D.MinZ, Target2D.MinZ - Buffer);
            Assert.Equal(Actual2D.MaxX, Target2D.MaxX + Buffer);
            Assert.Equal(Actual2D.MaxY, Target2D.MaxY + Buffer);
            Assert.Equal(Actual2D.MaxZ, Target2D.MaxZ + Buffer);

            Assert.Equal(Actual3D.MinX, Target3D.MinX - Buffer);
            Assert.Equal(Actual3D.MinY, Target3D.MinY - Buffer);
            Assert.Equal(Actual3D.MinZ, Target3D.MinZ - Buffer);
            Assert.Equal(Actual3D.MaxX, Target3D.MaxX + Buffer);
            Assert.Equal(Actual3D.MaxY, Target3D.MaxY + Buffer);
            Assert.Equal(Actual3D.MaxZ, Target3D.MaxZ + Buffer);
        }

        [Fact]
        public void ExpandToIncludeTest()
        {
            var A = _Faker3D.Generate();
            var B = _Faker3D.Generate();

            var Actual = A.ExpandToInclude(B);

            Assert.Equal(Actual.MinX, Math.Min(A.MinX, B.MinX));
            Assert.Equal(Actual.MinY, Math.Min(A.MinY, B.MinY));
            Assert.Equal(Actual.MinZ, Math.Min(A.MinZ, B.MinZ));

            Assert.Equal(Actual.MaxX, Math.Max(A.MaxX, B.MaxX));
            Assert.Equal(Actual.MaxY, Math.Max(A.MaxY, B.MaxY));
            Assert.Equal(Actual.MaxZ, Math.Max(A.MaxZ, B.MaxZ));
        }
        #endregion

        #region Containment & Intersection Tests
        [Fact]
        public void ContainsPointInsideSuccessTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            bool Contains2D = Target2D.Contains(Target2D.CentroidX, Target2D.CentroidY);
            bool Contains3D = Target3D.Contains(Target3D.CentroidX, Target3D.CentroidY, Target2D.CentroidZ);

            Assert.True(Contains2D);
            Assert.True(Contains3D);
        }

        [Fact]
        public void ContainsPointOutsideFailTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            bool BadX2D = Target2D.Contains(Target2D.MaxX + 0.01, Target2D.CentroidY);
            bool BadY2D = Target2D.Contains(Target2D.CentroidX, Target2D.MaxY + 0.01);

            Assert.False(BadX2D);
            Assert.False(BadY2D);

            bool BadX3D = Target3D.Contains(Target3D.MaxX + 0.01, Target3D.CentroidY, Target3D.CentroidZ);
            bool BadY3D = Target3D.Contains(Target3D.CentroidX, Target3D.MaxY + 0.01, Target3D.CentroidZ);
            bool BadZ3D = Target3D.Contains(Target3D.CentroidX, Target3D.CentroidY, Target3D.MaxZ + 0.01);

            Assert.False(BadX3D);
            Assert.False(BadY3D);
            Assert.False(BadZ3D);
        }

        [Fact]
        public void IntersectionSuccessTest()
        {
            double Buffer = 1.0;

            var Target2D = _Faker2D.Generate();
            var Target3D = _Faker3D.Generate();

            var Other2D = BoundingBox.FromPoint(Target2D.MaxX, Target2D.MaxY, Buffer);
            var Other3D = BoundingBox.FromPoint(Target3D.MaxX, Target3D.MaxY, Target3D.MaxZ, Buffer);

            Assert.True(Target2D.Intersects(Other2D));
            Assert.True(Target3D.Intersects(Other3D));
        }

        [Fact]
        public void IntersectionFailTest()
        {
            double Buffer = 1.0;
            double Offset = 2.0 * Buffer;

            var Target2D = _Faker2D.Generate();
            var Target3D = _Faker3D.Generate();

            var Other2D = BoundingBox.FromPoint(Target2D.MaxX + Offset, Target2D.MaxY + Offset, Buffer);
            var Other3D = BoundingBox.FromPoint(Target3D.MaxX + Offset, Target3D.MaxY + Offset, Target3D.MaxZ + Offset, Buffer);

            Assert.False(Target2D.Intersects(Other2D));
            Assert.False(Target3D.Intersects(Other3D));
        }
        #endregion

        #region Property Tests
        [Fact]
        public void WidthTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            double ExpectedWidth = Target2D.MaxX - Target2D.MinX;

            //< The Width should remain the same regardless of dimensionality
            Assert.Equal(ExpectedWidth, Target2D.Width);
            Assert.Equal(ExpectedWidth, Target3D.Width);
        }

        [Fact]
        public void HeightTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            double ExpectedHeight = Target2D.MaxY - Target2D.MinY;

            //< The Height should remain the same regardless of dimensionality
            Assert.Equal(ExpectedHeight, Target2D.Height);
            Assert.Equal(ExpectedHeight, Target3D.Height);
        }

        [Fact]
        public void DepthTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            double ExpectedDepth = Target3D.MaxZ - Target3D.MinZ;

            //< The Depth should be NaN for the 2D BB
            Assert.True(double.IsNaN(Target2D.Depth));
            //< The Depth should match expected for the 3D BB
            Assert.Equal(ExpectedDepth, Target3D.Depth);
        }

        [Fact]
        public void AreaTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            double ExpectedArea = Target2D.Width * Target2D.Height;

            //< The Area should remain the same between 2/3D BB that only differ in Z
            Assert.Equal(ExpectedArea, Target2D.Area);
            Assert.Equal(ExpectedArea, Target3D.Area);
        }

        [Fact]
        public void VolumeTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            double ExpectedVolume = Target3D.Width * Target3D.Height * Target3D.Depth;

            //< The 2D BB should have a NaN volume, while the 3D should be legit
            Assert.True(double.IsNaN(Target2D.Volume));
            Assert.Equal(ExpectedVolume, Target3D.Volume);
        }

        [Fact]
        public void GetCentroidTest()
        {
            var Target3D = _Faker3D.Generate();

            double ExpectedCentroidX = Target3D.MinX + (Target3D.MaxX - Target3D.MinX) / 2.0;
            double ExpectedCentroidY = Target3D.MinY + (Target3D.MaxY - Target3D.MinY) / 2.0;
            double ExpectedCentroidZ = Target3D.MinZ + (Target3D.MaxZ - Target3D.MinZ) / 2.0;

            double ActualCentroidX = BoundingBox.GetCentroid(Target3D.MinX, Target3D.MaxX);
            double ActualCentroidY = BoundingBox.GetCentroid(Target3D.MinY, Target3D.MaxY);
            double ActualCentroidZ = BoundingBox.GetCentroid(Target3D.MinZ, Target3D.MaxZ);

            Assert.Equal(ExpectedCentroidX, ActualCentroidX);
            Assert.Equal(ExpectedCentroidY, ActualCentroidY);
            Assert.Equal(ExpectedCentroidZ, ActualCentroidZ);
        }

        [Fact]
        public void CentroidXTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            double ExpectedCentroid = BoundingBox.GetCentroid(Target2D.MinX, Target2D.MaxX);

            Assert.Equal(ExpectedCentroid, Target2D.CentroidX);
            Assert.Equal(ExpectedCentroid, Target3D.CentroidX);
        }

        [Fact]
        public void CentroidYTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            double ExpectedCentroid = BoundingBox.GetCentroid(Target2D.MinY, Target2D.MaxY);

            Assert.Equal(ExpectedCentroid, Target2D.CentroidY);
            Assert.Equal(ExpectedCentroid, Target3D.CentroidY);
        }

        [Fact]
        public void CentroidZTest()
        {
            var Target2D = _Faker2D.Generate();
            var Target3D = BoundingBox.From(Target2D)
                                      .WithZ(0, 100.0);

            double ExpectedCentroid = BoundingBox.GetCentroid(Target3D.MinZ, Target3D.MaxZ);

            //< The 2D BB should have a NaN CentroidZ
            Assert.True(double.IsNaN(Target2D.CentroidZ));
            Assert.Equal(ExpectedCentroid, Target3D.CentroidZ);
        }
        #endregion
    }
}