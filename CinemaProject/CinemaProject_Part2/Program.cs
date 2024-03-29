﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CinemaProject_Part2
{
    internal class Program
    {
        const string password = "pass";

        static List<Film> films;
        static List<Hall> allHalls;
        static List<FilmSession> allFilmSessions;
        static StreamReader reader;
        static StreamWriter writer;
        static bool isLoad = false;



        static void Main()
        {
            Initializing();
            Console.ReadKey();
        }


        static void Initializing()
        {
            if (File.Exists("cinema.txt"))
            {
                reader = new StreamReader("cinema.txt", Encoding.UTF8);
                isLoad = true;
            }
            InitializingFilms();
            SettingFilmsHalls();
            GettingHallsAndSessionsLists();
            GetDrinks();
            if (isLoad)
            {
                reader.Close();
            }
            isLoad = false;

            MainProgramLoop();
            OnSave();
        }

        static void GetDrinks()
        {
            Console.WriteLine("Введите количество напитков в баре:");
            int drinkNumber = ReadNatural();
            for (int i = 0; i < drinkNumber; i++)
            {
                AddingDrink(true);
            }
        }

        static void OnSave()
        {
            try
            {
                writer = new StreamWriter(File.Create("cinema.txt"));
                SaveFilmList();
                SaveHallInfo();
                SaveDrinks();
                writer.Flush();
                writer.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Неудалось сохранить файл " + e.Message);
                writer.Close();
                File.Delete("cinema.txt");
            }
        }

        static void SaveFilmList()
        {
            writer.WriteLine(films.Count);
            foreach (Film film in films)
            {
                writer.WriteLine(film.Name);
            }
            foreach (Film film in films)
            {
                writer.WriteLine(film.Rating);
            }
        }

        static void SaveHallInfo()
        {
            foreach (Film film in films)
            {
                writer.WriteLine(film.Halls.Count);
                foreach (Hall hall in film.Halls)
                {
                    writer.WriteLine(hall.Name);
                    if (hall is OpenSpaceHall)
                    {
                        writer.WriteLine("Yes");
                    }
                    else
                    {
                        writer.WriteLine("No");
                    }
                }
                foreach (Hall hall in film.Halls)
                {
                    writer.WriteLine(hall.N + " " + hall.M);
                    if (hall is OpenSpaceHall)
                    {
                        writer.WriteLine(hall.DefaultPrices[0][0]);
                    }
                    else
                    {
                        foreach (int[] priceRow in hall.DefaultPrices)
                        {
                            for (int i = 0; i < priceRow.Length - 1; i++)
                            {
                                writer.Write(priceRow[i] + " ");
                            }
                            writer.Write(priceRow[priceRow.Length - 1]);
                            writer.WriteLine();
                        }
                    }
                    SaveSessions(hall);
                }
            }
        }

        static void SaveSessions(Hall hall)
        {
                writer.WriteLine(hall.Sessions.Count);
                foreach (FilmSession session in hall.Sessions)
                {
                    writer.WriteLine(session.Date.ToString("dd.MM.yyyy HH:mm"));
                }
                foreach (FilmSession session in hall.Sessions)
                {
                    for (int i = 0; i < hall.N; i++)
                    {
                        for (int j = 0; j < hall.M; j++)
                        {
                            writer.WriteLine(session.Places[i][j].IsBought);
                            if (session.Places[i][j].IsBought)
                            {
                                writer.WriteLine(session.Places[i][j].NameOfBuyer);
                                writer.WriteLine(session.Places[i][j].BoughtAtPrice);
                            }
                        }
                    }
                }
        }

        static void SaveDrinks()
        {
            writer.WriteLine(Bar.Drinks.Count);
            foreach(Drinks drink in Bar.Drinks) 
            {
                writer.WriteLine(drink.Name);
                writer.WriteLine(drink.Price);
                if (drink is AlcoholDrinks)
                {
                    writer.WriteLine("Yes");
                    writer.WriteLine(drink.Degree);
                }
                else
                {
                    writer.WriteLine("No");
                }
            }
        }


        static void InitializingFilms()
        {
            Console.WriteLine("Введите количество фильмов:");
            int filmsNumber = ReadNatural();
            Console.WriteLine();

            films = new List<Film>();

            Console.WriteLine($"В следующих {filmsNumber} строках введите названия этих фильмов:");

            for (int i = 0; i < filmsNumber; i++)
            {
                string filmName;
                if (isLoad)
                {
                    filmName = reader.ReadLine();
                } 
                else
                {
                    filmName = Console.ReadLine();
                }
                films.Add(new Film(filmName));
            }

            Console.WriteLine();


            Console.WriteLine($"В следующих {filmsNumber} строках введите возрастные рейтинги для этих фильмов:");

            for (int i = 0; i < filmsNumber; i++)
            {
                films[i].Rating = ReadRating();
            }

            Console.WriteLine();
        }



        static void SettingFilmsHalls()
        {
            foreach (var film in films)
            {
                Console.WriteLine();
                Console.WriteLine($"Теперь укажем информацию о залах фильма {film.Name}");

                film.Halls = InitializingHalls(film.Name);

                SettingHallsParameters(film);
            }
        }


        static List<Hall> InitializingHalls(string filmName = "", bool initial = true)
        {
            Console.WriteLine("Введите количество залов для этого фильма:");
            int hallsNumber = ReadNatural();

            Console.WriteLine();

            var halls = new List<Hall>();

            if (initial)
            {
                Console.WriteLine($"В следующих {hallsNumber} строках введите названия залов для фильма {filmName}:");
            }
            else
            {
                Console.WriteLine($"В следующих {hallsNumber} строках введите названия залов");
            }


            for (int i = 0; i < hallsNumber; i++)
            {
                string hallName;
                if (isLoad)
                {
                    hallName = reader.ReadLine();
                }
                else
                {
                    hallName = Console.ReadLine();
                }
                Console.WriteLine("Этот зал открытый? (Yes/No)");
                string answer;
                if (isLoad)
                {
                    answer = reader.ReadLine();
                }
                else
                {
                    answer = Console.ReadLine();
                }
                if (answer.Equals("Yes"))
                {
                    halls.Add(new OpenSpaceHall(hallName));
                }
                else
                {
                    halls.Add(new Hall(hallName));
                }
            }

            Console.WriteLine();

            return halls;
        }


        static void SettingHallsParameters(Film film)
        {
            Console.WriteLine("Теперь для каждого зала этого фильма давайте укажем его размер и дефолтные значения цен");

            Console.WriteLine();
            Console.WriteLine();

            foreach (var hall in film.Halls)
            {
                SettingOneHallParameters(film, hall);
            }
        }

        static void SettingOneHallParameters(Film film, Hall hall)
        {
            hall.Film = film;

            SettingHallsSize(hall.Name, out int N, out int M);

            hall.M = M;
            hall.N = N;

            if (hall is OpenSpaceHall)
            {
                Console.WriteLine("Введите дефолтную цену для открытого зала");
                var price = ReadNatural();
                hall.DefaultPrices = Enumerable.Range(0, hall.N).Select(_ => Enumerable.Repeat(price, hall.M).ToArray()).ToArray();
            }
            else
            {
                hall.DefaultPrices = SettingPrices(hall.N);
            }
            hall.Sessions = SettingHallsSessions(hall);
        }


        static void SettingHallsSize(string hallName, out int N, out int M)
        {
            Console.WriteLine();
            Console.WriteLine($"Введите через пробел размер зала {hallName}:");
            var sizes = ReadArray(2);

            N = sizes[0];
            M = sizes[1];
            Console.WriteLine();
        }


        static int[][] SettingPrices(int N, string pricesString = "дефолтные", string hallsString = "данного зала")
        {
            Console.WriteLine($"В следующих {N} строках введите через пробел {pricesString} стоимости мест по рядам для {hallsString}:");

            var prices = new int[N][];

            for (int i = 0; i < N; i++)
            {
                prices[i] = ReadArray(0);
            }

            Console.WriteLine();

            return prices;
        }


        static List<FilmSession> SettingHallsSessions(Hall hall)
        {
            Console.WriteLine("Введите количество временных сеансов/слотов для этого зала:");
            var sessionsNumber = ReadNatural();

            Console.WriteLine();


            var sessions = new List<FilmSession>();

            Console.WriteLine($"В следующих {sessionsNumber} строках введите дату и время сеансов в аналогичном формате: 05.02.2022 18:35");

            for (int i = 0; i < sessionsNumber; i++)
            {
                sessions.Add(new FilmSession(ReadDate(), hall));
            }

            Console.WriteLine();

            sessions = sessions
                .OrderBy(session => session.Date)
                .ToList();

            foreach (var session in sessions)
            {
                SettingOneHallsSession(hall, session);
            }

            return sessions;
        }

        static void SettingOneHallsSession(Hall hall, FilmSession session)
        {
            session.Places = new Place[hall.N][];

            for (int i = 0; i < hall.N; i++)
            {
                session.Places[i] = new Place[hall.M];

                for (int j = 0; j < hall.M; j++)
                {
                    var isBought = false;
                    string nameOfBuyer = null;
                    int? boughtAtPrice = null;
                    if (isLoad)
                    {
                        isBought = bool.Parse(reader.ReadLine());
                        if (isBought)
                        {
                            nameOfBuyer = reader.ReadLine();
                            boughtAtPrice = int.Parse(reader.ReadLine());
                        }
                    }
                    session.Places[i][j] = new Place
                    {
                        Row = i + 1,
                        Column = j + 1,
                        Price = hall.DefaultPrices[i][j],
                        Session = session,
                        IsBought = isBought,
                        NameOfBuyer = nameOfBuyer,
                        BoughtAtPrice = boughtAtPrice
                    };
                }
            }
        }


        static void GettingHallsAndSessionsLists()
        {
            allHalls = films
                .SelectMany(film => film.Halls)
                .ToList();

            allFilmSessions = allHalls
                .SelectMany(hall => hall.Sessions)
                .ToList();
        }




        static void MainProgramLoop()
        {
            while (true)
            {
                Console.Clear();

                var result = ChoosingInterface();

                if (result == false)
                {
                    UserInterface();
                }
                else if (result == true)
                {
                    AdminInterface();
                }
                else
                {
                    break;
                }
            }
        }

        static int ReadNatural()
        {
            uint input;
            while (true)
            {
                try
                {
                    if (isLoad)
                    {
                        input = uint.Parse(reader.ReadLine());
                    }
                    else
                    {
                        input = uint.Parse(Console.ReadLine());
                    }
                    break;
                }
                catch
                {
                    Console.WriteLine("Требуется натуральное число");
                }
            }
            return (int) input;
        }

        static string ReadRating()
        {
            string rating;
            while (true)
            {
                if (isLoad)
                {
                    rating = reader.ReadLine();
                }
                else
                {
                    rating = Console.ReadLine();
                }
                if (rating.Equals("0+") || rating.Equals("6+") ||
                    rating.Equals("12+") || rating.Equals("18+") || rating.Equals("16+")) break;
                else Console.WriteLine("Введите корректный возрастной рейтинг(0+, 6+, 12+, 16+, 18+)");
            }
            return rating;
        }

        static string ReadDate()
        {
            string data;
            while (true)
            {
                try
                {
                    if (isLoad)
                    {
                        data = reader.ReadLine();
                    }
                    else
                    {
                        data = Console.ReadLine();
                    }
                    DateTime.Parse(data);
                    break;
                }
                catch
                {
                    Console.WriteLine("Введите коректное дата и время сеанса");
                }
            }
            return data;
        }

        static int[] ReadArray(int size)
        {
            int[] arr;
            while (true)
            {
                try
                {
                    string arrString;
                    if (isLoad)
                    {
                        arrString = reader.ReadLine();
                    }
                    else
                    {
                        arrString = Console.ReadLine();
                    }
                   arr = arrString
                        .Split()
                        .Select(int.Parse)
                        .ToArray();
                    if (size == 0)
                    {
                        break;
                    }
                    if (arr.Length != size)
                    {
                        continue;
                    }
                    break;
                }
                catch
                {
                    Console.WriteLine($"Вводите {size} натуральных числел разделенных пробелами");
                }
            }
            return arr;
        }


        static bool? ChoosingInterface()
        {
            Console.WriteLine("Добро пожаловать на онлайн-платформу нашего кинотеатра!");
            Console.WriteLine();
            Console.WriteLine("Вы хотите запустить программу как админ или как клиент?");
            Console.WriteLine("Для продолжения как админ нажмите 1, для продолжения как клиент нажмите 2, для выхода - любую другую клавишу");

            var key = Console.ReadKey().Key;

            Console.WriteLine();
            Console.WriteLine();

            if (key == ConsoleKey.D1)
            {
                return true;
            }
            else if (key == ConsoleKey.D2)
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        static void UserInterface()
        {
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
                Console.WriteLine("НАжмите 5 для того чтобы купить напиток в баре");
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

                else if (key == ConsoleKey.D5)
                {
                    balance =  BuyDrink(balance);
                }

                else
                {
                    break;
                }
            }
        }

        static int BuyDrink(int balance)
        {
            balance -= Bar.BuyDrink(balance);
            PressAnyKey();
            return balance;
        }


        static int GetUserBalance()
        {
            Console.WriteLine("Укажите текущий баланс счета:");
 
            var answer = ReadNatural();

            Console.WriteLine();

            return answer;
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

        static void PressAnyKey()
        {
            Console.WriteLine("Нажмите любую кнопку для продолжения");

            Console.ReadKey();
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
                int[] requestedPlace;
                try
                {
                    requestedPlace = request
                        .Split()
                        .Select(int.Parse)
                        .ToArray();
                }
                catch
                {
                    return requestedPlaces;
                }

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

            var newBalance = balance + ReadNatural();

            Console.WriteLine();
            Console.WriteLine("Баланс успешно пополнен!");
            Console.WriteLine($"Ваш текущий баланс: {newBalance}");

            Console.WriteLine();

            PressAnyKey();

            return newBalance;
        }

        
        static void AdminInterface()
        {
            if (!CheckingPassword())
            {
                Console.WriteLine("К сожалению, указан неверный пароль");
                Console.WriteLine();

                PressAnyKey();
            }
            else
            {
                while (true)
                {
                    Console.Clear();

                    Console.WriteLine("Добро пожаловать в интерфейс администратора!");
                    Console.WriteLine();

                    Console.WriteLine("Что вы хотите сделать?");
                    Console.WriteLine("Нажмите 1 для редактирования информации о фильмах/залах/сеансах");
                    Console.WriteLine("Нажмите 2 для просмотра аналитики по продажам");
                    Console.WriteLine("Нажмите 3 для просмотра клиентской аналитики");
                    Console.WriteLine("Нажмите 4 для редактирования информации о баре");
                    Console.WriteLine("Нажмите любую другую кнопку для завершения сеанса администратора");

                    var key = Console.ReadKey().Key;

                    Console.Clear();

                    if (key == ConsoleKey.D1)
                    {
                        AdminEditingMenu();
                    }

                    else if (key == ConsoleKey.D2)
                    {
                        SessionsAnalytics();
                    }

                    else if (key == ConsoleKey.D3)
                    {
                        ClientsAnalytics();
                    }

                    else if (key == ConsoleKey.D4)
                    {
                        EditBarMenu();
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        static void EditBarMenu()
        {
            Console.WriteLine("Нажмите 1 для добавления нового напитка");
            Console.WriteLine("Нажмите 2 для удаления какого-либо напитка");

            var key = Console.ReadKey().Key;

            Console.Clear();

            if (key == ConsoleKey.D1)
            {
                AddingDrink(false);
            }

            else if (key == ConsoleKey.D2)
            {
                DeletingDrink();
            }
        }

        static void AddingDrink(bool initial)
        {
            Console.WriteLine("Добавление напитка");
            Console.WriteLine();


            Console.WriteLine("Введите название напитка:");

            string name;
            if (isLoad)
            {
                name = reader.ReadLine();
            }
            else
            {
                name = Console.ReadLine();
            }

            Console.WriteLine();

            Console.WriteLine("Введите цену напитка:");

            var price = ReadNatural();

            Console.WriteLine();

            Console.WriteLine("Новый напиток алкогольный? (Yes/No)");

            while (true)
            {
                string answer;
                if (isLoad)
                {
                    answer = reader.ReadLine();
                }
                else
                {
                    answer = Console.ReadLine();
                }
                if (answer.Equals("Yes"))
                {
                    Console.WriteLine("Введите градус напинтка");
                    var degree = ReadNatural();
                    Bar.AddNewDrink(name, price, true, degree);
                    break;
                } 
                else if (answer.Equals("No"))
                {
                    Bar.AddNewDrink(name, price, false, 0);
                    break;
                } 
                else
                {
                    Console.WriteLine("Требуется ввести ответ \"Yes\" или \"No\"");
                }
            }

            Console.WriteLine();
            Console.WriteLine();
            if (!initial)
            {
                Console.WriteLine("Напиток успешно добавлен!");
                Console.WriteLine();

                PressAnyKey();
            }
        }

        static void DeletingDrink()
        {
            Console.WriteLine("Удаление напитка");
            Console.WriteLine();


            Console.WriteLine("Введите название напитка, который вы бы хотели удалить");

            var name = Console.ReadLine();
            var drink = Bar.Drinks.FirstOrDefault(f => f.Name == name);

            Console.WriteLine();

            if (drink == null)
            {
                Console.WriteLine("К сожалению, такого напитка нет в базе");

                Console.WriteLine();
                Console.WriteLine();

                PressAnyKey();
            }
            else
            {
                Bar.Drinks.Remove(drink);

                Console.WriteLine("Напиток успешно удален!");
                Console.WriteLine();

                PressAnyKey();
            }
        }

        static bool CheckingPassword()
        {
            Console.WriteLine("Введите пароль администратора:");

            var pass = Console.ReadLine();

            Console.WriteLine();

            return pass == password;
        }

        static void AdminEditingMenu()
        {
            Console.WriteLine("Меню редактирования информации в кинотеатре");
            Console.WriteLine();

            Console.WriteLine("Что вы хотите сделать?");
            Console.WriteLine("Нажмите 1 для добавления нового фильма");
            Console.WriteLine("Нажмите 2 для редактирования информации, связанной с каким-то фильмом (в т.ч. залы, сеансы, ...)");
            Console.WriteLine("Нажмите 3 для удаления какого-либо фильма");
            Console.WriteLine("Чтобы вернуться в меню администратора, нажмите любую другую кнопку");

            var key = Console.ReadKey().Key;

            Console.Clear();

            if (key == ConsoleKey.D1)
            {
                AddingFilm();
            }

            else if (key == ConsoleKey.D2)
            {
                EdittingFilm();
            }

            else if (key == ConsoleKey.D3)
            {
                DeletingFilm();
            }
        }


        static void AddingFilm()
        {
            Console.WriteLine("Добавление фильма");
            Console.WriteLine();


            Console.WriteLine("Введите название фильма:");

            var film = new Film(Console.ReadLine());

            Console.WriteLine();


            Console.WriteLine("Введите возрастной рейтинг для этого фильма:");

            film.Rating = ReadRating();

            Console.WriteLine();
            Console.WriteLine();


            Console.WriteLine("Теперь укажем информацию о залах фильма:");

            film.Halls = InitializingHalls(initial: false);

            SettingHallsParameters(film);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();


            films.Add(film);
            GettingHallsAndSessionsLists();

            Console.WriteLine("Фильм успешно добавлен!");
            Console.WriteLine();

            PressAnyKey();           
        }



        static void EdittingFilm()
        {
            Console.WriteLine("Редактирование данных о фильме");
            Console.WriteLine();


            Console.WriteLine("Введите название фильма, данные по которому вы бы хотели отредактировать");

            var name = Console.ReadLine();
            var film = films.FirstOrDefault(f => f.Name == name);

            Console.WriteLine();

            if (film == null)
            {
                Console.WriteLine("К сожалению, такого фильма нет в базе");

                Console.WriteLine();
                Console.WriteLine();

                PressAnyKey();
            }
            else
            {
                EdittingFilmMenu(film);
            }
        }


        static void EdittingFilmMenu(Film film)
        {
            ShowingSessions(selectedFilm: film);

            Console.WriteLine("Укажите, какую информацию вы бы хотели отредактировать?");
            Console.WriteLine("Нажмите 1 для редактирования названия фильма");
            Console.WriteLine("Нажмите 2 для редактирования возрастного рейтинга фильма");
            Console.WriteLine("Нажмите 3 для изменения какой-либо информации о демонстрации фильма (залы, сеансы, ...)");
            Console.WriteLine("Чтобы вернуться в меню администратора, нажмите любую другую кнопку");

            var key = Console.ReadKey().Key;

            Console.WriteLine();
            Console.WriteLine();

            if (key == ConsoleKey.D1)
            {
                Console.WriteLine("Укажите новое название для этого фильма");

                film.Name = Console.ReadLine();

                Console.WriteLine();
                Console.WriteLine("Название успешно изменено!");
                Console.WriteLine();

                PressAnyKey();
            }

            else if (key == ConsoleKey.D2)
            {
                if (CheckingIfThereAreBoughtTickets(film))
                {
                    BoughtTicketsWarning("на этот фильм");
                }
                else
                {
                    Console.WriteLine("Укажите новый возрастной рейтинг для фильма");

                    film.Rating = ReadRating();

                    Console.WriteLine();
                    Console.WriteLine("Возрастной рейтинг успешно изменен!");
                    Console.WriteLine();

                    PressAnyKey();
                }
            }

            else if (key == ConsoleKey.D3)
            {
                EditHalls(film);
            }
        }

        static bool CheckingIfThereAreBoughtTickets(Film film = null, Hall hall = null, FilmSession session = null)
        {
            if (film != null)
            {
                return film.Halls.Exists(
                    h => h.Sessions.Exists(
                        ses => ses.Places.Any(
                            row => row.Any(
                                pl => pl.IsBought))));
            }

            if (hall != null)
            {
                return hall.Sessions.Exists(
                        ses => ses.Places.Any(
                            row => row.Any(
                                pl => pl.IsBought)));
            }

            if (session != null)
            {
                return session.Places.Any(
                            row => row.Any(
                                pl => pl.IsBought));
            }

            return false;
        }

        static void BoughtTicketsWarning(string str)
        {
            Console.WriteLine(
                $"К сожалению, вы не можете изменить эти данные, так как некоторые билеты {str} были уже куплены");

            Console.WriteLine();

            PressAnyKey();
        }


        static void EditHalls(Film film)
        {
            Console.WriteLine("Что вы хотите сделать?");
            Console.WriteLine("Нажмите 1 для добавления нового зала");
            Console.WriteLine("Нажмите 2 для редактирования информации о каком либо зале (в т.ч. сеансы, цены на билеты)");
            Console.WriteLine("Нажмите 3 для удаления какого-либо зала");
            Console.WriteLine("Чтобы вернуться в меню администратора, нажмите любую другую кнопку");

            var key = Console.ReadKey().Key;

            Console.WriteLine();
            Console.WriteLine();

            if (key == ConsoleKey.D1)
            {
                AddingHall(film);
            }

            else if (key == ConsoleKey.D2)
            {
                EdittingHall(film);
            }

            else if (key == ConsoleKey.D3)
            {
                DeletingHall(film);
            }
        }


        static void AddingHall(Film film)
        {
            Console.WriteLine("Введите название зала:");

            var hall = new Hall(Console.ReadLine());

            Console.WriteLine();

            SettingOneHallParameters(film, hall);

            film.Halls.Add(hall);
            GettingHallsAndSessionsLists();

            Console.WriteLine();
            Console.WriteLine("Зал успешно добавлен!");
            Console.WriteLine();

            PressAnyKey();
        }


        static void EdittingHall(Film film)
        {
            Console.WriteLine("Введите название зала (вместе с техническими символами, если есть), данные по которому вы бы хотели отредактировать");

            var name = Console.ReadLine();
            var hall = film.Halls.FirstOrDefault(h => h.Name == name);

            Console.WriteLine();

            if (hall == null)
            {
                Console.WriteLine("К сожалению, у данного фильма нет такого зала");

                Console.WriteLine();
                Console.WriteLine();

                PressAnyKey();
            }
            else
            {
                EdittingHallMenu(hall);
            }
        }

        static void EdittingHallMenu(Hall hall)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Укажите, какую информацию вы бы хотели отредактировать?");
            Console.WriteLine("Нажмите 1 для редактирования названия зала");
            Console.WriteLine("Нажмите 2 для редактирования цен на все сеансы этого зала");
            Console.WriteLine("Нажмите 3 для изменения сеансов (добавление новых, удаление существующих, редактирование)");
            Console.WriteLine("Чтобы вернуться в меню администратора, нажмите любую другую кнопку");

            var key = Console.ReadKey().Key;

            Console.WriteLine();
            Console.WriteLine();

            if (key == ConsoleKey.D1)
            {
                Console.WriteLine("Укажите новое название для этого зала");

                hall.Name = Console.ReadLine();

                Console.WriteLine();
                Console.WriteLine("Название успешно изменено!");
                Console.WriteLine();

                PressAnyKey();
            }

            else if (key == ConsoleKey.D2)
            {
                if (CheckingIfThereAreBoughtTickets(hall: hall))
                {
                    BoughtTicketsWarning("на сеансы в этом зале");
                }
                else
                {
                    if (hall is OpenSpaceHall)
                    {
                        Console.WriteLine("Введите новую дефолтную цену для открытого зала");
                        hall.DefaultPrices = Enumerable.Range(0, hall.N).Select(_ => Enumerable.Repeat(ReadNatural(), hall.M).ToArray()).ToArray();
                    }
                    else
                    {
                        hall.DefaultPrices = SettingPrices(hall.N, "новые", "сеансов данного зала");
                    }
                    SwitchingSessionsPricesToNewDefault(hall);

                    Console.WriteLine();
                    Console.WriteLine("Цены успешно изменены!");
                    Console.WriteLine();

                    PressAnyKey();
                }
            }

            else if (key == ConsoleKey.D3)
            {
                EditSessions(hall);
            }
        }


        static void SwitchingSessionsPricesToNewDefault(Hall hall)
        {
            foreach (var session in hall.Sessions)
            {
                for (int i = 0; i < hall.N; i++)
                {
                    for (int j = 0; j < hall.M; j++)
                    {
                        session.Places[i][j].Price = hall.DefaultPrices[i][j];
                    }
                }
            }
        }



        static void EditSessions(Hall hall)
        {
            Console.WriteLine("Что вы хотите сделать?");
            Console.WriteLine("Нажмите 1 для добавления нового сеанса");
            Console.WriteLine("Нажмите 2 для редактирования информации о каком либо сеансе (в т.ч. цены на билеты)");
            Console.WriteLine("Нажмите 3 для удаления какого-либо сеанса");
            Console.WriteLine("Чтобы вернуться в меню администратора, нажмите любую другую кнопку");

            var key = Console.ReadKey().Key;

            Console.WriteLine();
            Console.WriteLine();

            if (key == ConsoleKey.D1)
            {
                AddingSession(hall);
            }

            else if (key == ConsoleKey.D2)
            {
                if (hall.Sessions.Any(ses => ses.Date >= DateTime.Now))
                {
                    EdittingSession(hall);
                }
                else
                {
                    Console.WriteLine("К сожалению, все сеансы по этому фильму уже прошли, поэтому их нельзя отредактировать");

                    PressAnyKey();
                }                
            }

            else if (key == ConsoleKey.D3)
            {
                DeletingSession(hall);
            }
        }


        static void AddingSession(Hall hall)
        {
            Console.WriteLine("Введите дату и время сеанса в аналогичном формате: 05.02.2022 18:35");

            var session = new FilmSession(ReadDate(), hall);

            Console.WriteLine();

            SettingOneHallsSession(hall, session);

            hall.Sessions.Add(session);
            GettingHallsAndSessionsLists();

            Console.WriteLine();
            Console.WriteLine("Сеанс успешно добавлен!");
            Console.WriteLine();

            PressAnyKey();
        }


        static void EdittingSession(Hall hall)
        {
            PrintSessions(hall);

            Console.WriteLine("Укажите номер сеанса, информацию по которому вы бы хотели отредактировать");
            var number = 0;
            while (true)
            {
                number = ReadNatural() - 1;
                if (number >= 0 && number < hall.Sessions.Count)
                {
                    break;
                } 
                else
                {
                    Console.WriteLine($"Введите коректный номер сеанса от 1 до {hall.Sessions.Count}");
                }
            }
            var session = hall.Sessions[number - 1];

            if (session.Date < DateTime.Now)
            {
                Console.WriteLine();
                Console.WriteLine("К сожалению, данный сеанс уже прошел, поэтому его нельзя отредактировать");
                Console.WriteLine();
                Console.WriteLine();

                PressAnyKey();
            }            
            else
            {
                EdittingSessionMenu(session);
            }
        }

        static void PrintSessions(Hall hall)
        {
            var index = 0;

            foreach (var session in hall.Sessions)
            {
                index++;

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($"{index}.  ");
                Console.ResetColor();

                session.PrintSession();
            }
        }


        static void EdittingSessionMenu(FilmSession session)
        {
            Console.WriteLine();
            Console.WriteLine();

            session.PrintSessionFullInformation();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Укажите, какую информацию вы бы хотели отредактировать?");
            Console.WriteLine("Нажмите 1 для редактирования даты и времени сеанса");
            Console.WriteLine("Нажмите 2 для задания новой матрицы цен");
            Console.WriteLine("Нажмите 3 для изменения цены конкретного места");
            Console.WriteLine("Чтобы вернуться в меню администратора, нажмите любую другую кнопку");

            var key = Console.ReadKey().Key;

            Console.WriteLine();
            Console.WriteLine();

            if (key == ConsoleKey.D1)
            {
                if (CheckingIfThereAreBoughtTickets(session: session))
                {
                    BoughtTicketsWarning("на этот сеанс");
                }
                else
                {
                    Console.WriteLine("Укажите новую дату и время сеанса в аналогичном формате: 05.02.2022 18:35");

                    var date = DateTime.Parse(ReadDate());

                    if (date < DateTime.Now)
                    {
                        Console.WriteLine();
                        Console.WriteLine("К сожалению, это время уже прошло");
                        Console.WriteLine();

                        PressAnyKey();
                    }
                    else
                    {
                        session.Date = date;

                        Console.WriteLine();
                        Console.WriteLine("Дата сеанса успешно изменена!");
                        Console.WriteLine();

                        PressAnyKey();
                    }
                }
            }

            else if (key == ConsoleKey.D2)
            {
                if (CheckingIfThereAreBoughtTickets(session: session))
                {
                    BoughtTicketsWarning("на этот сеанс");
                }
                else
                {
                    int[][] prices;
                    if (session.Hall is OpenSpaceHall)
                    {
                        Console.WriteLine("Введите дефолтную цену для данного сеанса");
                        prices = Enumerable.Range(0, session.Hall.N).Select(_ => Enumerable.Repeat(ReadNatural(), session.Hall.M).ToArray()).ToArray();
                    }
                    else
                    {
                        prices = SettingPrices(session.Hall.N, "новые", "данного сеанса");
                    }
                    SwitchingSessionPrices(session, prices);

                    Console.WriteLine();
                    Console.WriteLine("Цены успешно изменены!");
                    Console.WriteLine();

                    PressAnyKey();
                }
            }

            else if (key == ConsoleKey.D3)
            {
                Console.WriteLine("Введите в строку через пробел номер ряда и места, цену которого хотите изменить");

                var place = ReadArray(2);

                var row = place[0];
                var column = place[1];

                Console.WriteLine();

                if (!(row >= 1 & row <= session.Hall.N &
                    column >= 1 & column <= session.Hall.M))
                {                    
                    Console.WriteLine("Такого места нет в зале");
                    Console.WriteLine();

                    PressAnyKey();
                }

                else if (session.Places[row - 1][column - 1].IsBought)
                {
                    Console.WriteLine("К сожалению, нельзя изменить цену этого места, так как оно уже куплено");
                    Console.WriteLine();

                    PressAnyKey();
                }

                else
                {
                    Console.WriteLine("Укажите новую цену для данного места:");

                    session.Places[row - 1][column - 1].Price = ReadNatural();

                    Console.WriteLine();
                    Console.WriteLine("Цена успешно изменена!");
                    Console.WriteLine();

                    PressAnyKey();
                }
            }
        }


        static void SwitchingSessionPrices(FilmSession session, int[][] prices)
        {
            for (int i = 0; i < session.Hall.N; i++)
            {
                for (int j = 0; j < session.Hall.M; j++)
                {
                    session.Places[i][j].Price = prices[i][j];
                }
            }            
        }



        static void DeletingFilm()
        {
            Console.WriteLine("Удаление фильма");
            Console.WriteLine();


            Console.WriteLine("Введите название фильма, который вы бы хотели удалить");

            var name = Console.ReadLine();
            var film = films.FirstOrDefault(f => f.Name == name);

            Console.WriteLine();

            if (film == null)
            {
                Console.WriteLine("К сожалению, такого фильма нет в базе");

                Console.WriteLine();
                Console.WriteLine();

                PressAnyKey();
            }
            else if (CheckingIfThereAreBoughtTickets(film: film))
            {
                DeletingBoughtTicketsWarning("фильм");
            }
            else
            {
                films.Remove(film);
                GettingHallsAndSessionsLists();

                Console.WriteLine("Фильм успешно удален!");
                Console.WriteLine();

                PressAnyKey();
            }
        }

        static void DeletingHall(Film film)
        {
            Console.WriteLine("Введите название зала (вместе с техническими символами, если есть), который вы бы хотели удалить");

            var name = Console.ReadLine();
            var hall = film.Halls.FirstOrDefault(h => h.Name == name);

            Console.WriteLine();

            if (hall == null)
            {
                Console.WriteLine("К сожалению, у данного фильма нет такого зала");

                Console.WriteLine();
                Console.WriteLine();

                PressAnyKey();
            }
            else if (CheckingIfThereAreBoughtTickets(hall: hall))
            {
                DeletingBoughtTicketsWarning("зал");
            }
            else
            {
                film.Halls.Remove(hall);
                GettingHallsAndSessionsLists();

                Console.WriteLine("Зал успешно удален!");
                Console.WriteLine();

                PressAnyKey();
            }
        }

        static void DeletingSession(Hall hall)
        {
            PrintSessions(hall);

            Console.WriteLine("Укажите номер сеанса, который вы бы хотели удалить");

            var number = 0;
            while (true)
            {
                number = ReadNatural() - 1;
                if (number >= 0 && number < hall.Sessions.Count)
                {
                    break;
                }
                else
                {
                    Console.WriteLine($"Введите коректный номер сеанса от 1 до {hall.Sessions.Count}");
                }
            }
            var session = hall.Sessions[number - 1];

            Console.WriteLine();

            if (session.Date < DateTime.Now)
            {                
                Console.WriteLine("К сожалению, данный сеанс уже прошел, поэтому его нельзя удалить");

                Console.WriteLine();
                Console.WriteLine();

                PressAnyKey();
            }
            else if (CheckingIfThereAreBoughtTickets(session: session))
            {
                DeletingBoughtTicketsWarning("сеанс");
            }
            else
            {
                hall.Sessions.Remove(session);
                GettingHallsAndSessionsLists();

                Console.WriteLine("Сеанс успешно удален!");
                Console.WriteLine();

                PressAnyKey();
            }
        }


        static void DeletingBoughtTicketsWarning(string str)
        {
            Console.WriteLine(
                $"К сожалению, вы не можете удалить этот {str}, так как некоторые билеты были уже куплены");

            Console.WriteLine();

            PressAnyKey();
        }




        static void SessionsAnalytics()
        {
            Console.WriteLine("Меню аналитики по продажам");
            Console.WriteLine();

            Console.WriteLine("При отображении аналитики по продажам вы можете задать один или несколько следующих фильтров:");
            Console.WriteLine("1. Данные по одному конкретному фильму");
            Console.WriteLine("2. Данные по одному конкретному залу");
            Console.WriteLine("3. Данные по одному конкретному сеансу");
            Console.WriteLine("4. Данные за определенный промежуток времени");
            Console.WriteLine("5. Данные по конкретной дате");
            Console.WriteLine("6. Данные по определенному временному интервалу в любую дату");
            Console.WriteLine("7. Данные по фильмам определенного возрастного рейтинга");
            Console.WriteLine();

            Console.WriteLine("Укажите через пробел номера фильтров, которые бы вы хотели задать");
            Console.WriteLine("Если вы хотите пропустить установку фильтров, просто нажмите Enter");
            Console.WriteLine();

            var answer = Console.ReadLine();

            Console.Clear();

            if (answer == "")
            {
                ShowingSessionAnalytics(allFilmSessions);
            }

            else
            {
                int[] filtersNumbers;
                while (true)
                {
                    try
                    {
                        filtersNumbers = answer
                            .Split()
                            .Select(int.Parse)
                            .ToArray();
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Вводите натуральные числа разделенные пробелами");
                        answer = Console.ReadLine();
                    }
                }
                var filteredSessions = FilteredSessionsByAdmin(filtersNumbers);

                Console.Clear();

                ShowingSessionAnalytics(filteredSessions);
            }            
        }


        static List<FilmSession> FilteredSessionsByAdmin(int[] filtersNumbers)
        {
            List<Func<FilmSession, bool>> filters = new List<Func<FilmSession, bool>>();

            if (filtersNumbers.Contains(1))
            {
                FilteringByFilm(filters,
                    "1. Фильтр по фильмам",
                    "Для отображения данных по продажам на конкретный фильм напишите его название",
                    "Если вы передумали задавать этот фильтр, просто нажмите Enter");
            }

            if (filtersNumbers.Contains(2))
            {
                FilteringByHall(filters,
                    "2. Фильтр по залам",
                    "Для отображения данных по продажам в конкретном зале, напишите его название (полностью, включая технические символы, если они есть)",
                    "Если вы передумали задавать этот фильтр, просто нажмите Enter");
            }

            if (filtersNumbers.Contains(3))
            {
                FilteringBySession(filters,
                    "3. Фильтр по сеансам",
                    "Для отображения данных по продажам на конкретный сеанс, напишите его номер из списка ниже",
                    "Если вы передумали задавать этот фильтр, просто нажмите Enter");
            }

            if (filtersNumbers.Contains(4))
            {
                FilteringByTimePeriod(filters,
                    "4. Фильтр по временному промежутку",
                    "Для отображения данных по продажам в течение определенного промежутка времени укажите в двух строках в формате 10.02.2022 13:00 начало и конец этого промежутка соответственно",
                    "Если вы передумали задавать этот фильтр, просто нажмите Enter");
            }

            if (filtersNumbers.Contains(5))
            {
                FilteringBySelectedDate(filters,
                    "5. Фильтр по дате",
                    "Для отображения данных по продажам за конкретный день, укажите этот день в формате ДД.ММ.ГГГГ",
                    "Если вы передумали задавать этот фильтр, просто нажмите Enter");
            }

            if (filtersNumbers.Contains(6))
            {
                FilteringByTimeInterval(filters,
                    "6. Фильтр по интервалу времени",
                    "Для отображения данных по продажам на сеансы в определенном интервале времени, укажите в двух строках в формате 13:00 начало и конец этого интервала соответственно",
                    "Если вы передумали задавать этот фильтр, просто нажмите Enter");
            }

            if (filtersNumbers.Contains(7))
            {
                FilteringByAgeRating(filters,
                    "7. Фильтр по возрастному рейтингу",
                    "Для отображения данных по продажам на фильмы определенного возрастного рейтинга, укажите этот возрастной рейтинг",
                    "Если вы передумали задавать этот фильтр, просто нажмите Enter");
            }


            var filteredSessions = allFilmSessions.Select(ses => ses);

            foreach (var filter in filters)
            {
                filteredSessions = filteredSessions.Where(filter);
            }

            return filteredSessions.ToList();
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


        static void FilteringBySession(List<Func<FilmSession, bool>> filters, string firstLine, string secondLine, string thirdLine)
        {
            Console.WriteLine(firstLine);
            Console.WriteLine();

            ShowingSessions(allSessions: true);

            Console.WriteLine(secondLine);
            Console.WriteLine(thirdLine);

            var answerTry = int.TryParse(Console.ReadLine(), out int answer);

            if (answerTry)
            {
                filters.Add(ses => ses == allFilmSessions[answer - 1]);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }


        static void FilteringByTimePeriod(List<Func<FilmSession, bool>> filters, string firstLine, string secondLine, string thirdLine)
        {
            Console.WriteLine(firstLine);
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine(secondLine);
            Console.WriteLine(thirdLine);

            var answerFirstLine = Console.ReadLine();

            if (answerFirstLine != "")
            {
                var answerBeginingTry = DateTime.TryParse(answerFirstLine, out DateTime answerBegining);
                var answerEndingTry = DateTime.TryParse(Console.ReadLine(), out DateTime answerEnding);

                if (answerBeginingTry & answerEndingTry & answerBegining <= answerEnding)
                {
                    filters.Add(ses => ses.Date >= answerBegining & ses.Date <= answerEnding);
                }
            }

            Console.WriteLine();
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


        static void FilteringByTimeInterval(List<Func<FilmSession, bool>> filters, string firstLine, string secondLine, string thirdLine)
        {
            Console.WriteLine(firstLine);
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine(secondLine);
            Console.WriteLine(thirdLine);

            var answerFirstLine = Console.ReadLine();
            int[] answerBegining;
            int[] answerEnding;

            if (answerFirstLine != "")
            {
                while (true)
                {
                    try
                    {
                        answerBegining = answerFirstLine
                            .Split(':')
                            .Select(int.Parse)
                            .ToArray();

                        answerEnding = Console.ReadLine()
                            .Split(':')
                            .Select(int.Parse)
                            .ToArray();
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Вводите натуральные числа разделенные :");
                        answerFirstLine = Console.ReadLine();
                    }
                }

                TimeSpan beginingTime = new TimeSpan(answerBegining[0], answerBegining[1], 0);
                TimeSpan endingTime = new TimeSpan(answerEnding[0], answerEnding[1], 0);

                filters.Add(ses => IsTimeOfDayBetween(
                    new TimeSpan(ses.Date.Hour, ses.Date.Minute, 0), 
                    beginingTime, 
                    endingTime));
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        static bool IsTimeOfDayBetween(TimeSpan time, TimeSpan startTime, TimeSpan endTime)
        {
            if (endTime == startTime)
            {
                return true;
            }
            else if (endTime < startTime)
            {
                return time <= endTime | time >= startTime;
            }
            else
            {
                return time >= startTime && time <= endTime;
            }
        }


        static void FilteringByAgeRating(List<Func<FilmSession, bool>> filters, string firstLine, string secondLine, string thirdLine)
        {
            Console.WriteLine(firstLine);
            Console.WriteLine();

            ShowingRatingsList();

            Console.WriteLine(secondLine);
            Console.WriteLine(thirdLine);

            var answer = ReadRating();

            if (answer != "")
            {
                filters.Add(ses => ses.Hall.Film.Rating == answer);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        static void ShowingRatingsList()
        {
            Console.WriteLine("Возрастные рейтинги фильмов кинотеатра:");
            Console.WriteLine();

            var ratings = films
                .Select(film => film.Rating)
                .Distinct();

            foreach (var rating in ratings)
            {
                Console.WriteLine(rating);
            }

            Console.WriteLine();
            Console.WriteLine();
        }


        static void FilteringByCurrentDate(List<Func<FilmSession, bool>> filters)
        {
            filters.Add(ses => ses.Date > DateTime.Now);
        }

        static void FilteringByAllBoughtPlaces(List<Func<FilmSession, bool>> filters)
        {
            filters.Add(ses => ses.Places.Any(row => row.Any(pl => !pl.IsBought)));
        }



        static void ShowingSessionAnalytics(List<FilmSession> sessions)
        {
            TotalNumberOfPlaces(sessions);
            PercentageOfVacantPlaces(sessions);
            TotalRevenue(sessions);

            PressAnyKey();
        }

        static void TotalNumberOfPlaces(List<FilmSession> sessions)
        {
            Console.WriteLine("Общее количество мест по выбранным залам:");

            var totalNumberOfPlaces = sessions.Sum(ses => ses.Hall.N * ses.Hall.M);

            Console.WriteLine(totalNumberOfPlaces);
            Console.WriteLine();
            Console.WriteLine();


            Console.WriteLine("Общее количество свободных мест по выбранным залам:");

            var totalNumberOfVacantPlaces = sessions.Sum(
                ses => ses.Places.Sum(
                    row => row.Count(
                        place => !place.IsBought)));

            Console.WriteLine(totalNumberOfVacantPlaces);
            Console.WriteLine();
            Console.WriteLine();


            Console.WriteLine("Общее количество занятых мест по выбранным залам:");

            Console.WriteLine(totalNumberOfPlaces - totalNumberOfVacantPlaces);
            Console.WriteLine();
            Console.WriteLine();
        }

        static void PercentageOfVacantPlaces(List<FilmSession> sessions)
        {
            Console.WriteLine("Средний процент заполненности зрительных залов:");

            if (sessions.Count > 0)
            {
                var percentage = sessions.Average(
                    ses => ses.Places.Sum(row => row.Count(place => place.IsBought)) /
                            (double)(ses.Hall.N * ses.Hall.M));

                Console.WriteLine($"{percentage * 100} %");
            }
            
            else
            {
                Console.WriteLine("-");
            }
            
            Console.WriteLine();
            Console.WriteLine();
        }

        static void TotalRevenue(List<FilmSession> sessions)
        {
            Console.WriteLine("Общая выручка по проданным билетам:");

            var revenue = sessions
                .Sum(ses => ses.Places
                    .Sum(row => row
                        .Where(pl => pl.IsBought)
                        .Sum(pl => pl.BoughtAtPrice)));

            Console.WriteLine(revenue);
            Console.WriteLine();
            Console.WriteLine();
        }

        

        static void ClientsAnalytics()
        {
            Console.WriteLine("Клиентская аналитика");
            Console.WriteLine();

            var soldTickets = allFilmSessions
                .SelectMany(ses => ses.Places.SelectMany(row => row))
                .Where(place => place.IsBought)
                .ToList();

            var clients = soldTickets
                .Select(tick => tick.NameOfBuyer)
                .Distinct()
                .ToList();

            Console.WriteLine("Общее количество клиентов, покупавших билеты в нашем кинотеатре:");
            Console.WriteLine(clients.Count());

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Укажите число наиболее лояльных клиентов (по нескольким параметрам), которое бы вы хотели отобразить в аналитике (Топ-N):");

            var number = ReadNatural();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            TopSoldTicketsClients(soldTickets, clients, number);
            TopNumberOfSessionsClients(clients, number);
            TopSpentMoneyClients(soldTickets, clients, number);

            PressAnyKey();
        }

        static void TopSoldTicketsClients(List<Place> soldTickets, List<string> clients, int number)
        {
            clients = clients
                .OrderByDescending(cl => soldTickets.Count(tick => tick.NameOfBuyer == cl))
                .ToList();

            PrintingClients(clients, number,
                "по количеству купленных билетов:");
        }

        static void TopNumberOfSessionsClients(List<string> clients, int number)
        {
            clients = clients
                .OrderByDescending(cl => allFilmSessions.Count(
                    ses => ses.Places.Any(
                        row => row.Any(
                            pl => pl.IsBought & pl.NameOfBuyer == cl))))
                .ToList();

            PrintingClients(clients, number,
                "по количеству различных сеансов, на которые были куплены билеты:");
        }

        static void TopSpentMoneyClients(List<Place> soldTickets, List<string> clients, int number)
        {
            clients = clients
                .OrderByDescending(cl => soldTickets.Where(tick => tick.NameOfBuyer == cl).Sum(tick => tick.BoughtAtPrice))
                .ToList();
            
            PrintingClients(clients, number,
                "по общей сумме потраченных денег на билеты:");
        }

        static void PrintingClients(List<string> clients, int number, string endOfTheLine)
        {
            Console.WriteLine($"Топ-{number} клиентов " + endOfTheLine);
            Console.WriteLine();

            foreach (var client in clients.Take(number))
            {
                Console.WriteLine(client);
            }

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
