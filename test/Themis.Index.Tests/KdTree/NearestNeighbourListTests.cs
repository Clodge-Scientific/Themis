using System;
using System.Linq;

using Xunit;
using Assert = Xunit.Assert;

using Themis.Index.KdTree;

namespace Themis.Index.Tests.KdTree;

public class NearestNeighbourListTests : IDisposable
{
    private NearestNeighbourList<Planet, float>? NearestNeighbours;

    private const int MaximumCapacity = 5;
    private static readonly FloatMath TypeMath = new();

    public NearestNeighbourListTests()
    {
        NearestNeighbours = new NearestNeighbourList<Planet, float>(TypeMath, MaximumCapacity);
    }

    void AddItems()
    {
        if (NearestNeighbours == null) throw new ArgumentNullException(nameof(NearestNeighbours));

        foreach (var planet in TestItems.Planets)
        {
            NearestNeighbours.Add(planet, planet.DistanceFromEarth);
        }
    }

    [Fact]
    public void TestAddAndCount()
    {
        if (NearestNeighbours == null) throw new ArgumentNullException(nameof(NearestNeighbours));

        AddItems();

        Assert.Equal(MaximumCapacity, NearestNeighbours.Count);
        Assert.True(NearestNeighbours.IsAtCapacity);
    }

    [Fact]
    public void TestRemoveFurthest()
    {
        if (NearestNeighbours == null) throw new ArgumentNullException(nameof(NearestNeighbours));

        AddItems();

        //< Get the five planets nearest to Earth from the test Planets
        var planetsByDist = TestItems.Planets.OrderBy(p => p.DistanceFromEarth)
                                               .Take(MaximumCapacity)
                                               .OrderByDescending(p => p.DistanceFromEarth)
                                               .ToArray();

        //< Ensure planets are grabbed in the correct order when fetching by furthest distance
        foreach (int index in Enumerable.Range(0, planetsByDist.Length))
        {
            var ExpectedPlanet = planetsByDist[index];
            var ActualPlanet = NearestNeighbours.RemoveFurthest();

            Assert.Equal(ExpectedPlanet.Name, ActualPlanet.Name);
            Assert.Equal(ExpectedPlanet.DistanceFromEarth, ActualPlanet.DistanceFromEarth);
        }

        //< NN list should be empty after prior loop
        Assert.Equal(0, NearestNeighbours.Count);
    }

    public void Dispose()
    {
        NearestNeighbours = null;
    }
}