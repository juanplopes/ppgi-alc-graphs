using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EigenThings
{
    public static class MatrixHelpers
    {
        public static double[][] Multiply(this double[][] matrix1, double[][] matrix2)
        {
            int a = matrix1.Length, b = matrix2.Length;
            if (a == 0 || b == 0) return new double[0][];
            int c = matrix2[0].Length;

            var res = new double[a][];

            for (int i = 0; i < a; i++)
            {
                res[i] = new double[c];
                for (int j = 0; j < b; j++)
                    for (int k = 0; k < c; k++)
                        res[i][k] += matrix1[i][j] * matrix2[j][k];
            }

            return res;
        }
    }
}
