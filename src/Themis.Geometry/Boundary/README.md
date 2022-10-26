# Themis.Geometry.Boundary
This namespace exposes the `BoundingBox` class which represents both 2D BoundingBox and 3D BoundingCube geometries within a single concrete class.  In both dimensional cases, the `BoundingBox` is defined by its local __Minima (X,Y,Z)__ and __Maxima (X,Y,Z)__.

__Note:__  When in 2D - the Minima/Maxima Z coordinates are left as double.NaN.

Further, the `BoundingBox` implementation is at the core of the spatial indexing functionality provided by the [QuadTree](#indexquadtree).

## Usage
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