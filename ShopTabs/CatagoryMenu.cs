using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopTabs
{
    internal class CatagoryMenu : IClickableMenu
    {
        public ClickableTextureComponent testTab;

        public CatagoryMenu(ShopMenu menu)
        {
            menu.SetChildMenu(this);
            Dictionary<ISalable, ItemStockInformation> itemDict = menu.itemPriceAndStock;
            Console.WriteLine(menu.xPositionOnScreen + ", "+  menu.yPositionOnScreen);
            testTab = new ClickableTextureComponent("Recipes", 
                new Rectangle(menu.xPositionOnScreen, menu.yPositionOnScreen - 60, 64, 64), 
                "", 
                "Recipes", 
                Game1.mouseCursors, 
                new Rectangle(16, 368, 16, 16), 
                4f);
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            testTab.draw(b);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            testTab.tryHover(x, y);
        }
    }
}
