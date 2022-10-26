using Bogus;
using Xunit;
using Assert = Xunit.Assert;

namespace Themis.Geometry.Tests
{
    public class ExtensionTests
    {
        const int Dimensions = 3;
        const double TestValue = -1.0;
        const double MinValue = 0.0;
        const double MaxValue = 1000.0;

        Faker _Faker;

        public ExtensionTests()
        {
            _Faker = new Faker();
        }

        [Fact]
        public void ToVectorTest()
        {
            var Actual = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue);

            var Vector = Actual.ToVector();

            Assert.Equal(Actual[0], Vector[0]);
            Assert.Equal(Actual[1], Vector[1]);
            Assert.Equal(Actual[2], Vector[2]);
        }

        [Fact]
        public void CloneModifyTest()
        {
            var posA = _Faker.RandomDoubleArray(Dimensions, MinValue, MaxValue)
                             .ToVector();

            var posB = posA.CloneModify(0, TestValue);

            Assert.NotEqual(posA[0], posB[0]);
            Assert.Equal(TestValue, posB[0]);
            Assert.Equal(posA[1], posB[1]);
            Assert.Equal(posA[2], posB[2]);
        }
    }
}
