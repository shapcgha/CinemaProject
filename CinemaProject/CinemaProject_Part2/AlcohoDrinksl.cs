using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProject_Part2
{
    class BottledDrinks : Drinks
    {
        public int VolumePerBottle { get; set; }

        public int  TotalAmount { get; set; }

        public int PricePerBottle { get; set; }

        public override int PriceCount()
        {
            int amount = 1;
            int price = amount * this.PricePerBottle;
            return price;
            
        }
    }
}
