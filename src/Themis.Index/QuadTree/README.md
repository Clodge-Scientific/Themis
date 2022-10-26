# Themis.Geometry.Index.QuadTree
This namespace exposes Themis' implemetation of a [QuadTree](https://en.wikipedia.org/wiki/Quadtree) (a 2D binary space partitioning tree) that can be used for fast and efficient storage and searching of geometries that can be represented by a 2D `IBoundingBox`.  QuadTrees are particularly useful for image processing, mesh generation, and surface collision detection.

This implementation is actually technically a 'Q-Tree' as we're able to specify the maximum capacity of each `QuadTreeNode` prior to splitting into child nodes - but by default the maximum capacity is 8.

A key note is that our `QuadTree<T>` implementation allows for storage of any object as long as they can be given an `IBoundingBox` envelope when inserted into the tree.

## Construction
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
## Search / Query
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