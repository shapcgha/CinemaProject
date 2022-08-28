using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    internal class Hall
    {
        internal int N { get; set; }

        internal int M { get; set; }

        internal string Name { get; set; }

        internal Film Film { get; set; }

        internal List<FilmSession> Sessions { get; set; }

        internal int[][] DefaultPrices { get; set; }


        internal Hall(string name)
        {
            Name = name;
        }

        internal void SetPrice(int price)
        {
            DefaultPrices = Enumerable.Range(0, N).Select(_ => Enumerable.Repeat(price, M).ToArray()).ToArray();
        }


        internal void PrintHallName()
        {
            Console.Write("Зал \"");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(Name);
            Console.ResetColor();

            Console.Write("\". В нем демонстрируется фильм \"");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(Film.Name);
            Console.ResetColor();

            Console.Write("\"");
            Console.WriteLine();
            Console.WriteLine();
        }

        internal void PrintHallShortName()
        {
            Console.Write("Зал \"");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(Name);
            Console.ResetColor();

            Console.Write("\"");
            Console.WriteLine();
        }
    }
}
