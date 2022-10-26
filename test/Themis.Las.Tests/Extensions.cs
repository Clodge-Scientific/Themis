using System.Linq;

using Bogus;
using MathNet.Numerics.LinearAlgebra;

namespace Themis.Las.Tests;

internal static class Extensions
{
    internal static double[] RandomDoubleArray(this Faker f, int count, double min, double max)
    {
        return Enumerable.Range(0, count)
                         .Select(i => f.Random.Double(min, max))
                         .ToArray();
    }

    internal static Vector<double> RandomPosition(this Faker f, int count, double min, double max)
    {
        return CreateVector.Dense(f.RandomDoubleArray(count, min, max));
    }
}