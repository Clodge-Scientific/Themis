# Themis.Geometry.Triangles
This namespace exposes the `Triangle` class which is used to represent 2D/3D triangular geometries as defined by a set of three vector vertices.  Once created a `Triangle` exposes the following key functionality & fields:
- A collection of all edges as `LineSegments`
- A `BoundingBox` envelope of the `Triangle` geometry
- The geometry's [Normal Vector](https://mathworld.wolfram.com/NormalVector.html#:~:text=The%20normal%20vector%2C%20often%20simply,pointing%20normal%20are%20usually%20distinguished.)
- Methods to check if a given (X,Y) position is contained by the 2D projection of the `Triangle` geometry
- Methods to extract the elevation (Z) on the `Triangle` surface for a given (X,Y) position
## Usage
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