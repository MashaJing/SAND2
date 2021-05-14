using System;
using System.Numerics;
using System.IO;
using ExcelDataReader;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAND_1.X2_method;

namespace SAND_1
{
    class Program
    {
        static void Main(string[] args)
        {
            //открываем excel файл и строим двумерный массив
            //результат чтения
            double[][] RES = new double[11][];

            //чтение данных из excel-файла: потоку указываем путь, метод открытия файла, уровень доступа  
            using (var stream = File.Open(Path.Combine("C:\\Users\\mdzek\\source\\repos\\SAND_1\\SAND_1\\Excel_Data\\Arrays.xlsx"), FileMode.Open, FileAccess.Read))
            {
                //читатель для файлов с расширением *.xlsx
                var excelReader = ExcelReaderFactory.CreateReader(stream);

               //инициализируем хранилище элементов
                for (int i = 0; i < 11; i++)
                {
                    RES[i] = new double[1000];
                    //считываем очередную строку из книги Excel
                    excelReader.Read();

                    //цикл по столбцам книги
                    for (int j = 0; j < 1000; j++) 
                    { RES[i][j] = excelReader.GetDouble(j); }
                }

                //после завершения чтения освобождаем ресурсы
                excelReader.Close();
            }

            Console.WriteLine("Adm_Dur\t | ProductRel\t | TrafficT\t | Month\t | Weekend\t | OS\t | ProductRel_Dur\t | Info_Dur\t | SpecialDay\t | BounceRates\t | Revenue\t");
            //вызываем функцию для вычисления парной взаимосвязи

            Xi2_check check = new Xi2_check();

            double[][] Kramer_arr = new double[10][];

            for (int i = 0; i < 10; i++)
            {
                Kramer_arr[i] = new double[10];
                for (int j = 0; j < 10; j++)
                {
                    Kramer_arr[i][j] = new double();
                    if (j == i)
                        Kramer_arr[i][j] = 0; //чтобы не учитывались при вычислении F(S)
                    else
                    {
                        Kramer_arr[i][j] = check.Count_Xi2(RES[j], RES[i]);
                    }
                    Console.Write("{0}\t", Math.Round(Kramer_arr[i][j], 6));
                }
                Console.WriteLine();
            }

            double[] Rev_Kramer_arr = new double[10];

            for (int i = 0; i < 10; i++)
            {
                Rev_Kramer_arr[i] = new double();
                Rev_Kramer_arr[i] = check.Count_Xi2(RES[i], RES[10]);
            }

            int iter = 1;
            double r = 0;
            double R = 0;
            int one_counter = 0;
            List<int> Res;
            double[] FS = new double[1023];

            while (iter < 1024)
            {
                Res = into_binary(iter);
                Console.WriteLine(Res.Count());

                for (int i=0; i<10; i++)
                {
                    R += Rev_Kramer_arr[i] * Res[i];
                    one_counter += Res[i];
                }
                for (int j = 0; j < 10; j++)
                {
                    if (Res[j] == 0) continue;

                    for (int i = 0; i < 10; i++)
                        r += Kramer_arr[j][i] * Res[i];
                }
                FS[iter-1] = R/Math.Sqrt(one_counter + r);
                
                
                R = 0;
                r = 0;
                one_counter = 0;
                iter++;
            }

            double max_elem = 0;
            iter = 0;
            for (int i = 0; i < FS.Length; i++)
            {
                if (FS[i] > max_elem) 
                {
                    max_elem = FS[i];
                    iter = i;
                }
            }

            Console.WriteLine(String.Join(" ", into_binary(iter + 1).ToArray()));
            Console.Read();

  //          Console.WriteLine();

  //          Console.WriteLine("Pirson:");
  //          for (int i = 10; i > 0; i--)
  //          {
  //              for (int j = 0; j < 10; j++)
  //              {
  //                  if (j >= i)
  //                      Console.Write(" -\t\t");
  //                  else
  //                  {
  //                      double Xi = check.Count_Xi2(RES[j], RES[i]);
  ////                      Console.Write("{0}\t", Math.Round(Xi2_check.Count_Pirson_ratio(Xi, 1000), 6));
  //                  }

  //              }
  //              Console.WriteLine();
  //          }

            /*
            //приложение, которое откроет excel-файл по завершению алгоритма программы
            Excel.Application Excel_App = new Excel.Application();

            //создаём экземпляр рабочей книги Excel
            Excel.Workbook Work_Book = Excel_App.Workbooks.Add();

            //создаём экземпляр листа Excel: 1 - номер листа из списка листов (если лист будет не один в книге)
            Excel.Worksheet Work_Sheet = (Excel.Worksheet)Work_Book.Worksheets.get_Item(1);

            //указываем имя листа книги
            Work_Sheet.Name = "Correlations";

            //заполняем строки таблицы числами
            for (int i = 2; i <= 1000; i++)
            {
                for (int j = 2; j <= 1000; j++)
                {
                    Work_Sheet.Cells[j][i] = RES[i - 1][j - 1];
                }
            }

            //открываем созданный excel-файл
            Excel_App.Visible = true;
            Excel_App.UserControl = true;
            */
        }

        public static List<int> into_binary(int a)
        {
            List<int> result = new List<int>();

            if (a == 0)
            {
                while (result.Count() < 10) result.Add(0);
            }
            else
            {
                while (a != 1)
                {
                    result.Add(a % 2);
                    a /= 2;
                }
                result.Add(1);
                while (result.Count() < 10) result.Add(0);
                result.Reverse();
            }

            return result;
        }


    }
}
