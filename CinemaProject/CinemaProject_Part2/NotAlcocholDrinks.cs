using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    class NotAlcocholDrinks : Drinks //неалкогольные напиткм
    {

        public override void Buy()
        {
            Count++;
            int price = volume * this.PricePerLitre;
            return price;
        }



    }
}
