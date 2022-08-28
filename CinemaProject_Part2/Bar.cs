using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    static internal class Bar
    {
        static public List<Drinks> Drinks = new List<Drinks>();

        static public void AddNewDrink(string name, int price, bool isAlcohol, int degree)
        {
            if (isAlcohol)
            {
                Drinks.Add(new AlcoholDrinks(name, price, degree));
            } 
            else
            {
                Drinks.Add(new NotAlcocholDrinks(name, price));
            }
        }

        static public int BuyDrink(int balance)
        {
            Console.WriteLine("Список напитков для покупки:");
            Console.WriteLine();
            Console.WriteLine();
            PrintDrinkList();
            bool wasChosen = ChoosingDrink(out int numberInMenu);
            if (numberInMenu != -1)
            {
                if (!(Drinks[numberInMenu].ShowPrice() > balance))
                {
                    Drinks[numberInMenu].Buy();
                    Console.WriteLine($"Куплен {Drinks[numberInMenu].Name} по цене {Drinks[numberInMenu].ShowPrice()}");
                    return Drinks[numberInMenu].Price;
                }
            }
            Console.WriteLine("Не удалось купить напиток попробуйте снова");
            return 0;
        }

        static private bool ChoosingDrink(out int numberInMenu)
        {
            Console.WriteLine("Напишите номер напитка, который вы бы хотели купить.");
            Console.WriteLine("Чтобы вернуться в пользовательское меню, нажмите Enter");

            var answerBool = int.TryParse(Console.ReadLine(), out int answer);

            if (!answerBool)
            {
                numberInMenu = -1;
                return false;
            }
            else if (!(answer >= 1 & answer <= Drinks.Count))
            {
                numberInMenu = -1;
                return false;
            }
            else
            {
                numberInMenu = answer - 1;
                return true;
            }
        }

        static private void PrintDrinkList()
        {
            int index = 0;
            foreach (var drink in Drinks)
            {     
                index++;

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($"{index}.  ");
                Console.ResetColor();
                drink.PrintDrink();
            }

            Console.WriteLine();
            Console.WriteLine();
        }

    }

}
