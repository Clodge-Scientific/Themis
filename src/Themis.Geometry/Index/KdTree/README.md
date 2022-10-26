# Themis.Geometry.Index.KdTree
This namespace exposes Themis' implementation of a [K-D Tree](https://en.wikipedia.org/wiki/K-d_tree) (a binary space partitioning tree) that can be used for fast and efficient radial & nearest neighbour searches of point geometries.  The key advantage of a K-D Tree is that it has an average complexity of O(logn) for Insert, Search, and Delete which is ideal for spatial datasets of significant scale and variable dimensionality.

A key note is that we've implemented this structure as `KdTree<TKey, TValue>` so that consumers can store any desired `TValue` type as long as they can compose an associated point geometry of form `IEnumerable<TKey>` to store within the tree.

Further to that point - each `KdTree` must be given a `ITypeMath<TKey>` that defines how we should compare the point geometries stored within the `KdTree`.  This is done so that we can support storing point geometries in both Spherical and Euclidean coordinate systems.  As of now, we current support the following `ITypeMath<TKey>` implementations:
- DoubleMath (`TypeMath<double>` - Euclidean)
- FloatMath (`TypeMath<float>` - Euclidean)
- GeographicMath (`TypeMath<float>` - Spherical)
## Construction
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
## Search
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
## Balancing
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