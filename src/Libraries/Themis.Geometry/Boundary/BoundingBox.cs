namespace Themis.Geometry.Boundary;

public class BoundingBox : IBoundingBox, IEquatable<BoundingBox>
{
    //< This is the allowable 'epsilon' (error) when checking intersection
    const double IntersectionEPS = 1E-7;

    public const double SinglePointBuffer = 2.0 * 1E-3;

    public double MinX { get; set; } = double.MaxValue;
    public double MinY { get; set; } = double.MaxValue;
    public double MinZ { get; set; } = double.MaxValue;

    public double MaxX { get; set; } = double.MinValue;
    public double MaxY { get; set; } = double.MinValue;
    public double MaxZ { get; set; } = double.MinValue;

    public double Width => (MaxX - MinX);
    public double Height => (MaxY - MinY);
    public double Depth => (MaxZ - MinZ);

    public double Area => Width * Height;
    public double Volume => Width * Height * Depth;

    public double CentroidX => GetCentroid(MinX, MaxX);
    public double CentroidY => GetCentroid(MinY, MaxY);
    public double CentroidZ => GetCentroid(MinZ, MaxZ);

    #region IBoundingBox Fluent Interface
    public IBoundingBox WithMinima(double minX, double minY, double minZ = double.NaN)
    {
        this.MinX = minX;
        this.MinY = minY;
        this.MinZ = minZ;

        return this;
    }
    public IBoundingBox WithMinima(IBoundingBox other)
    {
        this.MinX = other.MinX;
        this.MinY = other.MinY;
        this.MinZ = other.MinZ;

        return this;
    }

    public IBoundingBox WithMaxima(double maxX, double maxY, double maxZ = double.NaN)
    {
        this.MaxX = maxX;
        this.MaxY = maxY;
        this.MaxZ = maxZ;

        return this;
    }
    public IBoundingBox WithMaxima(IBoundingBox other)
    {
        this.MaxX = other.MaxX;
        this.MaxY = other.MaxY;
        this.MaxZ = other.MaxZ;

        return this;
    }

    public IBoundingBox WithZ(double minZ, double maxZ)
    {
        this.MinZ = minZ;
        this.MaxZ = maxZ;

        return this;
    }

    public IBoundingBox Buffer(double buffer)
    {
        return new BoundingBox().WithMinima(MinX - buffer, MinY - buffer, MinZ - buffer)
                                .WithMaxima(MaxX + buffer, MaxY + buffer, MaxZ + buffer);
    }

    public IBoundingBox ExpandToInclude(IBoundingBox that)
    {
        return new BoundingBox()
                    .WithMinima(Math.Min(this.MinX, that.MinX), Math.Min(this.MinY, that.MinY), Math.Min(this.MinZ, that.MinZ))
                    .WithMaxima(Math.Max(this.MaxX, that.MaxX), Math.Max(this.MaxY, that.MaxY), Math.Max(this.MaxZ, that.MaxZ));
    }
    #endregion

