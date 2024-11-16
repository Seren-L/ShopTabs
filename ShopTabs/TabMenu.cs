using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Content;

namespace ShopTabs
{
    internal class TabMenu : IClickableMenu
    {
        public ShopMenu targetMenu;

        public Dictionary<ISalable, ItemStockInformation> shopItems;

        public List<ClickableTextureComponent> filterTabs = new();

        public ClickableTextureComponent allTab;

        private ClickableTextureComponent? hoveredTab;

        private List<string> availableTabs = new();

        // Define filter types and their conditions in a dictionary
        public static readonly Dictionary<string, Func<StardewValley.Object, bool>> FilterConditions = new()
{
    { "Seeds", obj => (obj.Type == "Seeds" || obj.Type == "Seed") && obj.Category == -74 },
    { "Saplings", obj => obj.Type == "Basic" && obj.Category == -74 },
    { "Cooking", obj => obj.Type == "Cooking" && obj.Category == -7 && !obj.IsRecipe },
    { "Fertillizer", obj => obj.Type == "Basic" && obj.Category == -19 },
    { "Crops", obj => (obj.Type == "Basic" || obj.Type == "Vegetable") && obj.Category == -75 },
    { "Recipes", obj => obj.IsRecipe },
    { "Animal Product", obj => obj.Type == "Basic" && (obj.Category == -6 || obj.Category == -5 || obj.Category == -18) },
    { "Fish" , obj => obj.Type == "Fish" && obj.Category == -4 },
    { "Artisan", obj => (obj.Type == "Basic" || obj.Type == "ArtisanGoods") && (obj.Category == -26 || obj.Category == -27) },
    { "Flowers", obj => obj.Type == "Basic" && obj.Category == -80 },
    { "Minerals", obj => obj.Type == "Minerals" && (obj.Category == -15 || obj.Category == -12 || obj.Category == -2)},
    { "Crafting", obj => obj.Type == "Crafting" && !obj.IsRecipe},
    { "Monster Drops", obj => obj.Type == "Basic" && obj.Category == -28},
    { "Bait", obj => obj.Type == "Basic" && obj.Category == -21},
    { "Artifacts", obj => obj.Type == "Arch"},
    { "Other", obj => !FilterConditions.Where(kvp => kvp.Key != "Other").Any(kvp => kvp.Value(obj)) }
};

        public TabMenu(ShopMenu menu, Dictionary<ISalable, ItemStockInformation> shopItems)
        {
            this.targetMenu = menu;
            menu.SetChildMenu(this);
            this.shopItems = shopItems;
            GetAvaliableTabs(shopItems);

            if (shopItems != null && availableTabs.Count > 1)
            {
                int i = 0;
                foreach (string filterType in availableTabs)
                {
                    string assetName = "assets/" + filterType;

                    // ensure the asset exists
                    Texture2D asset;
                    try
                    {
                        asset = ModEntry.Content.Load<Texture2D>(assetName);
                    }
                    catch (ContentLoadException)
                    {
                        asset = ModEntry.Content.Load<Texture2D>("assets/TestTab");
                    }

                    ClickableTextureComponent tab = new(
                        filterType,
                        new Rectangle(menu.xPositionOnScreen + 64 * i, menu.yPositionOnScreen - 60, 64, 64),
                        "",
                        ModEntry.Translation.Get(filterType),
                        asset,
                        new Rectangle(0, 0, 16, 16),
                        4f);
                    filterTabs.Add(tab);
                    i++;
                }

                // add the allTab at the right of the menu
                allTab = new ClickableTextureComponent(
                    "All",
                    new Rectangle(menu.xPositionOnScreen + targetMenu.width - 128, menu.yPositionOnScreen - 60, 64, 64),
                    "",
                    ModEntry.Translation.Get("All"),
                    ModEntry.Content.Load<Texture2D>("assets/All"),
                    new Rectangle(0, 0, 16, 16),
                    4f);
            }
        }

