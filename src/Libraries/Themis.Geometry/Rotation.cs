using MathNet.Numerics.LinearAlgebra;

namespace Themis.Geometry
{
    public static class Rotation
    {
        const int Dimensions = 3;

        /// <summary>
        /// Generate a [3, 3] Matrix that will apply a rotation based on the input 3D rotation vector
        /// </summary>
        /// <param name="rph"></param>
        /// <returns>Fully composed [3, 3] Rotation Matrix</returns>
        public static Matrix<double> GenerateRotationMatrix(Vector<double> rph)
        {
            if (rph.Count < Dimensions) throw new ArgumentException($"Cannot create 3D rotation with less than 3 dimensions.", nameof(rph));

            return GenerateRotationMatrix(rph[0], rph[1], rph[2]);
        }

        /// <summary>
        /// Generate a [3, 3] Matrix that will apply a rotation based on the input Roll (X), Pitch (Y), and Heading/Yaw (Z) angles
        /// </summary>
        /// <param name="r">Roll (X) angle - in radians</param>
        /// <param name="p">Pitch (Y) angle - in radians</param>
        /// <param name="h">Heading (Z) angle - in radians</param>
        /// <returns>Fully composed [3, 3] Rotation Matrix</returns>
        public static Matrix<double> GenerateRotationMatrix(double r, double p, double h)
        {
            double cos_x = Math.Cos(r); double sin_x = Math.Sin(r);
            double cos_y = Math.Cos(p); double sin_y = Math.Sin(p);
            double cos_z = Math.Cos(h); double sin_z = Math.Sin(h);

            //< Generate the 9 rotational matrix elements
            double[] values = new double[]
            {
                cos_y * cos_z,
                cos_y * sin_z,
                -sin_y,

                sin_x * sin_y * cos_z - cos_x * sin_z,
                sin_x * sin_y * sin_z + cos_x * cos_z,
                cos_y * sin_x,

                cos_x * sin_y * cos_z + sin_x * sin_z,
                cos_x * sin_y * sin_z - sin_x * cos_z,
                cos_y * cos_x
            };

            //< Build the [3, 3] rotation matrix
            return Matrix<double>.Build.Dense(Dimensions, Dimensions, values);
        }

        /// <summary>
        /// Generate a [3, 3] Matrix that will apply a rotation around the X-axis with a magnitude of the input angle (in radians)
        /// </summary>
        /// <param name="angleRadians">Input rotation magnitude (in radians)</param>
        /// <returns>Fully composed [3, 3] Rotation Matrix</returns>
        public static Matrix<double> GenerateRotationAroundX(double angleRadians)
        {
            double s = Math.Sin(angleRadians); 
            double c = Math.Cos(angleRadians);

            var mat = Matrix<double>.Build.Dense(Dimensions, Dimensions);

            mat[0, 0] = 1;
            mat[1, 1] = c;
            mat[1, 2] = -s;
            mat[2, 1] = s;
            mat[2, 2] = c;

            return mat;
        }

        /// <summary>
        /// Generate a [3, 3] Matrix that will apply a rotation around the Y-axis with a magnitude of the input angle (in radians)
        /// </summary>
        /// <param name="angleRadians">Input rotation magnitude (in radians)</param>
        /// <returns>Fully composed [3, 3] Rotation Matrix</returns>
        public static Matrix<double> GenerateRotationAroundY(double angleRadians)
        {
            double s = Math.Sin(angleRadians);
            double c = Math.Cos(angleRadians);

            var mat = Matrix<double>.Build.Dense(Dimensions, Dimensions);

            mat[0, 0] = c;
            mat[0, 2] = s;
            mat[1, 1] = 1;
            mat[2, 0] = -s;
            mat[2, 2] = c;

            return mat;
        }

        /// <summary>
        /// Generate a [3, 3] Matrix that will apply a rotation around the Z-axis with a magnitude of the input angle (in radians)
        /// </summary>
        /// <param name="angleRadians">Input rotation magnitude (in radians)</param>
        /// <returns>Fully composed [3, 3] Rotation Matrix</returns>
        public static Matrix<double> GenerateRotationAroundZ(double angleRadians)
        {
            double s = Math.Sin(angleRadians);
            double c = Math.Cos(angleRadians);

            var mat = Matrix<double>.Build.Dense(Dimensions, Dimensions);

            mat[0, 0] = c;
            mat[0, 1] = -s;
            mat[1, 0] = s;
            mat[1, 1] = c;
            mat[2, 2] = 1;

            return mat;
        }
    }
}
