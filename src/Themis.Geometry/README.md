# Themis.Geometry
- [Themis.Geometry](#themisgeometry)
    - [Overview](#overview)
- [Boundary](#themisgeometryboundary)
- [Lines](#themisgeometrylines)
- [Triangles](#themisgeometrytriangles)

## Overview
This project encompasses the core geometriic object models and funcitonality that allow for efficient and reliable representation, manipulation, and analysis of common geospatial data types.

# Themis.Geometry.Boundary
This namespace exposes the `BoundingBox` class which represents both 2D BoundingBox and 3D BoundingCube geometries within a single concrete class.  In both dimensional cases, the `BoundingBox` is defined by its local __Minima (X,Y,Z)__ and __Maxima (X,Y,Z)__.

__Note:__  When in 2D - the Minima/Maxima Z coordinates are left as double.NaN.

Further, the `BoundingBox` implementation is at the core of the spatial indexing functionality provided by the [QuadTree](#indexquadtree).

## Boundary Usage
While the current `BoundingBox` implementation only has a default constructor it also exposes a number of Factory methods as well as a Fluent interface to generate the desired resultant geometry.

Consider the following:
```csharp

// Create a 2D BoundingBox centered at (1, 1) with minima (-1.5, -1.5) and maxima (3.5, 3.5)
var box = BoundingBox.FromPoint(1.0, 1.0, 5.0);
var box2 = BoundingBox.From(-1.5, -1.5, 3.5, 3.5);
// Resulting Minima & Maxima are equal - thus the two BoundingBoxes are equal
Assert.Equal(box, box2);
Assert.True(box.Contains(1.0, 1.0)); 

// Creates a new 3D BoundingBox with the specified elevation (Z) range
var cube = BoundingBox.From(box).WithZ(0, 100.0);
// The 3D BoundingBox can still perform both 2D & 3D containment checks
Assert.True(cube.Contains(1.0, 1.0));
Assert.True(cube.Contains(1.0, 1.0, 1.0));
```
We can also easily expand a given `BoundingBox` to include any other `BoundingBox` or simply alter the local Minima / Maxima as follows:
```csharp
// Create two BoundingBoxes that overlap and then create a 3rd that include them both
var A = BoundingBox.From(-1.5, -1.5, 3.5, 3.5);
var B = BoundingBox.From(0, 0, 5.0, 5.0);
// Can check that the two BoundingBoxes intersect/overlap as well
Assert.True(A.Intersects(B));
// Lets create a new BoundingBox by expanding A to include B
var C = A.ExpandToInclude(B);
// Can now see the maxima & minima are expanded as expected
Assert.Equal(5.0, C.MaxX);
Assert.Equal(5.0, C.MaxY);
```
Beyond the above functionality - each `BoundingBox` also exposes the following fields:
- Width (MaxX - MinX)
- Height (MaxY - MinY)
- Depth (MaxZ - MinZ)
- Area (Width * Height)
- Volume (Width * Height * Depth)
- (X,Y,Z) Coordinates of the Centroid

# Themis.Geometry.Lines
This namespace exposes Themis' `LineSegment` and `LineString` implementations that are intended to be used to model 2/3D linear geometries.  While both implementations will technically function within any dimension it's recommended consumers limit their dimensionality to 2/3D.

The `LineSegment` is composed of an ordered pair of vector vertices (named A & B) and encompasses both the infinite line between A->B but also the discrete `LineSegment` formed by A->B.  Given those two components the `LineSegment` is able to efficiently do the following:
- Get the station (distance along line from A->B) of any input position
- Extract a point along the `LineSegment` at any station (distance along the line)
- Extract the nearest point on the `LineSegment` to any input position
- Get the minimum distance to the `LineSegment` from any input position

The `LineString` is composed of two or more vertices as an ordered, connected, set of linear geometries.  In order to build the `LineString` we create a `LineSegment` between each vertex and the following vertex (excluding the final vertex). While in 2D this could represent the connectivity map of power poles, the 3D extension can be used to model wire geometries or other complex shapes composed of many lines.  Given this, the `LineString` exposes the following functionality:
- Extract the nearest point from all contained `LineSegment` geometries to any input position
- Calculate total 2D & 3D Length of all contained `LineSegment` geometries
## Lines Usage
In order to instantiate a `LineSegment` we'll need to first have two [Vectors](https://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra/Vector%601.htm) that represent the starting Vertex (A) and the terminating Vertex (B).  

__Note:__ We've also included some extension methods to easily convert a given `IEnumerable<T>` into a `Vector<T>` that simplifies this.

Here's an example:

```csharp
// Need to instantiate the two vertices (0,0,0) and (5,5,5)
var A = new double[] { 0.0, 0.0, 0.0 }.ToVector();
var B = new double[] { 5.0, 5.0, 5.0 }.ToVector();
// Generate the LineSegment A->B from (0,0,0) to (5,5,5)
var line = new LineSegment(A, B);

// Get the 3D and 2D length of the LineSegment
double len = line.Length;
double len2D = line.Length2D;
Assert.NotEqual(len, len2D); // True
```
Another key example is when you need to find the nearest point (from a collection of points) to a given `LineSegment`:
```csharp
// A collection of position vectors
var points = new List<Vector<double>>() { .. };
// Since this is ascending by default, can take the 'first' element as nearest
var nearest = points.OrderyBy(p => line.DistanceToPoint(p)).First();
```
Or the inverse - given a collection of `LineSegments` find the one nearest a given point of interest:
```csharp
// A single query POI
var point = new double[] { .. }.ToVector();
// A collection of LineSegments
var segs = new List<LineSegment>() { .. };
// Get the LineSegment closest to the input POI
var nearestSeg = segs.OrderBy(s => s.DistanceToPoint(point)).First();
var nearestPoint = nearestSeg.GetClosestPoint(point);
```

# Themis.Geometry.Triangles
This namespace exposes the `Triangle` class which is used to represent 2D/3D triangular geometries as defined by a set of three vector vertices.  Once created a `Triangle` exposes the following key functionality & fields:
- A collection of all edges as `LineSegments`
- A `BoundingBox` envelope of the `Triangle` geometry
- The geometry's [Normal Vector](https://mathworld.wolfram.com/NormalVector.html#:~:text=The%20normal%20vector%2C%20often%20simply,pointing%20normal%20are%20usually%20distinguished.)
- Methods to check if a given (X,Y) position is contained by the 2D projection of the `Triangle` geometry
- Methods to extract the elevation (Z) on the `Triangle` surface for a given (X,Y) position
## Triangles Usage
As mentioned above - in order to generate a `Triangle` we'll need to have a collection of vertices that define the `Triangle` geometry.  Here's an example:
```csharp
// Forming Triangle (0, 0, 0) -> (1, 0, 1) -> (0, 1, 0)
var A = new double[] { 0.0, 0.0, 0.0 }.ToVector();
var B = new double[] { 1.0, 0.0, 1.0 }.ToVector();
var C = new double[] { 0.0, 1.0, 0.0 }.ToVector();
// Generate the Triangle object
var Triangle = new Triangle(new() { A, B, C });
```
Now with the `Triangle` defined, we can check for containment of any given position and then sample its elevation on the `Triangle` surface as follows:
```csharp
// Input POI's elevation doesn't matter for containment or Z-sampling
var pos = new double[] {0.25, 0.25, double.NaN}.ToVector();
// Checking containment & extract elevation (Z)
if(Triangle.Contains(pos))
{
    double Z = Triangle.GetZ(pos); // 0.25
}
```