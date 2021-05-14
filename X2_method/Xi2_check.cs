using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAND_1.X2_method
{
    class Xi2_check
    {
        //для alpha = 0.05 и n = [1, 16] приведены значения (1-alpha)-квантили X^2-распределния
        Dictionary<int, double> Xi2_quantile = new Dictionary<int, double> { 
    {1, 3.841},
    {2, 5.991},
    {3, 7.815},
    {4, 9.488},
    {5, 11.07},
    {6, 12.592},
    {7, 14.067},
    {8, 15.507},
    {9, 16.919},
    {10, 18.307},
    {11, 19.675},
    {12, 21.026},
    {13, 22.362},
    {14, 23.685},
    {15, 24.996},
    {16, 26.296},
    {20, 10.85},
    {24, 13.848},
    {25, 14.611},
    {27, 16.151},
    {36, 23.268},
    {45, 30.612},
    {60, 43.188},
    {72, 53.462},
    {75, 56.054},
    {84, 63.876},
    {135, 109.156}
    };


        public double Count_Xi2(double[] ExplainV, double[] dependV)
        {
            //Составляем массив неповторяющихся величин
            IEnumerable<double> ExplainVDist = ExplainV.Distinct();
            IEnumerable<double> DependVDist = dependV.Distinct();

            //создаем массивы сумм частот по строкам и столбцам
            int[] Sumi = new int[ExplainVDist.Count()];
            int[] Sumj = new int[DependVDist.Count()];

            //составляем таблицу частот
            int[][] TableFreq = new int[Sumi.Length][];

            int col, row = 0;

            foreach (double val in ExplainVDist)
            {
                TableFreq[row] = new int[Sumj.Length];
                for (int i = 0; i < ExplainV.Length; i++) //проходимся по всему списку размером 1000
                {
                        col = 0;
                    foreach (double category in DependVDist) //проверяем частоту по одному значению зависимой переменной
                    {
                       if (dependV[i] == category && ExplainV[i] == val)
                         { 
                            TableFreq[row][col]++;
                            break;
                         }
                        col++;
                    }
                }
                row++;
            }

            //инициализируем массивы сумм 
            for (int i = 0; i < Sumi.Length; i++)
            {
                for (int j = 0; j < Sumj.Length; j++)
                {
                    Sumi[i] += TableFreq[i][j];
                    Sumj[j] += TableFreq[i][j];
                }
            }

            //считаем статистику хи-квадрат
            double Xi2 = 0;
            double ExpecFreq;

            for (int i = 0; i < Sumi.Length; i++)
                for (int j = 0; j < Sumj.Length; j++)
                {
                    ExpecFreq = ((double)Sumi[i]*Sumj[j])/1000;
                    Xi2 += Math.Pow(TableFreq[i][j] - ExpecFreq, 2)/ ExpecFreq;
                }

            //Console.Write("{0}\t", Math.Round(Count_Kramer_ratio(Xi2, 1000, Sumi.Length, Sumj.Length), 5));
            //Is_dependent(Xi2, (Sumi.Length - 1) * (Sumj.Length - 1));
            //return Xi2;
            return Count_Kramer_ratio(Xi2, 1000, Sumi.Length, Sumj.Length);
        }

        public static double Count_Kramer_ratio(double Xi2, int n, int a, int b)
        {
            return Math.Sqrt(Xi2 / n/Math.Min(a-1,b-1));
        }

        public static double Count_Pirson_ratio(double Xi2, int n)
        {
            return Math.Sqrt(Xi2 /(n + Xi2));
        }

        public bool Is_dependent(double Xi2, int n)
        {
            return Xi2 > Xi2_quantile[n];
        }

    }
}
