using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    class UserInterface
    {
        static List<Film> films;
        static List<FilmSession> allFilmSessions;
        static List<Hall> allHalls;

        static public void start(List<Film> listFilms, List<FilmSession> listFilmSessions, List<Hall> halls)
        {
            films = listFilms;
            allFilmSessions = listFilmSessions;
            allHalls = halls;
            Console.Clear();
            Console.WriteLine("Вы вошли в систему как клиент!");
            Console.WriteLine();

            var balance = GetUserBalance();
            var name = GetUserName();

            var boughtTickets = new List<Place>();

            while (true)
            {
                Console.Clear();

                Console.WriteLine("Что вы хотите сделать?");
                Console.WriteLine("Нажмите 1 для просмотра информации о фильмах в кинотеатре");
                Console.WriteLine("Нажмите 2 для покупки билетов");
                Console.WriteLine("Нажмите 3 для просмотра купленных вами билетов");
                Console.WriteLine("Нажмите 4 для того, чтобы пополнить баланс");
                Console.WriteLine("Нажмите любую другую кнопку для завершения пользовательского сеанса");

                var key = Console.ReadKey().Key;

                Console.Clear();

                if (key == ConsoleKey.D1)
                {
                    ShowingFilmsInformation();
                }

                else if (key == ConsoleKey.D2)
                {
                    var locallyBoughtTickets = BuyingTickets(balance, name, out balance);

                    if (locallyBoughtTickets != null)
                    {
                        boughtTickets.AddRange(locallyBoughtTickets);

                        boughtTickets = boughtTickets
                            .OrderBy(tick => tick.Session.Date)
                            .ThenBy(tick => tick.Row)
                            .ThenBy(tick => tick.Column)
                            .ToList();
                    }
                }

                else if (key == ConsoleKey.D3)
                {
                    ShowingBoughtTickets(boughtTickets);
                }

                else if (key == ConsoleKey.D4)
                {
                    balance = IncreasingBalance(balance);
                }

                else
                {
                    break;
                }
            }
        }

        static int GetUserBalance()
        {
            Console.WriteLine("Укажите текущий баланс счета:");

            uint answer = 0;

            while (true)
            {
                try
                {
                    answer = uint.Parse(Console.ReadLine());
                    break;
                }
                catch
                {
                    Console.WriteLine("Требуется натуральное число");
                }
            }

            Console.WriteLine();

            return (int) answer;
        }

        static string GetUserName()
        {
            Console.WriteLine("Укажите ваше ФИО для корректной работы с системой:");

            var answer = Console.ReadLine();

            Console.WriteLine();

            return answer;
        }

        static void ShowingFilmsInformation()
        {
            Console.WriteLine("Репертуар:");
            Console.WriteLine();

            foreach (var film in films)
            {
                film.PrintFilmInformation();
            }

            PressAnyKey();
        }

        static List<Place> BuyingTickets(int balance, string name, out int newBalance)
        {
            newBalance = balance;

            while (true)
            {
                var filteredSessions = FilteredSessionsByUser();

                if (filteredSessions.Count == 0)
                {
                    if (RepeatFilters()) { continue; }
                    else { return null; }
                }

                else
                {
                    Console.Clear();

                    Console.WriteLine("Список сеансов для покупки:");
                    Console.WriteLine();
                    Console.WriteLine();

                    ShowingSessions(filteredSessions);

                    bool wasChosen = ChoosingSession(filteredSessions, out FilmSession chosenSession);

                    if (!wasChosen) { return null; }


                    Console.Clear();

                    chosenSession.PrintSessionFullInformation();

                    bool wasBought = BuyingTicketsOfChosenSession(chosenSession, balance, name, out List<Place> boughtPlaces, out newBalance);

                    if (!wasBought) { return null; }

                    return boughtPlaces;
                }
            }
        }

        static List<FilmSession> FilteredSessionsByUser()
        {
            Console.WriteLine("Ниже зададим фильтры для отображения сеансов:");
            Console.WriteLine();

            List<Func<FilmSession, bool>> filters = new List<Func<FilmSession, bool>>();


            FilteringByFilm(filters,
                "1. Фильтр по фильмам",
                "Если вы хотите отобразить сеансы одного конкретного фильма, напишите его название",
                "Для отображения сеансов всех фильмов нажмите Enter");

            FilteringByHall(filters,
                "2. Фильтр по залам",
                "Если вы хотите отобразить сеансы в одном конкретном зале, напишите его название (напишите название полностью, с техническими символами, если они есть)",
                "Для отображения сеансов по всем залам нажмите Enter");

            FilteringBySelectedDate(filters,
                "3. Фильтр по дате",
                "Если вы хотите отобразить сеансы на конкретную дату, напишите эту дату в формате ДД.ММ.ГГГГ",
                "Для отображения сеансов по всем датам нажмите Enter");

            FilteringByCurrentDate(filters);
            FilteringByAllBoughtPlaces(filters);


            var filteredSessions = allFilmSessions.Where(filters[0]);

            for (int i = 1; i < filters.Count; i++)
            {
                filteredSessions = filteredSessions.Where(filters[i]);
            }

            return filteredSessions.ToList();
        }

        static bool RepeatFilters()
        {
            Console.WriteLine("К сожалению, с учетом указанных вами фильтров нет доступных сеансов");
            Console.WriteLine("Чтобы повторить ввод фильтров нажмите 1");
            Console.WriteLine("Чтобы вернуться назад, нажмите любую другую кнопку");

            var key = Console.ReadKey().Key;

            Console.Clear();

            return key == ConsoleKey.D1;
        }

        static void ShowingBoughtTickets(List<Place> boughtTickets)
        {
            if (!boughtTickets.Any())
            {
                Console.WriteLine("У вас сейчас нет купленных билетов");
                Console.WriteLine();
            }

            else
            {
                Console.WriteLine("Список ваших билетов:");
                Console.WriteLine();
                Console.WriteLine();

                var filteredFilms = boughtTickets
                    .Select(tick => tick.Session.Hall.Film)
                    .Distinct();

                var filteredHalls = boughtTickets
                    .Select(tick => tick.Session.Hall)
                    .Distinct();

                var filteredSessions = boughtTickets
                    .Select(tick => tick.Session)
                    .Distinct();


                foreach (var film in filteredFilms)
                {
                    film.PrintFilmInformation();

                    foreach (var hall in film.Halls.Intersect(filteredHalls))
                    {
                        hall.PrintHallShortName();

                        foreach (var session in hall.Sessions.Intersect(filteredSessions))
                        {
                            session.PrintSession();

                            var ticketsOfSession = boughtTickets
                                .Where(tick => tick.Session == session);

                            foreach (var ticket in ticketsOfSession)
                            {
                                ticket.PrintTicket();
                            }

                            Console.WriteLine();
                        }

                        Console.WriteLine();
                        Console.WriteLine();
                    }

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }

            PressAnyKey();
        }

        static int IncreasingBalance(int balance)
        {
            Console.WriteLine($"Ваш текущий баланс: {balance}");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Введите сумму, на которую вы хотите пополнить баланс:");

            uint newBalance = 0;

            while (true)
            {
                try
                {
                    newBalance = uint.Parse(Console.ReadLine());
                    break;
                }
                catch
                {
                    Console.WriteLine("Требуется натуральное число");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Баланс успешно пополнен!");
            Console.WriteLine($"Ваш текущий баланс: {newBalance}");

            Console.WriteLine();

            PressAnyKey();

            return (int) newBalance;
        }

        static void ShowingSessions(List<FilmSession> sessions = null, bool allSessions = false, Film selectedFilm = null)
        {
            int index = 0;

            List<Film> filteredFilms;

            if (selectedFilm != null)
            {
                filteredFilms = new List<Film> { selectedFilm };
            }
            else if (allSessions)
            {
                filteredFilms = films;
            }
            else
            {
                filteredFilms = sessions
                    .Select(ses => ses.Hall.Film)
                    .Distinct()
                    .ToList();
            }


            foreach (var film in filteredFilms)
            {
                film.PrintFilmInformation();

                List<Hall> filteredHalls;

                if (!allSessions & selectedFilm == null)
                {
                    filteredHalls = film.Halls
                        .Where(hall => hall.Sessions.Intersect(sessions).Any())
                        .ToList();
                }
                else { filteredHalls = film.Halls; }


                foreach (var hall in filteredHalls)
                {
                    hall.PrintHallShortName();

                    List<FilmSession> filteredSessions;

                    if (!allSessions & selectedFilm == null)
                    {
                        filteredSessions = hall.Sessions
                            .Where(ses => sessions.Contains(ses))
                            .ToList();
                    }
                    else { filteredSessions = hall.Sessions; }


                    foreach (var session in filteredSessions)
                    {
                        index++;

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write($"{index}.  ");
                        Console.ResetColor();

                        session.PrintSession();
                    }

                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine();
            }
        }



        static bool ChoosingSession(List<FilmSession> sessions, out FilmSession chosenSession)
        {
            Console.WriteLine("Напишите номер сеанса, на который вы бы хотели посмотреть билеты.");
            Console.WriteLine("Чтобы вернуться в пользовательское меню, нажмите Enter");

            var answerBool = int.TryParse(Console.ReadLine(), out int answer);

            if (!answerBool)
            {
                chosenSession = null;
                return false;
            }
            else if (!(answer >= 1 & answer <= sessions.Count))
            {
                chosenSession = null;
                return false;
            }
            else
            {
                chosenSession = sessions[answer - 1];
                return true;
            }
        }



        static bool BuyingTicketsOfChosenSession(FilmSession session, int balance, string nameOfBuyer, out List<Place> boughtPlaces, out int newBalance)
        {
            boughtPlaces = null;
            newBalance = balance;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"Ваш баланс: {balance}");
            Console.WriteLine();
            Console.WriteLine();


            while (true)
            {
                List<Place> requestedPlaces = FormingTheListOfRequestedPlaces(session);

                if (requestedPlaces.Count == 0) { return false; }

                if (requestedPlaces.Sum(place => place.Price) > balance)
                {
                    LowFundsNotification();

                    if (!BuyingTicketsAgain()) { return false; }

                    Console.WriteLine();
                    Console.WriteLine();
                }

                else
                {
                    foreach (var place in requestedPlaces)
                    {
                        place.IsBought = true;
                        place.BoughtAtPrice = place.Price;
                        place.NameOfBuyer = nameOfBuyer;
                    }

                    newBalance = balance - requestedPlaces.Sum(place => place.Price);
                    boughtPlaces = requestedPlaces;

                    TicketsSuccessfullyBoughtNotification();

                    PressAnyKey();

                    return true;
                }
            }
        }

        static bool BuyingTicketsAgain()
        {
            Console.WriteLine("Нажмите 1, чтобы выбрать билеты еще раз");
            Console.WriteLine("Чтобы вернуться назад, нажмите любую другую кнопку");

            return Console.ReadKey().Key == ConsoleKey.D1;
        }

        static void LowFundsNotification()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("На вашем счете недостаточно средств");
            Console.ResetColor();
            Console.WriteLine();
        }

        static void TicketsSuccessfullyBoughtNotification()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Билеты успешно куплены");
            Console.ResetColor();
            Console.WriteLine();
        }



        static List<Place> FormingTheListOfRequestedPlaces(FilmSession session)
        {
            Console.WriteLine("Введите в строку через пробел номер ряда и места, которые хотите купить, и нажмите Enter");
            Console.WriteLine("Вы можете ввести так несколько мест подряд в столбик. Когда захотите закончить покупку, нажмите Enter еще раз");
            Console.WriteLine("Если вы не хотите покупать билеты вообще, просто нажмите Enter");

            Console.WriteLine();

            List<Place> requestedPlaces = new List<Place>();

            while (true)
            {
                var request = Console.ReadLine();

                if (request == "") { return requestedPlaces; }

                var requestedPlace = request
                    .Split()
                    .Select(int.Parse)
                    .ToArray();

                var row = requestedPlace[0];
                var column = requestedPlace[1];

                if (!(row >= 1 & row <= session.Hall.N &
                    column >= 1 & column <= session.Hall.M))
                {
                    ErrorMessage("Такого места нет в зале");
                }

                else if (session.Places[row - 1][column - 1].IsBought)
                {
                    ErrorMessage("К сожалению, это место уже куплено");
                }

                else if (requestedPlaces.Exists(p => p == session.Places[row - 1][column - 1]))
                {
                    ErrorMessage("Вы уже указали это место");
                }

                else
                {
                    requestedPlaces.Add(session.Places[row - 1][column - 1]);
                }
            }
        }

        static void ErrorMessage(string errorMessage)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
            Console.WriteLine();
        }

        static void PressAnyKey()
        {
            Console.WriteLine("Нажмите любую кнопку для продолжения");

            Console.ReadKey();
        }

        static void FilteringByCurrentDate(List<Func<FilmSession, bool>> filters)
        {
            filters.Add(ses => ses.Date > DateTime.Now);
        }

        static void FilteringByAllBoughtPlaces(List<Func<FilmSession, bool>> filters)
        {
            filters.Add(ses => ses.Places.Any(row => row.Any(pl => !pl.IsBought)));
        }

        static void FilteringByFilm(List<Func<FilmSession, bool>> filters, string firstLine, string secondLine, string thirdLine)
        {
            Console.WriteLine(firstLine);
            Console.WriteLine();

            ShowingFilmsList();

            Console.WriteLine(secondLine);
            Console.WriteLine(thirdLine);

            var answer = Console.ReadLine();

            if (answer != "")
            {
                filters.Add(ses => ses.Hall.Film.Name == answer);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        static void ShowingFilmsList()
        {
            Console.WriteLine("Список всех фильмов:");
            Console.WriteLine();

            foreach (var film in films)
            {
                film.PrintFilmName();
            }

            Console.WriteLine();
            Console.WriteLine();
        }


        static void FilteringByHall(List<Func<FilmSession, bool>> filters, string firstLine, string secondLine, string thirdLine)
        {
            Console.WriteLine(firstLine);
            Console.WriteLine();

            ShowingHallsList();

            Console.WriteLine(secondLine);
            Console.WriteLine(thirdLine);

            var answer = Console.ReadLine();

            if (answer != "")
            {
                filters.Add(ses => ses.Hall.Name == answer);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        static void ShowingHallsList()
        {
            Console.WriteLine("Список всех залов кинотеатра:");
            Console.WriteLine();

            foreach (var hall in allHalls)
            {
                hall.PrintHallName();
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        static void FilteringBySelectedDate(List<Func<FilmSession, bool>> filters, string firstLine, string secondLine, string thirdLine)
        {
            Console.WriteLine(firstLine);
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine(secondLine);
            Console.WriteLine(thirdLine);

            var answerTry = DateTime.TryParse(Console.ReadLine(), out DateTime answer);

            if (answerTry)
            {
                var day = answer.Day;
                var month = answer.Month;
                var year = answer.Year;

                filters.Add(ses => ses.Date.Day == day & ses.Date.Month == month & ses.Date.Year == year);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
