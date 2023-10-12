namespace Themis.Geometry.Lines;

public static class LineSegmentIntersection
{
    /// <summary>
    /// Find and return the (2D) point-of-intersection if it exists - otherwise return null
    /// </summary>
    /// <param name="lineA">First ILineSegment to compare</param>
    /// <param name="lineB">Second ILineSegment to compare</param>
    /// <param name="precision">Integer precision tolerance</param>
    /// <returns>The (2D) point-of-intersection if it exists - otherwise null</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static Vector<double>? Find2D(ILineSegment lineA, ILineSegment lineB, int precision = 5)
    {
        var tolerance = Math.Round(Math.Pow(0.1, precision), precision);
        return Find2D(lineA, lineB, tolerance);
    }

    /// <summary>
    /// Find and return the (2D) point-of-intersection if it exists - otherwise return null
    /// </summary>
    /// <param name="lineA">First ILineSegment to compare</param>
    /// <param name="lineB">Second ILineSegment to compare</param>
    /// <param name="tolerance">Decimal precision tolerance to be applied</param>
    /// <returns>The (2D) point-of-intersection if it exists - otherwise null</returns>
    /// <exception cref="Exception">If both input lines are the equal</exception>
    public static Vector<double>? Find2D(ILineSegment lineA, ILineSegment lineB, double tolerance)
    {
        //< See: https://github.com/justcoding121/Advanced-Algorithms/blob/develop/src/Advanced.Algorithms/Geometry/LineIntersection.cs
        if (lineA == lineB) throw new Exception("Cannot intersect two equal lines, kthx.");

        (var left, var right) = OrderSegmentsLeftRight(lineA, lineB);

        double x1 = left.A[0], y1 = left.A[1];
        double x2 = left.B[0], y2 = left.B[1];
        double x3 = right.A[0], y3 = right.A[1];
        double x4 = right.B[0], y4 = right.B[1];

        //< In the event of two vertical, overlapping lines
        if (x1 == x2 && x3 == x4 && x1 == x3)
        {
            var pos = new double[2] { x1, y3 }.ToVector();
            if (IsInsideLine(left, pos, tolerance) && IsInsideLine(right, pos, tolerance)) return pos;
        }

        //< In the event of two horizontal, overlapping lines
        if (y1 == y2 && y3 == y4 && y1 == y3)
        {
            var pos = new double[2] { x3, y3 }.ToVector();
            if (IsInsideLine(left, pos, tolerance) && IsInsideLine(right, pos, tolerance)) return pos;
        }

        //< Two non-overlapping, vertical lines have no intersection
        if (x1 == x2 && x3 == x4) return null;
        //< Two non-overlapping, horizontal lines have no intersection
        if (y1 == y2 && y3 == y4) return null;

        double x, y;

        //< Handle vertical 'left' line
        if (Math.Abs(x1 - x2) < tolerance)
        {
            var m2 = (y4 - y3) / (x4 - x3);
            var c2 = -m2 * x3 + y3;
            x = x1;
            y = c2 + m2 * x1;
        }
        //< Handle vertical 'right' line
        else if (Math.Abs(x3 - x4) < tolerance)
        {
            var m1 = (y2 - y1) / (x2 - x1);
            var c1 = -m1 * x1 + y1;
            x = x3;
            y = c1 + m1 * x3;
        }
        //< Lines are not vertical
        else
        {
            //< Compute slope of line 1(m1) and c2
            var m1 = (y2 - y1) / (x2 - x1);
            var c1 = -m1 * x1 + y1;

            //< Compute slope of line 2 (m2) and c2
            var m2 = (y4 - y3) / (x4 - x3);
            var c2 = -m2 * x3 + y3;

            //< Solving equations (3) and (4) => x = (c1-c2)/(m2-m1)
            //<  - By plugging x value in equation (4) => y = c2 + m2 * x
            x = (c1 - c2) / (m2 - m1);
            y = c2 + m2 * x;

            //< Verify intersection by plugging back into (1) and (2) - return null if this fails
            if (!(Math.Abs(-m1 * x + y - c1) < tolerance && Math.Abs(-m2 * x + y - c2) < tolerance))
                return null;
        }

        //< Generate result and ensure it lies within the LineSegment(s)
        var res = new double[2] { x, y }.ToVector();
        if (IsInsideLine(left, res, tolerance) && IsInsideLine(right, res, tolerance)) return res;

        //< No intersection - return null;
        return null;
    }

    static (ILineSegment left, ILineSegment right) OrderSegmentsLeftRight(ILineSegment A, ILineSegment B)
    {
        var lineA = LineSegment.GenerateOrdered(A);
        var lineB = LineSegment.GenerateOrdered(B);

        if (lineA.A[0].CompareTo(lineB.A[0]) > 0)
        {
            return (B, A);
        }
        else if (lineA.A[0].CompareTo(lineB.A[0]) == 0)
        {
            if (lineA.A[1].CompareTo(lineB.A[1]) > 0) return (B, A);
        }

        return (A, B);
    }

    static bool IsInsideLine(ILineSegment line, Vector<double> pos, double tolerance)
    {
        if (pos.Count < 2) throw new ArgumentException("Input position must be at least 2D!");

        double x = pos[0], y = pos[1];

        var leftX = line.A[0];
        var leftY = line.A[1];

        var rightX = line.B[0];
        var rightY = line.B[1];

        return (x.IsGreaterThanOrEqual(leftX, tolerance) && x.IsLessThanOrEqual(rightX, tolerance)
                || x.IsGreaterThanOrEqual(rightX, tolerance) && x.IsLessThanOrEqual(leftX, tolerance))
               && (y.IsGreaterThanOrEqual(leftY, tolerance) && y.IsLessThanOrEqual(rightY, tolerance)
                   || y.IsGreaterThanOrEqual(rightY, tolerance) && y.IsLessThanOrEqual(leftY, tolerance));
    }
}

/// <summary>
/// Extension methods for the LineSegment class
/// </summary>
public static class LineSegmentExtensions
{
    public static bool Intersects(this ILineSegment A, ILineSegment B, int precision = 5)
        => LineSegmentIntersection.Find2D(A, B, precision) != null;

    public static Vector<double>? Intersection(this ILineSegment A, ILineSegment B, int precision = 5)
        => LineSegmentIntersection.Find2D(A, B, precision);

    internal static bool Intersects(this ILineSegment A, ILineSegment B, double tolerance)
        => LineSegmentIntersection.Find2D(A, B, tolerance) != null;

    internal static Vector<double>? Intersection(this ILineSegment A, ILineSegment B, double tolerance)
        => LineSegmentIntersection.Find2D(A, B, tolerance);

    internal static bool IsGreaterThanOrEqual(this double a, double b, double tolerance)
    {
        var result = a - b;
        return result > tolerance || Math.Abs(result) < tolerance;
    }

    internal static bool IsLessThanOrEqual(this double a, double b, double tolerance)
    {
        var result = a - b;
        return result < -tolerance || Math.Abs(result) < tolerance;
    }
}
