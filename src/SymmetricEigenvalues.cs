using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EigenThings
{
    public class SymmetricEigenvalues
    {
        //Reduced version of EigenvalueDecomposition class from
        //http://www.codeproject.com/KB/recipes/psdotnetmatrix.aspx

        private readonly int n;
        private readonly double[] d;
        private readonly double[] e;
        private readonly double[][] V;

        private static double Hypot(double a, double b)
        {
            double r;
            if (Math.Abs(a) > Math.Abs(b))
            {
                r = b / a;
                r = Math.Abs(a) * Math.Sqrt(1 + r * r);
            }
            else if (b != 0)
            {
                r = a / b;
                r = Math.Abs(b) * Math.Sqrt(1 + r * r);
            }
            else
            {
                r = 0.0;
            }
            return r;
        }

        private void Tridiagonalize()
        {
            for (int j = 0; j < n; j++)
            {
                d[j] = V[n - 1][j];
            }


            for (int i = n - 1; i > 0; i--)
            {
                double scale = 0.0;
                double h = 0.0;
                for (int k = 0; k < i; k++)
                {
                    scale = scale + System.Math.Abs(d[k]);
                }
                if (scale == 0.0)
                {
                    e[i] = d[i - 1];
                    for (int j = 0; j < i; j++)
                    {
                        d[j] = V[i - 1][j];
                        V[i][j] = 0.0;
                        V[j][i] = 0.0;
                    }
                }
                else
                {
                    for (int k = 0; k < i; k++)
                    {
                        d[k] /= scale;
                        h += d[k] * d[k];
                    }
                    double f = d[i - 1];
                    double g = System.Math.Sqrt(h);
                    if (f > 0)
                    {
                        g = -g;
                    }
                    e[i] = scale * g;
                    h = h - f * g;
                    d[i - 1] = f - g;
                    for (int j = 0; j < i; j++)
                    {
                        e[j] = 0.0;
                    }

                    for (int j = 0; j < i; j++)
                    {
                        f = d[j];
                        V[j][i] = f;
                        g = e[j] + V[j][j] * f;
                        for (int k = j + 1; k <= i - 1; k++)
                        {
                            g += V[k][j] * d[k];
                            e[k] += V[k][j] * f;
                        }
                        e[j] = g;
                    }
                    f = 0.0;
                    for (int j = 0; j < i; j++)
                    {
                        e[j] /= h;
                        f += e[j] * d[j];
                    }
                    double hh = f / (h + h);
                    for (int j = 0; j < i; j++)
                    {
                        e[j] -= hh * d[j];
                    }
                    for (int j = 0; j < i; j++)
                    {
                        f = d[j];
                        g = e[j];
                        for (int k = j; k <= i - 1; k++)
                        {
                            V[k][j] -= (f * e[k] + g * d[k]);
                        }
                        d[j] = V[i - 1][j];
                        V[i][j] = 0.0;
                    }
                }
                d[i] = h;
            }

            for (int i = 0; i < n - 1; i++)
            {
                V[n - 1][i] = V[i][i];
                V[i][i] = 1.0;
                double h = d[i + 1];
                if (h != 0.0)
                {
                    for (int k = 0; k <= i; k++)
                    {
                        d[k] = V[k][i + 1] / h;
                    }
                    for (int j = 0; j <= i; j++)
                    {
                        double g = 0.0;
                        for (int k = 0; k <= i; k++)
                        {
                            g += V[k][i + 1] * V[k][j];
                        }
                        for (int k = 0; k <= i; k++)
                        {
                            V[k][j] -= g * d[k];
                        }
                    }
                }
                for (int k = 0; k <= i; k++)
                {
                    V[k][i + 1] = 0.0;
                }
            }
            for (int j = 0; j < n; j++)
            {
                d[j] = V[n - 1][j];
                V[n - 1][j] = 0.0;
            }
            V[n - 1][n - 1] = 1.0;
            e[0] = 0.0;
        }

        private void Diagonalize()
        {

            for (int i = 1; i < n; i++)
            {
                e[i - 1] = e[i];
            }
            e[n - 1] = 0.0;

            double f = 0.0;
            double tst1 = 0.0;
            double eps = System.Math.Pow(2.0, -52.0);
            for (int l = 0; l < n; l++)
            {
                tst1 = System.Math.Max(tst1, System.Math.Abs(d[l]) + System.Math.Abs(e[l]));
                int m = l;
                while (m < n)
                {
                    if (System.Math.Abs(e[m]) <= eps * tst1)
                    {
                        break;
                    }
                    m++;
                }
                if (m > l)
                {
                    int iter = 0;
                    do
                    {
                        iter = iter + 1;

                        double g = d[l];
                        double p = (d[l + 1] - g) / (2.0 * e[l]);
                        double r = Hypot(p, 1.0);
                        if (p < 0)
                        {
                            r = -r;
                        }
                        d[l] = e[l] / (p + r);
                        d[l + 1] = e[l] * (p + r);
                        double dl1 = d[l + 1];
                        double h = g - d[l];
                        for (int i = l + 2; i < n; i++)
                        {
                            d[i] -= h;
                        }
                        f = f + h;

                        p = d[m];
                        double c = 1.0;
                        double c2 = c;
                        double c3 = c;
                        double el1 = e[l + 1];
                        double s = 0.0;
                        double s2 = 0.0;
                        for (int i = m - 1; i >= l; i--)
                        {
                            c3 = c2;
                            c2 = c;
                            s2 = s;
                            g = c * e[i];
                            h = c * p;
                            r = Hypot(p, e[i]);
                            e[i + 1] = s * r;
                            s = e[i] / r;
                            c = p / r;
                            p = c * d[i] - s * g;
                            d[i + 1] = h + s * (c * g + s * d[i]);

                            for (int k = 0; k < n; k++)
                            {
                                h = V[k][i + 1];
                                V[k][i + 1] = s * V[k][i] + c * h;
                                V[k][i] = c * V[k][i] - s * h;
                            }
                        }
                        p = (-s) * s2 * c3 * el1 * e[l] / dl1;
                        e[l] = s * p;
                        d[l] = c * p;

                    }
                    while (System.Math.Abs(e[l]) > eps * tst1);
                }
                d[l] = d[l] + f;
                e[l] = 0.0;
            }


            for (int i = 0; i < n - 1; i++)
            {
                int k = i;
                double p = d[i];
                for (int j = i + 1; j < n; j++)
                {
                    if (d[j] < p)
                    {
                        k = j;
                        p = d[j];
                    }
                }
                if (k != i)
                {
                    d[k] = d[i];
                    d[i] = p;
                    for (int j = 0; j < n; j++)
                    {
                        p = V[j][i];
                        V[j][i] = V[j][k];
                        V[j][k] = p;
                    }
                }
            }
        }


        public SymmetricEigenvalues(double[][] matrix)
        {
            this.n = matrix.Length;
            V = new double[n][];

            for (int i = 0; i < n; i++)
                V[i] = new double[n];
            
            d = new double[n];
            e = new double[n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    V[i][j] = matrix[i][j];

            Tridiagonalize();
            Diagonalize();
        }

        public double[] EigenValues { get { return d; } }
        public double[][] EigenVectors { get { return V; } }
    }
}
