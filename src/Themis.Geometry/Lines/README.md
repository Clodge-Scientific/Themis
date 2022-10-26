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
## Usage
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