        public void GetAvaliableTabs(Dictionary<ISalable, ItemStockInformation> shopItems)
        {
            List<ISalable> items = shopItems.Keys.ToList();
            foreach (ISalable item in items)
            {
                if (item is StardewValley.Object obj)
                {
                    foreach (string filterType in FilterConditions.Keys)
                    {
                        if (FilterConditions[filterType](obj) && !availableTabs.Contains(filterType))
                        {
                            availableTabs.Add(filterType);
                        }
                    }

                }
            }
            // ensure the last tab is always "Other"
            if (!availableTabs.Contains("Other"))
            {
                availableTabs.Add("Other");
            }
            else
            {
                availableTabs.Remove("Other");
                availableTabs.Add("Other");
            }

        }

        public void ApplyTab(string type)
        {
            Dictionary<ISalable, ItemStockInformation> newStock = new();

            // if the tab is "All", display all items
            if (type == "All")
            {
                targetMenu.itemPriceAndStock = shopItems;
                targetMenu.forSale = shopItems.Keys.ToList();
                return;
            }

            // Use the condition from the dictionary
            if (FilterConditions.TryGetValue(type, out Func<StardewValley.Object, bool>? condition))
            {
                foreach (var item in this.shopItems)
                {
                    if (item.Key is StardewValley.Object obj)
                    {
                        if (condition(obj))
                            newStock[item.Key] = item.Value;
                    }
                    else
                    {
                        if (type == "Other")
                            newStock[item.Key] = item.Value;
                    }

                }
            }

            targetMenu.itemPriceAndStock = newStock;
            targetMenu.forSale = newStock.Keys.ToList();
        }

        public override void draw(SpriteBatch b)
        {
            targetMenu.draw(b);
            foreach (ClickableTextureComponent tab in filterTabs)
            {
                tab.draw(b);
            }
            if (allTab != null)
                allTab.draw(b);

            if (hoveredTab != null)
            {
                IClickableMenu.drawHoverText(b, hoveredTab.hoverText, Game1.smallFont);
            }

            drawMouse(b);
        }

        public override void performHoverAction(int x, int y)
        {
            targetMenu.performHoverAction(x, y);
            hoveredTab = null;
            foreach (ClickableTextureComponent tab in filterTabs)
            {
                if (tab.containsPoint(x, y))
                {
                    tab.tryHover(x, y);
                    hoveredTab = tab;
                }
            }
            if (allTab != null && allTab.containsPoint(x, y))
            {
                allTab.tryHover(x, y);
                hoveredTab = allTab;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (ClickableTextureComponent tab in filterTabs)
            {
                if (tab.containsPoint(x, y))
                {
                    ApplyTab(tab.name);
                }
            }
            if (allTab != null && allTab.containsPoint(x, y))
            {
                ApplyTab("All");
            }
            targetMenu.receiveLeftClick(x, y, playSound);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            targetMenu.receiveScrollWheelAction(direction);
        }

        public override void update(GameTime time)
        {
            targetMenu.update(time);
        }

        public override bool readyToClose()
        {
            // Close both TabMenu and ShopMenu when targetMenu is ready to close
            if (targetMenu.readyToClose())
            {
                Game1.activeClickableMenu = null;
                return true;
            }
            return false;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            targetMenu.gameWindowSizeChanged(oldBounds, newBounds);

            // Update the position of the tabs
            int i = 0;
            foreach (ClickableTextureComponent tab in filterTabs)
            {
                tab.bounds = new Rectangle(targetMenu.xPositionOnScreen + 64 * i, targetMenu.yPositionOnScreen - 60, 64, 64);
                i++;
            }
            if (allTab != null)
                allTab.bounds = new Rectangle(targetMenu.xPositionOnScreen + targetMenu.width - 128, targetMenu.yPositionOnScreen - 60, 64, 64);
        }

        public override void snapToDefaultClickableComponent()
        {
            targetMenu.snapToDefaultClickableComponent();
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            targetMenu.snapCursorToCurrentSnappedComponent();
        }
    }
}
