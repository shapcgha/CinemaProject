using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    internal class Film
    {
        public string Name { get; set; }

        public string Rating { get; set; }

        public List<Hall> Halls { get; set; }


        public Film(string name)
        {
            Name = name;
        }


        public void PrintFilmInformation()
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

        public void PrintFilmName()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(Name);
            Console.ResetColor();
        }
    }
}
