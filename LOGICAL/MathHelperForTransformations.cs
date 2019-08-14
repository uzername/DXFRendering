using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFRendering.LOGICAL
{
    /// <summary>
    /// A crude way how to perform transformations.
    /// important thing is how to rotate point: https://stackoverflow.com/q/2259476/
    /// </summary>
    public static class MathHelperForTransformations
    {
        public static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }
        public static double ConvertRadiansToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }
        // https://www.geeksforgeeks.org/find-mirror-image-point-2-d-plane/
        public static Tuple<double, double> mirrorImage(double a, double b, double c, double x1, double y1)
        {
            double temp = -2 * (a * x1 + b * y1 + c) / (a * a + b * b);
            double x = temp * a + x1;
            double y = temp * b + y1;
            return new Tuple<double, double>(x, y);
        }
        public static Tuple<double, double> mirrorImage(double verticalMirrorX, double x1, double y1)
        {
            double deltaMirror = Math.Abs(verticalMirrorX - x1);
            double x = x1; double y = y1;
            if (verticalMirrorX > x1)
            {
                x = x1 + 2 * deltaMirror;
            }
            else
            {
                x = x1 - 2 * deltaMirror;
            }
            return new Tuple<double, double>(x, y);
        }

 
        /// <summary>
        /// multiply two 2d matrices 
        /// <seealso cref="http://dev.bratched.fr/en/fun-with-matrix-multiplication-and-unsafe-code/"/> ;;
        /// <seealso cref="https://stackoverflow.com/questions/6311309/how-can-i-multiply-two-matrices-in-c"/> 
        /// </summary>
        /// <returns></returns>
        public static double[,] CrudeMultiplication2(double[,] m1, double[,] m2, int m1_rows, int m1_cols, int m2_rows, int m2_cols)
        {
            if (m1_cols != m2_rows)
            {
                return null;
            }
            int resultMatrixHeight = m1_rows; int resultMatrixWidth = m2_cols;
            double[,] resultMatrix = new double[resultMatrixHeight, resultMatrixWidth];

            for (int i = 0; i < resultMatrixHeight; i++)
            {
                for (int j = 0; j < resultMatrixWidth; j++)
                {
                    resultMatrix[i, j] = 0;
                    for (int k = 0; k < m1_cols; k++)
                    {
                        resultMatrix[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }

            return resultMatrix;
        }

        public static double[,] getRotationMatrixAroundPoint(double cx, double cy, double angleRAD)
        {
            double[,] translationMtr1 = new double[,] { { 1.0, 0.0, cx }, { 0.0, 1.0, cy }, { 0.0, 0.0, 1.0 } };
            double[,] rotationMtr = new double[,] { { Math.Cos(angleRAD), -Math.Sin(angleRAD), 0.0 }, { Math.Sin(angleRAD), Math.Cos(angleRAD), 0.0 }, { 0.0, 0.0, 1.0 } };
            double[,] translationMtr2 = new double[,] { { 1.0, 0.0, -cx }, { 0.0, 1.0, -cy }, { 0.0, 0.0, 1.0 } };
            return CrudeMultiplication2(CrudeMultiplication2(translationMtr1, rotationMtr, 3, 3, 3, 3), translationMtr2, 3, 3, 3, 3);
        }

        /// <summary>
        /// rotate point px, py around point cx, cy : by angleRad . Returns new point coordinatez
        /// https://math.stackexchange.com/questions/2093314/rotation-matrix-of-rotation-around-a-point-other-than-the-origin
        /// </summary>
        public static Tuple<double, double> rotateImageUsingMatrix(double cx, double cy, double angleRAD, double px, double py)
        {
            double[,] coordinateMtrInitial = new double[,] { { px }, { py }, { 1.0 } };
            double[,] coordinateMtr = CrudeMultiplication2(getRotationMatrixAroundPoint(cx, cy, angleRAD), coordinateMtrInitial, 3, 3, 3, 1);
            return new Tuple<double, double>(coordinateMtr[0, 0], coordinateMtr[1, 0]);

        }
        /// <summary>
        /// apply transformation encoded in in_TransfrmMtr to the point declared by px and py. in_TransfrmMtr should have 3 rows and 3 cols. It is double 2d array. Returns new point coordinatez
        /// </summary>
        public static Tuple<double, double> rotateImageUsingPrecalculatedTransformationMatrix(double[,] in_TransfrmMtr, double px, double py)
        {
            double[,] coordinateMtrInitial = new double[,] { { px }, { py }, { 1.0 } };
            double[,] coordinateMtr = CrudeMultiplication2(in_TransfrmMtr, coordinateMtrInitial, 3, 3, 3, 1);
            return new Tuple<double, double>(coordinateMtr[0, 0], coordinateMtr[1, 0]);
        }
    }
}