    #region Factory Methods
    /// <summary>
    /// Generate a new BoundingBox with the same Minima & Maxima of an existing BoundingBox
    /// </summary>
    /// <param name="other">Input BoundingBox to copy Maxima & Minima from</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IBoundingBox From(IBoundingBox other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        return new BoundingBox()
                    .WithMinima(other)
                    .WithMaxima(other);
    }

    /// <summary>
    /// Generate a new 2D BoundingBox with the specified X/Y Minima & Maxima
    /// </summary>
    /// <param name="minX">Minimum X-Coordinate</param>
    /// <param name="minY">Minimum Y-Coordinate</param>
    /// <param name="maxX">Maximum X-Coordinate</param>
    /// <param name="maxY">Maximum Y-Coordinate</param>
    /// <returns></returns>
    public static IBoundingBox From(double minX, double minY, double maxX, double maxY)
    {
        return new BoundingBox().WithMinima(minX, minY).WithMaxima(maxX, maxY);
    }

    /// <summary>
    /// Generate a new 3D BoundingBox with the specified X/Y/Z Minima & Maxima
    /// </summary>
    /// <param name="minX">Minimum X-Coordinate</param>
    /// <param name="minY">Minimum Y-Coordinate</param>
    /// <param name="minZ">Minimum Z-Coordinate</param>
    /// <param name="maxX">Maximum X-Coordinate</param>
    /// <param name="maxY">Maximum Y-Coordinate</param>
    /// <param name="maxZ">Maximum Z-Coordinate</param>
    /// <returns></returns>
    public static IBoundingBox From(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
    {
        return new BoundingBox().WithMinima(minX, minY, minZ).WithMaxima(maxX, maxY, maxZ);
    }

    /// <summary>
    /// Create a simple 2D 'buffered' bounding box around the specified (x, y) position
    /// </summary>
    /// <param name="x">Input position X</param>
    /// <param name="y">Input position Y</param>
    /// <param name="bufferValue">Scalar buffer value to be added to each dimension</param>
    /// <returns></returns>
    public static IBoundingBox FromPoint(double x, double y, double bufferValue)
    {
        //< Get the 'half-width' of thee buffer to be applied to both minima/maxima
        double halfWidth = bufferValue / 2.0;
        //< Generate and return the fully-formed BoundingBox
        return new BoundingBox().WithMinima(x - halfWidth, y - halfWidth)
                                .WithMaxima(x + halfWidth, y + halfWidth);
    }

    /// <summary>
    /// Create a simple 3D 'buffered' bounding cube around the specified (x, y, z) position
    /// </summary>
    /// <param name="x">Input position X</param>
    /// <param name="y">Input position Y</param>
    /// <param name="z">Input position Z</param>
    /// <param name="bufferValue">Scalar buffer value to be added to each dimension</param>
    /// <returns></returns>
    public static IBoundingBox FromPoint(double x, double y, double z, double bufferValue)
    {
        //< Get the 'half-width' of thee buffer to be applied to both minima/maxima
        double halfWidth = bufferValue / 2.0;
        //< Generate and return the fully-formed BoundingBox
        return new BoundingBox().WithMinima(x - halfWidth, y - halfWidth, z - halfWidth)
                                .WithMaxima(x + halfWidth, y + halfWidth, z + halfWidth);
    }
    #endregion

    public bool Contains(double x, double y, double z = double.NaN)
    {
        if (x < MinX || x > MaxX) return false;
        if (y < MinY || y > MaxY) return false;

        return double.IsNaN(z) || (z >= MinZ && z <= MaxZ);
    }

    public bool Intersects(IBoundingBox other)
    {
        //< Get the dimensional offsets between the two centroids and double them
        double dX = Math.Abs(CentroidX - other.CentroidX) * 2.0;
        double dY = Math.Abs(CentroidY - other.CentroidY) * 2.0;
        double dZ = Math.Abs(CentroidZ - other.CentroidZ) * 2.0;

        //< Check if any of the dimensional differences exceed the combined widths of the BoundingBoxes
        if (dX > (this.Width + other.Width + IntersectionEPS)) return false;
        if (dY > (this.Height + other.Height + IntersectionEPS)) return false;

        //< If one of the Z-value is NaN, we can't compare and need to revert to 2D intersection
        if (double.IsNaN(dZ)) return true;

        //< Perform the Z-range check for 3D and return true if we're safe
        return dZ < (this.Depth + other.Depth + IntersectionEPS);
    }

    /// <summary>
    /// Calculate the centroid of the given min/max dimensional values
    /// </summary>
    /// <param name="min">Minimum (smallest) value</param>
    /// <param name="max">Maximum (largest) value</param>
    /// <returns></returns>
    public static double GetCentroid(double min, double max)
    {
        return min + (max - min) / 2.0;
    }

    #region IEquatable
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        return Equals(obj as BoundingBox);
    }

    public bool Equals(BoundingBox? other)
    {
        return other != null &&
               MinX.Equals(other.MinX) &&
               MinY.Equals(other.MinY) &&
               MinZ.Equals(other.MinZ) &&
               MaxX.Equals(other.MaxX) &&
               MaxY.Equals(other.MaxY) &&
               MaxZ.Equals(other.MaxZ);
    }

    public override int GetHashCode()
    {
        if (double.IsNaN(CentroidZ)) return HashCode.Combine(MinX, MinY, MaxX, MaxY);

        return HashCode.Combine(MinX, MinY, MinZ, MaxX, MaxY, MaxZ);
    }
    #endregion
}