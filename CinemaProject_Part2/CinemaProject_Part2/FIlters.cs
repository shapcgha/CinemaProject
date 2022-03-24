using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    static class FIlters
    {
        static void FilteringByCurrentDate(List<Func<FilmSession, bool>> filters)
        {
            filters.Add(ses => ses.Date > DateTime.Now);
        }

        static void FilteringByAllBoughtPlaces(List<Func<FilmSession, bool>> filters)
        {
            filters.Add(ses => ses.Places.Any(row => row.Any(pl => !pl.IsBought)));
        }


    }
}
