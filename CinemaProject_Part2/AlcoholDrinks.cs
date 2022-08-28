using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    internal class AlcoholDrinks : Drinks
    {
        private const int TAX = 6;

        internal AlcoholDrinks(string name, int price, int degree) : base(name, price) 
        { 
            Degree = degree;
        }

        internal override int Buy()
        {
            Console.WriteLine("Вам есть 18 лет? Yes/No");
            string answer = Console.ReadLine();
            if (answer.Equals("Yes"))
            {
                Count++;
                return Price + Degree * TAX;
            }
            else
            {
                Console.WriteLine("Простите мы не можем продать вам данный напиток");
                return 0;
            }
        }

        internal override void PrintDrink()
        {
            Console.WriteLine(Name + " " + ShowPrice() + " 18+");
        }

        internal override int ShowPrice()
        {
            return Price + Degree * TAX;
        }
    }
}
