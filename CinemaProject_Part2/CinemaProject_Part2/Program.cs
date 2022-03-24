using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CinemaProject_Part2
{
    internal class Program
    {
        const string password = "pass";

        static List<Film> films;
        static List<Hall> allHalls;
        static List<FilmSession> allFilmSessions;
        static StringBuilder file;
        static StreamReader sc;



        static void Main()
        {
            Initializing();
            Console.ReadKey();
        }


        static void Initializing()
        {
            if (File.Exists("cinema.txt"))
            {
                sc = new StreamReader("cinema.txt");
            }
            else
            {
                sc = new StreamReader(Console.OpenStandardInput());
            }
           
            file = new StringBuilder();
            InitializingFilms();
            SettingFilmsHalls();
            GettingHallsAndSessionsLists();

            MainProgramLoop();
        }


        static void InitializingFilms()
        {
            Console.WriteLine("Введите количество фильмов:");
            uint filmsNumber = 0;
            while (true)
            {
                try
                {
                    filmsNumber = uint.Parse(sc.ReadLine());
                    break;
                }
                catch
                {
                    Console.WriteLine("Требуется натуральное число");
                }
            }
            Console.WriteLine();

            films = new List<Film>();

            Console.WriteLine($"В следующих {filmsNumber} строках введите названия этих фильмов:");

            for (int i = 0; i < filmsNumber; i++)
            {
                var filmName = sc.ReadLine();
                films.Add(new Film(filmName));
            }

            Console.WriteLine();


            Console.WriteLine($"В следующих {filmsNumber} строках введите возрастные рейтинги для этих фильмов:");

            for (int i = 0; i < filmsNumber; i++)
            {
                while (true)
                {
                    films[i].Rating = sc.ReadLine();
                    if (films[i].Rating.Equals("0+") || films[i].Rating.Equals("6+") ||
                        films[i].Rating.Equals("12+") || films[i].Rating.Equals("18+") || films[i].Rating.Equals("16+")) break;
                    else Console.WriteLine("Введите корректный возрастной рейтинг(0+, 6+, 12+, 16+, 18+)");
                }
                file.AppendLine(films[i].Rating);
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
            uint hallsNumber;
            while (true)
            {
                try
                {
                    hallsNumber = uint.Parse(sc.ReadLine());
                    break;
                }
                catch
                {
                    Console.WriteLine("Требуется натуральное число");
                }
            }

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
                var hallName = sc.ReadLine();
                halls.Add(new Hall(hallName));
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

            hall.DefaultPrices = SettingPrices(hall.N);
            hall.Sessions = SettingHallsSessions(hall);
        }


        static void SettingHallsSize(string hallName, out int N, out int M)
        {
            Console.WriteLine();
            Console.WriteLine($"Введите через пробел размер зала {hallName}:");
            while (true)
            {
                try
                {
                    var sizes = sc.ReadLine().Split();

                    N = (int) uint.Parse(sizes[0]);
                    M = (int) uint.Parse(sizes[1]);
                    break;
                }
                catch
                {
                    Console.WriteLine("Введите 2 натуральных числа");
                }
            }
            Console.WriteLine();
        }


        static int[][] SettingPrices(int N, string pricesString = "дефолтные", string hallsString = "данного зала")
        {
            Console.WriteLine($"В следующих {N} строках введите через пробел {pricesString} стоимости мест по рядам для {hallsString}:");

            var prices = new int[N][];

            for (int i = 0; i < N; i++)
            {
                while (true)
                {
                    try
                    {
                        prices[i] = sc.ReadLine()
                            .Split()
                            .Select(int.Parse)
                            .ToArray();
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Вводите натуральные числа разделенные пробелами");
                    }
                }
            }

            Console.WriteLine();

            return prices;
        }


        static List<FilmSession> SettingHallsSessions(Hall hall)
        {
            Console.WriteLine("Введите количество временных сеансов/слотов для этого зала:");
            var sessionsNumber = 0;
            while (true)
            {
                try
                {
                    sessionsNumber = (int) uint.Parse(sc.ReadLine());
                    break;
                }
                catch
                {
                    Console.WriteLine("Вводите натуральное число");
                }
            }

            Console.WriteLine();


            var sessions = new List<FilmSession>();

            Console.WriteLine($"В следующих {sessionsNumber} строках введите дату и время сеансов в аналогичном формате: 05.02.2022 18:35");

            for (int i = 0; i < sessionsNumber; i++)
            {
                while (true)
                {
                    try
                    {
                        var data = sc.ReadLine();
                        DateTime.Parse(data);
                        sessions.Add(new FilmSession(data, hall));
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("ВВедите коректное дата и время сеанса");
                    }
                }
            }

            Console.WriteLine();


            foreach (var session in sessions)
            {
                SettingOneHallsSession(hall, session);
            }

            sessions = sessions
                .OrderBy(session => session.Date)
                .ToList();

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
                    session.Places[i][j] = new Place
                    {
                        Row = i + 1,
                        Column = j + 1,
                        Price = hall.DefaultPrices[i][j],
                        Session = session,
                        IsBought = false,
                        NameOfBuyer = null,
                        BoughtAtPrice = null
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
                    UserInterface.start(films, allFilmSessions, allHalls);
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

        static void PressAnyKey()
        {
            Console.WriteLine("Нажмите любую кнопку для продолжения");

            Console.ReadKey();
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

                    else
                    {
                        break;
                    }
                }
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

            film.Rating = Console.ReadLine();

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

                    film.Rating = Console.ReadLine();

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
                    hall.DefaultPrices = SettingPrices(hall.N, "новые", "сеансов данного зала");

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

            var session = new FilmSession(Console.ReadLine(), hall);

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

            var session = hall.Sessions[int.Parse(Console.ReadLine()) - 1];

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

                    var date = DateTime.Parse(Console.ReadLine());

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
                    var prices = SettingPrices(session.Hall.N, "новые", "данного сеанса");

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

                var place = Console.ReadLine()
                    .Split()
                    .Select(int.Parse)
                    .ToArray();

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

                    session.Places[row - 1][column - 1].Price = int.Parse(Console.ReadLine());

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

            var session = hall.Sessions[int.Parse(Console.ReadLine()) - 1];

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
                var filtersNumbers = answer
                    .Split()
                    .Select(int.Parse)
                    .ToArray();

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

            if (answerFirstLine != "")
            {
                var answerBegining = answerFirstLine
                    .Split(':')
                    .Select(int.Parse)
                    .ToArray();

                var answerEnding = Console.ReadLine()
                    .Split(':')
                    .Select(int.Parse)
                    .ToArray();

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

            var answer = Console.ReadLine();

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

            var number = int.Parse(Console.ReadLine());

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
