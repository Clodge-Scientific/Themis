using System.Linq;

using Bogus;
using Xunit;
using Assert = Xunit.Assert;

using Themis.Geometry;

using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry.Tests
{
    public class RotationTests
    {
        const int Dimensions = 3;
        const double Epsilon = 1E-6;

        const double MinValue = -500.0;
        const double MaxValue = 500.0;

        const double MinAngle = 30.0;
        const double MaxAngle = 120.0;

        Faker _Faker;

        public RotationTests()
        {
            _Faker = new Faker();
        }

        [Fact]
        public void RotationMatrixTest()
        {
            double RotationRadians = Functions.ToRadians(90.0);

            var A = new double[] { 1.0, 1.0, 1.0 }.ToVector();

            var RotX = Rotation.GenerateRotationMatrix(RotationRadians, 0.0, 0.0);
            var RotY = Rotation.GenerateRotationMatrix(0.0, RotationRadians, 0.0);
            var RotZ = Rotation.GenerateRotationMatrix(0.0, 0.0, RotationRadians);
            var RotAll = Rotation.GenerateRotationMatrix(RotationRadians, RotationRadians, RotationRadians);

            var Bx = A * RotX;
            var By = A * RotY;
            var Bz = A * RotZ;
            var B = A * RotAll;

            //< Expected result vectors for each rotation
            var ExpectedBx = A.CloneModify(2, -1.0);
            var ExpectedBy = A.CloneModify(0, -1.0);
            var ExpectedBz = A.CloneModify(1, -1.0);
            var ExpectedB = A.CloneModify(0, -1.0);

            //< Get the difference between expected result vectors and actual
            var dX = (ExpectedBx - Bx).L2Norm();
            var dY = (ExpectedBy - By).L2Norm();
            var dZ = (ExpectedBz - Bz).L2Norm();
            var dAll = (ExpectedB - B).L2Norm();

            //< Ensure the differences are less than our allowable error (epsilon)
            Assert.True(dX < Epsilon);
            Assert.True(dY < Epsilon);
            Assert.True(dZ < Epsilon);
            Assert.True(dAll < Epsilon);
        }

        [Fact]
        public void RotationMatrixByVectorTest()
        {
            var Vec = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();

            double RotationDegrees = _Faker.Random.Double(MinAngle, MaxAngle);
            double RotationRads = Functions.ToRadians(RotationDegrees);
            var RotVec = new double[] { RotationRads, RotationRads, RotationRads }.ToVector();

            var RotByValue = Rotation.GenerateRotationMatrix(RotationRads, RotationRads, RotationRads);
            var RotByVector = Rotation.GenerateRotationMatrix(RotVec);

            var ResultByValue = Vec * RotByValue;
            var ResultByVector = Vec * RotByVector;

            var Difference = (ResultByValue - ResultByVector).L2Norm();
            Assert.True(Difference < Epsilon);
        }

        [Fact]
        public void RotationAroundXTest()
        {
            var Vec = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();

            double RotationDegrees = _Faker.Random.Double(MinAngle, MaxAngle);
            double RotationX = Functions.ToRadians(RotationDegrees);

            var RotationMatrix = Rotation.GenerateRotationAroundX(RotationX);

            var VecRotated = Vec * RotationMatrix;

            Assert.Equal(Vec[0], VecRotated[0]);
            Assert.NotEqual(Vec[1], VecRotated[1]);
            Assert.NotEqual(Vec[2], VecRotated[2]);
        }

        [Fact]
        public void RotationAroundYTest()
        {
            var Vec = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();

            double RotationDegrees = _Faker.Random.Double(MinAngle, MaxAngle);
            double RotationY = Functions.ToRadians(RotationDegrees);
            var RotationMatrix = Rotation.GenerateRotationAroundY(RotationY);

            var VecRotated = Vec * RotationMatrix;

            Assert.NotEqual(Vec[0], VecRotated[0]);
            Assert.Equal(Vec[1], VecRotated[1]);
            Assert.NotEqual(Vec[2], VecRotated[2]);
        }

        [Fact]
        public void RotationAroundZTest()
        {
            var Vec = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue).ToVector();

            double RotationDegrees = _Faker.Random.Double(MinAngle, MaxAngle);
            double RotationZ = Functions.ToRadians(RotationDegrees);
            var RotationMatrix = Rotation.GenerateRotationAroundZ(RotationZ);

            var VecRotated = Vec * RotationMatrix;

            Assert.NotEqual(Vec[0], VecRotated[0]);
            Assert.NotEqual(Vec[1], VecRotated[1]);
            Assert.Equal(Vec[2], VecRotated[2]);
        }
    }
}
