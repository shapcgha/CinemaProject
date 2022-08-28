using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    internal class NotAlcocholDrinks : Drinks //неалкогольные напиткм
    {
        internal NotAlcocholDrinks(string name, int price) : base(name, price) { Degree = 0; }

        internal override int Buy()
        {
            Count++;
            return Price;
        }

        internal override void PrintDrink()
        {
            Console.WriteLine(Name + " " + ShowPrice());
        }

        internal override int ShowPrice()
        {
            return Price;
        }

    }
}
