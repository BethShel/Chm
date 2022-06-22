using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Linq;
//using NumericalMethods.UI;


namespace ConsoleApp1
{
    
    public static class MatrixExtensions
    {
        public static IReadOnlyList<(T X, T Y)> ToPoints<T>(this T[,] matrix)
        {
            var result = new List<(T X, T Y)>();

            for (var i = 0; i < matrix.GetLength(1); ++i)
                result.Add((matrix[0, i], matrix[1, i]));

            return result;
        }
    }

    class RungeKutta
    {
        public double[,] Function;
        private double H, A, B, Eps, Y0;
        private int N;
        public double F(double x, double y)
        {
            return Math.Cos(x + y) + 0.5 * (x - y);
        }
        public double CountYMinus(double xPrev, double yPrev, double h)
        {

            return yPrev + (2 * K1(xPrev, yPrev, h) + K3Minus(xPrev, yPrev, h) * 3) / 3;
        }

        public double CountYPlus(double xPrev, double yPrev, double h)
        {
            return yPrev + K3Plus(xPrev, yPrev, h);
        }

        public double K3Plus(double xPrev, double yPrev, double h)
        {
            return h * F(xPrev + h / 2, yPrev + K2(xPrev, yPrev, h) / 2);
        }

        public double K3Minus(double xPrev, double yPrev, double h)
        {
            return h * F(xPrev + h * 5 / 6, yPrev + K2(xPrev, yPrev, h) * 5 / 6);
        }

        public double K2(double xPrev, double yPrev, double h)
        {
            return h * F(xPrev + h / 3, yPrev + K1(xPrev, yPrev, h) / 3);
        }

        public double K1(double xPrev, double yPrev, double h)
        {
            return h * F(xPrev, yPrev);
        }

        public void CountFunc()
        {
            double yPlus = CountYPlus(Function[0, 0], Function[1, 0], H);
            double yMinus = CountYMinus(Function[0, 0], Function[1, 0], H);
            Function[1, 1] = (yMinus + yPlus) / 2.0;
            //  Console.WriteLine(AnaliticFunction[0, 0] + ": " + yPlus + " " + AnaliticFunction[1, 0] + " " + yMinus);

            for (int i = 2; i < Function.GetLength(1); i++)
            {
                double _yPlus = CountYPlus(Function[0, i - 1], yPlus, H);
                double _yMinus = CountYMinus(Function[0, i - 1], yMinus, H);
                Function[1, i] = (_yMinus + _yPlus) / 2.0;
                yPlus = _yPlus;
                yMinus = _yMinus;
            }
        }
        public RungeKutta(double a, double b, int n, double eps, double y0)
        {
            N = n; A = a; B = b; Eps = eps; Y0 = y0;
            Function = new double[2, n + 1];
            H = (b - a) / n;
            for (int i = 0; i <= n; i++)
            {
                Function[0, i] = a + H * i;
            }
            Function[1, 0] = y0;
            CountFunc();
        }

        public static void PrintTable(double[,] table, int n)
        {
            for (int j = 0; j <= n; j++)
            {
                Console.Write($"{/*Math.Round(*/table[0, j]}-> ");
                Console.Write($"{/*Math.Round(*/table[1, j]} \n");
            }
            Console.WriteLine();
            Console.WriteLine();

        }
    }

    class Program
    {

        private struct ToSave
        {
            public double[] X1 { get; set; }
            public double[] Y1 { get; set; }
            public double[] Z1 { get; set; }
            public double[] X3 { get; set; }
            public double[] Y3 { get; set; }
            public double[] Z3 { get; set; }
        };
        public static void CountRK()
        {
            double a = 0, y0 = 0, b = 2.6, eps = 0.05;
            int n = 10;

            RungeKutta rungeKutta = new RungeKutta(a, b, n, eps, y0);
            RungeKutta rungeKuttaN = rungeKutta;
            RungeKutta rungeKutta2N = null;

            int i = 0;
            while (i < 20)
            {
                n *= 2;
                if ((b-a)/n < 64e-6)
                {
                    throw new ApplicationException("Шаг стал недопустимо маленьким, n = " + n);
                }
                rungeKutta2N = new RungeKutta(a, b, n, eps, y0);
                double currentEpsY = Math.Abs(rungeKuttaN.Function[1, rungeKuttaN.Function.GetLength(1) - 1] -
                    rungeKutta2N.Function[1, rungeKuttaN.Function.GetLength(1) - 1]) / 7;
                if (currentEpsY < eps)
                {
                    Console.WriteLine("Найдено нужное n = " + n);
                    break;
                }
                i++;
                rungeKuttaN = rungeKutta2N;
            }
            if (i == 20) { Console.WriteLine("Цикл выполнен 20 раз"); }

            double[] x1 = new double[rungeKutta.Function.GetLength(1)], y1 = new double[rungeKutta.Function.GetLength(1)], 
                x2 = new double[rungeKutta2N.Function.GetLength(1)], y2 = new double[rungeKutta2N.Function.GetLength(1)];

            for (i = 0; i < x1.Length; ++i)
            {
                x1[i] = rungeKutta.Function[0, i];
                y1[i] = rungeKutta.Function[1, i];
            }

            for (i = 0; i < x2.Length; ++i)
            {
                x2[i] = rungeKutta2N.Function[0, i];
                y2[i] = rungeKutta2N.Function[1, i];
            }

            drawingGraph(x1, y1, x2, y2);
        }
        static void drawingGraph(double[] x1, double[] y1, double[] x2, double[] y2)
        {
            try
            {
                ToSave data = new ToSave();
                data.X1 = x1;
                data.Y1 = y1;
                data.X3 = x2;
                data.Y3 = y2;

                string json = JsonSerializer.Serialize(data);
                File.WriteAllText(@"D:\Лиза\ВУЗ\second1\second1\Save\temp.json", json);
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(@"D:\Лиза\ВУЗ\second1\second1\visualizationGraphs\visualizationGraphs.py")
                {
                    UseShellExecute = true
                };
                p.Start();
                p.WaitForExit();
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void Main(string[] args)
        {
            CountRK();
        }
    }
}