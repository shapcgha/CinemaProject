using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    internal class Place
    {
        internal int Row { get; set; }

        internal int Column { get; set; }

        internal int Price { get; set; }

        internal FilmSession Session { get; set; }

        internal bool IsBought { get; set; }

        internal int? BoughtAtPrice { get; set; }

        internal string NameOfBuyer { get; set; }


        internal void PrintTicket()
        {
            Console.WriteLine($"Ряд: {Row}. Место {Column}");
        }
    }
}
