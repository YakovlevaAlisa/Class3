using OneVariableFunction = System.Func<double, double>;
using FunctionName = System.String;
using NUnit.Framework.Constraints;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using NUnit.Framework;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Task2
{
   
    public class Task2
    {

/*
 * В этом задании необходимо написать программу, способную табулировать сразу несколько
 * функций одной вещественной переменной на одном заданном отрезке.
 */


// Сформируйте набор как минимум из десяти вещественных функций одной переменной
    internal static Dictionary<FunctionName, OneVariableFunction> AvailableFunctions =
            new Dictionary<FunctionName, OneVariableFunction>
            {
                { "sin", Math.Sin },
                { "cos", Math.Cos },
                { "tan", x => Math.Tan(x) },
                { "fractionalPart", x => x - Math.Truncate(x) },
                { "absoluteValue", x => (x >= 0) ? x : -x },
                { "squareRoot", x => Math.Sqrt(x) },
                { "cubeRoot", x => Math.Cbrt(x) },
                { "square", x => x * x },
                { "cube", x => x * x * x },
                { "exponent", x => Math.Exp(x) },
            };

// Тип данных для представления входных данных
    internal record InputData(double fromX, double toX, int numberOfPoints, List<string> functionNames);

// Чтение входных данных из параметров командной строки
        private static InputData? prepareData(string[] args)
        {
            var functionNames = new List<FunctionName>();
            double fromX = double.Parse(args[0]);
            double toX = double.Parse(args[1]);
            int numberOfPoints = int.Parse(args[2]);
            
            for (int i = 3; i < args.Length; i++)
                functionNames.Add (args[i]);

            return new InputData(fromX, toX, numberOfPoints, functionNames);
        }

// Тип данных для представления таблицы значений функций
// с заголовками столбцов и строками (первый столбец --- значение x,
// остальные столбцы --- значения функций). Одно из полей --- количество знаков
// после десятичной точки.
internal record FunctionTable(string table)
{
            
            // Код, возвращающий строковое представление таблицы (с использованием StringBuilder)
            // Столбец x выравнивается по левому краю, все остальные столбцы по правому.
            // Для форматирования можно использовать функцию String.Format.
            public override string ToString()
            {
                StringBuilder tabulation = new StringBuilder();
                string[] lines = table.Split("\n");
                string[] lineFunctionNames = lines[0].Split(' ');
                tabulation.Append(String.Format("{0,-16} ", lineFunctionNames[0]));

                for (int j = 1; j < lineFunctionNames.Length; j++)
                    tabulation.Append(String.Format("{0,16} ", lineFunctionNames[j]));
                tabulation.AppendLine();

                for (int i = 1; i < lines.Length; i++)
                {
                    string[] functionValues = lines[i].Split(' ');

                    for (int j = 0; j < functionValues.Length - 1; j++)
                    {
                        if (j == 0) tabulation.Append(String.Format("{0,-16} ", Convert.ToDouble(functionValues[j]).ToString("e8")));
                        else tabulation.Append(String.Format("{0,16} ", Convert.ToDouble(functionValues[j]).ToString("e8")));
                    }

                    tabulation.Append(String.Format("{0,16} ", functionValues[functionValues.Length - 1]));
                    if (i != lines.Length - 1) tabulation.AppendLine();
                }

                return tabulation.ToString();
            }
        }

/*
 * Возвращает таблицу значений заданных функций на заданном отрезке [fromX, toX]
 * с заданным количеством точек.
 */
        internal static FunctionTable tabulate(InputData input)
        {
            double fromX = input.fromX;
            double toX = input.toX;
            int numberOfPoints = input.numberOfPoints;
            var functionNames = input.functionNames;
            double distance = (numberOfPoints == 1) ? 0 : (toX - fromX)/(numberOfPoints - 1);
            double x = fromX;
            string table = "x";

            foreach (string function in functionNames)
                if (AvailableFunctions.ContainsKey(function)) table += " " + function;
            table += " length\n";
            
            for (int count = 0; count < numberOfPoints; count++)
            {
                table += CalculateFunction(x, functionNames);
                if (count != numberOfPoints - 1) table += "\n";
                x += distance;
                if (x > toX)
                    x = toX;
            }

            return new FunctionTable(table);
        }
        
        public static void Main(string[] args)
        {
            // Входные данные принимаются в аргументах командной строки
            // fromX fromY numberOfPoints function1 function2 function3 ...

            var input = prepareData(args);

            // Собственно табулирование и печать результата (что надо поменять в этой строке?):
            
            Console.WriteLine((input == null) ? "Данные введены неверно" : tabulate(input).ToString());
        }

        private static string CalculateFunction(double x, List<string> functionNames)
        {
            string s = x.ToString();

            foreach (string function in functionNames)
            {
                if (AvailableFunctions.ContainsKey(function))
                    s += " " + AvailableFunctions[function].Invoke(x).ToString();
            }

            return s + " 8"; //количество знаков после десятичной точки
        }
    }
}