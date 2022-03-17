using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    internal class Hall
    {
        public int N { get; set; }

        public int M { get; set; }

        public string Name { get; set; }

        public Film Film { get; set; }

        public List<FilmSession> Sessions { get; set; }

        public int[][] DefaultPrices { get; set; }


        public Hall(string name)
        {
            Name = name;
        }


        public void PrintHallName()
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

        public void PrintHallShortName()
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
