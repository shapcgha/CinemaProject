using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    internal class Film
    {
        internal string Name { get; set; }

        internal string Rating { get; set; }

        internal List<Hall> Halls { get; set; }


        internal Film(string name)
        {
            Name = name;
        }


        internal void PrintFilmInformation()
        {
            Console.Write("\"");

            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Console.Write(Name);

            Console.ResetColor();

            Console.Write("\"");
            Console.WriteLine();

            Console.WriteLine($"Возрастной рейтинг: {Rating}");
            Console.WriteLine();
        }

        internal void PrintFilmName()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(Name);
            Console.ResetColor();
        }
    }
}
