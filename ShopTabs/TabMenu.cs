using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopTabs
{
    internal class TabMenu : IClickableMenu
    {
        public static List<string> catagories = new List<string>
        {
            "Forage", // type = Basic, catagory == -81
            "Seeds", // type == Seeds, category == -74
            "Saplings", // type == Basic, catagory == -74
            "Cooking", // type == Cooking, catagory == -7
            "Animal Product", // type == Basic, catagory == -6 for milk, -5 for eggs
            "Fish", // type == fish, catagory == -4
            "Artisan", // type == Basic, catagory == -26 for usual artisan, -27 for tree tap products
            "Fertillizer", // type == Basic, catagory == -19
            "Crops", // type == Basic, catagory == -75
            "Recipes",
            "Other"
        };
        public TabMenu() { }
    }
}
