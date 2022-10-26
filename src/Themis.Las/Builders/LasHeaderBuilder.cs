namespace Themis.Las.Builders;

public class LasHeaderBuilder : ILasHeaderBuilder
{
    private LasHeader header = new();

    #region ILasHeaderBuilder Methods
    public ILasHeaderBuilder SetVersion(byte major, byte minor)
    {
        header.SetVersion(major, minor);
        return this;
    }
    public ILasHeaderBuilder SetVersionMajor(byte versionMajor)
    {
        header.SetVersion(versionMajor, header.VersionMinor);
        return this;
    }
    public ILasHeaderBuilder SetVersionMinor(byte versionMinor)
    {
        header.SetVersion(header.VersionMajor, versionMinor);
        return this;
    }

    public ILasHeaderBuilder SetMinima(IEnumerable<double> minima)
    {
        if (minima.Count() < 3) throw new ArgumentException($"Minima must be at least 3D");

        return SetMinima(minima.ElementAt(0), minima.ElementAt(1), minima.ElementAt(2));
    }
    public ILasHeaderBuilder SetMinima(double minX, double minY, double minZ)
    {
        header.SetMinima(minX, minY, minZ);
        return this;
    }
    public ILasHeaderBuilder SetMaxima(IEnumerable<double> maxima)
    {
        if (maxima.Count() < 3) throw new ArgumentException($"Maxima must be at least 3D");

        return SetMaxima(maxima.ElementAt(0), maxima.ElementAt(1), maxima.ElementAt(2));
    }
    public ILasHeaderBuilder SetMaxima(double maxX, double maxY, double maxZ)
    {
        header.SetMaxima(maxX, maxY, maxZ);
        return this;
    }
    public ILasHeaderBuilder SetScale(IEnumerable<double> scale)
    {
        if (scale.Count() < 3) throw new ArgumentException($"Scale must be at least 3D");

        return SetScale(scale.ElementAt(0), scale.ElementAt(1), scale.ElementAt(2));
    }
    public ILasHeaderBuilder SetScale(double scaleX, double scaleY, double scaleZ)
    {
        header.SetScale(scaleX, scaleY, scaleZ);
        return this;
    }
    public ILasHeaderBuilder SetOrigin(IEnumerable<double> origin)
    {
        if (origin.Count() < 3) throw new ArgumentException($"Scale must be at least 3D");

        return SetOrigin(origin.ElementAt(0), origin.ElementAt(1), origin.ElementAt(2));
    }
    public ILasHeaderBuilder SetOrigin(double originX, double originY, double originZ)
    {
        header.SetOrigin(originX, originY, originZ);
        return this;
    }

    public ILasHeaderBuilder SetNumberOfVLRs(uint numberOfVLRs)
    {
        header.SetNumVLRs(numberOfVLRs);
        return this;
    }
    public ILasHeaderBuilder SetOffsetToPointData(uint offset)
    {
        header.SetOffsetToPointData(offset);
        return this;
    }

    public ILasHeaderBuilder SetPointCount(ulong count)
    {
        header.SetPointCount(count);
        return this;
    }
    public ILasHeaderBuilder SetPointDataFormat(byte format)
    {
        header.SetPointDataFormat(format);
        return this;
    }

    public ILasHeaderBuilder SetCreationDate(ushort year, ushort doy)
    {
        header.SetCreationDate(year, doy);
        return this;
    }
    public ILasHeaderBuilder SetCreationYear(ushort year)
    {
        header.SetCreationDate(year, header.CreationDOY);
        return this;
    }
    public ILasHeaderBuilder SetCreationDayOfYear(ushort dayOfYear)
    {
        header.SetCreationDate(header.CreationYear, dayOfYear);
        return this;
    }
    #endregion

    #region IFluentBuilder<T> Methods
    public ILasHeader Build(bool reset = true)
    {
        LasHeader result = header;

        if (reset) Reset();

        return result;
    }

    public void Reset()
    {
        header = new();
    }
    #endregion
}