using System.Linq;

using Bogus;

namespace Themis.Index.Tests;

internal static class TestExtensions
{
    internal static double[] RandomDoubleArray(this Faker f, int count, double min, double max)
    {
        return Enumerable.Range(0, count)
                         .Select(i => f.Random.Double(min, max))
                         .ToArray();
    }
}