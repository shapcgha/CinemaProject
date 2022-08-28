using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    internal abstract class Drinks
    {
        internal string Name { get; set; }
        internal int Price { get; set; }
        protected int Count { get; set; }

        internal int Degree { get; set; }

        internal Drinks(string name, int price)
        {
            Name = name;
            Price = price;
            Count = 0;
        }

        internal abstract int ShowPrice();

        internal abstract int Buy();

        internal abstract void PrintDrink();
    }
}
