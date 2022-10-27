# Themis.Index
- [Themis.Index](#themisindex)
    - [Overview](#overview)
- [Themis.Index.KdTree](#themisindexkdtree)
    - [Construction](#kdtree-construction)
    - [Search](#kdtree-search)
    - [Balancing](#kdtree-balancing)
- [Themis.Index.QuadTree](#themisindexquadtree)
    - [Construction](#quadtree-construction)
    - [Search / Query](#quadtree-search--query)

## Overview
This project encompasses the core spatial indexing functionality that allowed for efficient in-memory storage and retrieval of common geospatial datatypes via binary space partioning trees.

# Themis.Index.KdTree
This namespace exposes Themis' implementation of a [K-D Tree](https://en.wikipedia.org/wiki/K-d_tree) (a binary space partitioning tree) that can be used for fast and efficient radial & nearest neighbour searches of point geometries.  The key advantage of a K-D Tree is that it has an average complexity of O(logn) for Insert, Search, and Delete which is ideal for spatial datasets of significant scale and variable dimensionality.

A key note is that we've implemented this structure as `KdTree<TKey, TValue>` so that consumers can store any desired `TValue` type as long as they can compose an associated point geometry of form `IEnumerable<TKey>` to store within the tree.

Further to that point - each `KdTree` must be given a `ITypeMath<TKey>` that defines how we should compare the point geometries stored within the `KdTree`.  This is done so that we can support storing point geometries in both Spherical and Euclidean coordinate systems.  As of now, we current support the following `ITypeMath<TKey>` implementations:
- DoubleMath (`TypeMath<double>` - Euclidean)
- FloatMath (`TypeMath<float>` - Euclidean)
- GeographicMath (`TypeMath<float>` - Spherical)

## KdTree Construction
When constructing a new `KdTree` we always have to specify both the dimensionality but also the intended `ITypeMath` to be applied as follows:
```csharp
int dimensions = 2;
FLoatMath math = new();

var tree = new KdTree<float, string>(dimensions, typeMath); 
```
Now, the `KdTree` also allows for a few different ways of handling the case when the client attempts to insert a duplicate point geometry into the `KdTree`.  Currently we support the following behaviours:
- `AddDuplicateBehaviour.Skip`
    - Don't add or change anything, simply skip the 'new' point
- `AddDuplicateBehaviour.Error`
    - Throw a `DuplicateNodeError` when attempting to add a duplicate point
- `AddDuplicateBehaviour.Update`
    - Replace the existing `TValue` with the new `TValue`

Here's how to specify the intended behaviour for a given `KdTree`:
```csharp
var tree = new KdTree<float, string>(dimensions, typeMath, AddDuplicateBehavior.Skip);
```
With that construction effort out of the way - you can now actually fill your tree out as follows:
```csharp
internal record struct PointRecord(float[] Position, string Name);
var points = new PointRecord[] { .. };
// Adding points into the tree
foreach (var point in points)
{
    tree.Add(point.Position, point.Name); 
}
// Removing an existing point from the tree
tree.Remove(point.Position);
// Clearing all currently stored nodes
tree.Clear();
```
## KdTree Search
So, assuming you've already got a `KdTree` built and want to actually find something that's stored within it - you've got a few options:
- Search for a specific `IEnumerable<TKey>` position and return the `TValue` stored there
- Search for a specific `TValue` and return its `IEnumerable<TKey>` position
- Search for N (configurable number) of nearest-neighbours
- Search for points around a given `IEnumerable<TKey>` position within a specified radial distance

Here's a few examples:
```csharp
// Attempt to find a value at a given position
if (tree.FindValueAt(point.Position, out value))
{
    Console.WriteLine($"Found value: {value}");
}
// Attempt to find a position for a given value
if (tree.FindValue(point.Value, out position))
{
    Console.WriteLine($"Found position: {position.ToArray()}");
}
// Perform a radial search for all points w/in 5.0 units of the given position
foreach (var point in tree.RadialSearch(point.Position, 5.0)) { .. }
// Perform a search for the three nearest neighbours to a given position
foreach (var neigh in tree.GetNearestNeighbours(point.Position, 3)) { .. }
```
## KdTree Balancing
Now, our `KdTree` is not self-balancing during construction and as such can fall victim to becoming un-balanced based on the input data.  It does, however, have a simple method that can be called to re-construct the tree in a balanced state as follows:
```csharp
// Construct the tree
var tree = new KdTree<float, string>(dimensions, typeMath); 
// Adding points into the tree
var points = new PointRecord[] { .. };
foreach (var point in points) { tree.Add(point.Position, point.Name); }
// Balance the tree
tree.Balance();
```
The call to `tree.Balance()` will edit the `KdTree` in-place and will ensure that your searches are as optimized as possible given the input dataset.

# Themis.Index.QuadTree
This namespace exposes Themis' implemetation of a [QuadTree](https://en.wikipedia.org/wiki/Quadtree) (a 2D binary space partitioning tree) that can be used for fast and efficient storage and searching of geometries that can be represented by a 2D `IBoundingBox`.  QuadTrees are particularly useful for image processing, mesh generation, and surface collision detection.

This implementation is actually technically a 'Q-Tree' as we're able to specify the maximum capacity of each `QuadTreeNode` prior to splitting into child nodes - but by default the maximum capacity is 8.

A key note is that our `QuadTree<T>` implementation allows for storage of any object as long as they can be given an `IBoundingBox` envelope when inserted into the tree.

## QuadTree Construction
Unlike the `KdTree` we don't have to specify anything beyond what type is being stored within the tree and (optionally) how many items are allowed per `QuadTreeNode<T>` prior to split.

Here's an example:
```csharp
int maxItems = 16;
// Both trees are empty and store Triangles - but TreeB has a maximum node capacity of 16 
var TreeA = new QuadTree<Triangle>();
var TreeB = new QuadTree<Triangle>(maxItems);
```
It's also quite easy to add point geometries into `QuadTree<T>`'s by generating an `IBoundingBox` as follows:
```csharp
var tree = new QuadTree<Vector<double>>();
// Point geometries are supported - just need to compose an IBoundingBox envelope
var point = new double[] { .. }.ToVector();
var env = BoundingBox.FromPoint(point[0], point[1], BoundingBox.SinglePointBuffer);
// Add to tree
tree.Add(point, env);
```
Now, let's say you've got a collection of `Triangle` geometries that represent a 3D ground surface & you want to perform efficient collision detection.  Here's how you'd build the tree:
```csharp
var tree = new QuadTree<Triangle>();
// Add each Triangle using the existing IBoundingBox envelope
foreach (var triangle in Triangles) { tree.Add(triangle, triangle.Envelope); }
```
## QuadTree Search / Query
The `QuadTree<T>` has more rudimentary 'search' functionality when compared to the `KdTree<TKey, TValue>` but is still quite powerful.  Consumers can query a given `QuadTree<T>` as follows:
- Get all contained elements that intersect with a given 2D (X, Y) position
- Get all contained elements that intersect with a given `IBoundingBox`

__Note:__ Any given element could be stored in a root `QuadTreeNode<T>` as well as within one or more children `QuadTreeNode<T>` of that root node.  As such, calls to `QueryDistinct()` are guaranteed to return each element only once, whereas `QueryNonDistinct()` can contain duplicates.

Building on our above example with the `QuadTree<Triangle>`, here's how we'd query the tree to find the Triangle that covers a given position:
```csharp
var tree = new QuadTree<Triangle>();
// Add each Triangle using the existing IBoundingBox envelope
foreach (var triangle in Triangles) { tree.Add(triangle, triangle.Envelope); }
// The point-of-interest (POI) to check
var poi = new double[] { .. }.ToVector();
// Get all candidate Triangles, filter to explicitly covering, then select the first (or default)
var covering = tree.QueryDistinct(poi[0], poi[1])
                   .Where(t => t.Contains(poi))
                   .FirstOrDefault();
```
As you can see - there are two steps to consider when querying the `QuadTree<T>`:
-  Getting all potential candidates
-  Filtering down to specific target

Now, let's say we instead wanted to get all Triangles within a specified 2D search distance around a given POI:
```csharp
// The 2D search distance
double searchDistance = 50.0;
// Assuming tree is built as above - compose the POI and associated AOI
var poi = new double[] { .. }.ToVector();
var queryAoi = BoundingBox.FromPoint(poi[0], poi[1], searchDistance);
// Get all elements that overlap with the given AOI
var candidates = tree.QueryDistinct(queryAoi);
```