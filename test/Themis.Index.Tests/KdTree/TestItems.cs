using System.Collections.Generic;

using Themis.Index.KdTree;

namespace Themis.Index.Tests.KdTree;

internal struct Person
{
    public string Name;
    public float Age;
}

internal struct Planet
{
    public string Name;
    public float DistanceFromEarth;
}

internal struct City
{
    public string Address;
    public float Lat;
    public float Long;
    public float DistanceFromToowoomba;
}

internal static class TestItems
{
    public static readonly List<Person> People = new()
    {
        new Person { Name = "Chris", Age = 16.4f },
        new Person { Name = "Stewie", Age = 1.0f },
        new Person { Name = "Brian", Age = 10.1f },
        new Person { Name = "Meg", Age = 15.0f },
        new Person { Name = "Peter", Age = 41.999f },
        new Person { Name = "Lois", Age = 38.25f }
    };

    public static readonly List<Planet> Planets = new()
    {
        new Planet { Name = "Mercury", DistanceFromEarth = 91700000f },
        new Planet { Name = "Venus", DistanceFromEarth = 41400000f },
        new Planet { Name = "Mars", DistanceFromEarth = 78300000f },
        new Planet { Name = "Jupiter", DistanceFromEarth = 624400000f },
        new Planet { Name = "Saturn", DistanceFromEarth = 1250000000f },
        new Planet { Name = "Uranus", DistanceFromEarth = 2720000000f },
        new Planet { Name = "Neptune", DistanceFromEarth = 4350000000f }
    };

    public static readonly City Toowoomba = new City
    {
        Address = "Toowoomba, QLD, Australia",
        Lat = -27.5829487f,
        Long = 151.8643252f,
        DistanceFromToowoomba = 0
    };

    public static readonly List<City> Cities = new()
    {
        Toowoomba,
        new City()
        {
            Address = "Brisbane, QLD, Australia",
            Lat = -27.4710107f,
            Long = 153.0234489f,
            DistanceFromToowoomba = 1.16451615177537f
        },
        new City()
        {
            Address = "Goldcoast, QLD, Australia",
            Lat = -28.0172605f,
            Long = 153.4256987f,
            DistanceFromToowoomba = 1.6206523211724f
        },
        new City()
        {
            Address = "Sunshine, QLD, Australia",
            Lat = -27.3748288f,
            Long = 153.0554193f,
            DistanceFromToowoomba = 1.20913979664506f
        },
        new City()
        {
            Address = "Melbourne, VIC, Australia",
            Lat = -37.814107f,
            Long = 144.96328f,
            DistanceFromToowoomba = 12.3410301438779f
        },
        new City()
        {
            Address = "Sydney, NSW, Australia",
            Lat = -33.8674869f,
            Long = 151.2069902f,
            DistanceFromToowoomba = 6.31882185929341f
        },
        new City()
        {
            Address = "Perth, WA, Australia",
            Lat = -31.9530044f,
            Long = 115.8574693f,
            DistanceFromToowoomba = 36.2710774395312f
        },
        new City()
        {
            Address = "Darwin, NT, Australia",
            Lat = -12.4628198f,
            Long = 130.8417694f,
            DistanceFromToowoomba = 25.895292049265f
        }
    };

    public static List<KdTreeNode<float, string>> Nodes = new()
    {
        new KdTreeNode<float, string>(new float[] { 5, 5 }, "Root"),
        new KdTreeNode<float, string>(new float[] { 2.5f, 2.5f }, "Root-Left"),
        new KdTreeNode<float, string>(new float[] { 7.5f, 7.5f }, "Root-Right"),
        new KdTreeNode<float, string>(new float[] { 1, 10 }, "Root-Left-Left"),
        new KdTreeNode<float, string>(new float[] { 10, 10 }, "Root-Right-Right")
    };
}