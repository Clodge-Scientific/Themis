# Themis.Las

*Themis.Las* is a C# (.NET6) project that exposes APIs for working with LiDAR point cloud in the form of ASPRS LAS files.  It attempts to expose a simple interface where a consumer can iterate through a LAS file (of any format) point-by-point and perform any business logic they desire upon each point, and then output those points to any other format.

*Themis.Las.Time* contains a series of helper functions for handling common timestamp manipulation that often occurs when working with point cloud data.

## Usage

In order to parse a LAS file available on local disk, it can be done via the following:
```csharp
string lasFile = @"..\points.las";
using var reader = new LasReader(lasFile);
var lpt = new LasPoint();

//< Histogram to count number of points by classification
var hist = new uint[byte.MaxValue];
//< Iterate through all points in the file and update histogram
while (!reader.EOF)
{
    reader.GetNextPoint(ref lpt);
    hist[lpt.Classification]++;
}
```
One should note that the `LasPoint` class is used to represent a LAS PointRecord of ANY format with a few caveats:  
- The position values are the actual decimal coordinate rather than the integer values stored in the LAS file itself.  
- Only when writing back out to disk is the `LasPoint` converted into the necessary output struct.

Instantiating a single `LasPoint` first and then updating its fields with a call to `reader.GetNextPoint(ref lpt)` provides a significant performance boost when compared to created a new `LasPoint` for each new record as follows:

```csharp
using var reader = new LasReader(lasFile);
while (!reader.EOF)
{
    var lpt = reader.GetNextPoint();
    hist[lpt.Classification]++;
}
```