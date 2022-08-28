using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    internal class FilmSession
    {
        const char vacant = '0';
        const char bought = 'x';


        internal DateTime Date { get; set; }

        internal Hall Hall { get; set; }

        internal Place[][] Places { get; set; }

        internal FilmSession(string date, Hall hall)
        {
            Date = DateTime.Parse(date);
            Hall = hall;
        }



        internal void PrintSession()
        {
            Console.WriteLine(Date.ToString("dd.MM.yyyy HH:mm"));
        }


        internal void PrintSessionFullInformation()
        {
            Console.WriteLine("Информация о выбранном сеансе:");
            Console.WriteLine();
            
            Hall.Film.PrintFilmInformation();
            Hall.PrintHallShortName();

            Console.WriteLine();
            Console.WriteLine("Дата сеанса:");
            PrintSession();

            Console.WriteLine();
            
            ShowingPlacesScheme();
            ShowingPricesScheme();

            Console.WriteLine();
        }


        void ShowingPlacesScheme()
        {
            Console.WriteLine($"Свободные и занятые места:");

            foreach (var row in Places)
            {
                foreach (var place in row)
                {
                    if (place.IsBought) { WriteBoughtChar(); }
                    else { WriteVacantChar(); }                    
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        void WriteVacantChar()
        {
            Console.Write(vacant);
            Console.Write(" ");
        }

        void WriteBoughtChar()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(bought);
            Console.ResetColor();
            Console.Write(" ");
        }


        void ShowingPricesScheme()
        {
            Console.WriteLine("Цены билетов:");

            var prices = Places.Select(
                row => row.Select(
                    place => place.Price));

            foreach (var row in prices)
            {
                Console.WriteLine(string.Join(" ", row));
            }

            Console.WriteLine();
        }
    }
}
