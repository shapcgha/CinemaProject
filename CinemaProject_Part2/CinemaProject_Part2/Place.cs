using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    internal class Place
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public int Price { get; set; }

        public FilmSession Session { get; set; }

        public bool IsBought { get; set; }

        public int? BoughtAtPrice { get; set; }        

        public string NameOfBuyer { get; set; }


        public void PrintTicket()
        {
            Console.WriteLine($"Ряд: {Row}. Место {Column}");
        }
    }
}
