using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

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

        public ClickableTextureComponent currentTab;

        public int initialMyID = 1000;

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
                        4f)
                    {
                        myID = initialMyID + i
                    };
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
                    4f)
                {
                    myID = 1999
                };

                currentTab = allTab;

                setupNeighbors();

                populateClickableComponentList();
            }
        }

        public void setupNeighbors()
        {
            for (int i = 0; i < filterTabs.Count; i++)
            {
                filterTabs[i].upNeighborID = -1;
                filterTabs[i].downNeighborID = 3546;
                // if the tab is the first one, set the left neighbor to the all tab
                if (i == 0)
                {
                    filterTabs[i].leftNeighborID = 1999;
                }
                else
                {
                    filterTabs[i].leftNeighborID = filterTabs[i - 1].myID;
                }
                // if the tab is the last one, set the right neighbor to the all tab
                if (i == filterTabs.Count - 1)
                {
                    filterTabs[i].rightNeighborID = 1999;
                }
                else
                {
                    filterTabs[i].rightNeighborID = filterTabs[i + 1].myID;
                }
            }
            // set the all tab neighbors
            allTab.upNeighborID = -1;
            allTab.downNeighborID = 3546;
            allTab.leftNeighborID = filterTabs[filterTabs.Count - 1].myID;
            allTab.rightNeighborID = filterTabs[0].myID;

            targetMenu.forSaleButtons[0].upNeighborID = 1000;
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
            // set currentTab to the tab that is clicked
            currentTab = filterTabs.Find(tab => tab.name == type);
            if (type == "All")
            {
                currentTab = allTab;
            }

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
                // reset the tab position
                tab.bounds.Y = targetMenu.yPositionOnScreen - 60;
                // if tab is selected, draw the tab lower
                if (tab == currentTab)
                {
                    tab.bounds.Y = targetMenu.yPositionOnScreen - 60 + 8;
                }
                tab.draw(b);
            }
            if (allTab != null)
            {
                allTab.bounds.Y = targetMenu.yPositionOnScreen - 60;
                if (allTab == currentTab)
                {
                    allTab.bounds.Y = targetMenu.yPositionOnScreen - 60 + 8;
                }

                allTab.draw(b);
            }

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

            // ensure tabs are not enlarged when not hovered
            foreach (ClickableTextureComponent tab in filterTabs)
            {
                if (tab != hoveredTab)
                {
                    tab.scale = 4f;
                }
            }
            if (allTab != null && allTab != hoveredTab)
            {
                allTab.scale = 4f;
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

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            targetMenu.receiveRightClick(x, y, playSound);
        }

        public override void receiveKeyPress(Keys key)
        {
            targetMenu.receiveKeyPress(key);
        }

        public override void receiveGamePadButton(Buttons button)
        {
            if (button == Buttons.LeftShoulder || button == Buttons.RightShoulder)
            {
                if (filterTabs.Count != 0)
                {
                    processShouldButtons(button);
                    return;
                }
            }
            currentlySnappedComponent = targetMenu.currentlySnappedComponent;
            if (currentlySnappedComponent == null)
            {
                currentlySnappedComponent = targetMenu.forSaleButtons[0];
            }

            if (currentlySnappedComponent.myID == 3546 && (button == Buttons.DPadUp || button == Buttons.LeftThumbstickUp))
            {
                currentlySnappedComponent = currentTab;
            }
            else if (currentlySnappedComponent.myID >= 1000 && currentlySnappedComponent.myID <= 1999)
            {
                if (button == Buttons.DPadDown || button == Buttons.LeftThumbstickDown)
                {
                    currentlySnappedComponent = targetMenu.forSaleButtons[0];
                }
                else if (button == Buttons.DPadLeft || button == Buttons.LeftThumbstickLeft)
                {
                    currentlySnappedComponent = getComponentWithID(currentlySnappedComponent.leftNeighborID);
                }
                else if (button == Buttons.DPadRight || button == Buttons.LeftThumbstickRight)
                {
                    currentlySnappedComponent = getComponentWithID(currentlySnappedComponent.rightNeighborID);
                }
            }
            else
            {
                targetMenu.receiveGamePadButton(button);
                currentlySnappedComponent = targetMenu.currentlySnappedComponent;
            }
            targetMenu.currentlySnappedComponent = currentlySnappedComponent;
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
            currentlySnappedComponent = getComponentWithID(3546);
            snapCursorToCurrentSnappedComponent();
        }

        public void processShouldButtons(Buttons b)
        {
            string currentTabName = currentTab.name;
            if (b == Buttons.LeftShoulder)
            {
                if (currentTabName == "All")
                {
                    currentTabName = availableTabs[availableTabs.Count - 1];
                }
                else if (availableTabs.IndexOf(currentTabName) == 0)
                {
                    currentTabName = "All";
                }
                else
                {
                    // currentTabName = previous tab in availableTabs
                    currentTabName = availableTabs[availableTabs.IndexOf(currentTabName) - 1];
                }
            }
            else if (b == Buttons.RightShoulder)
            {
                if (currentTabName == "All")
                {
                    currentTabName = availableTabs[0];
                }
                else if (availableTabs.IndexOf(currentTabName) == availableTabs.Count - 1)
                {
                    currentTabName = "All";
                }
                else
                {
                    // currentTabName = next tab in availableTabs
                    currentTabName = availableTabs[availableTabs.IndexOf(currentTabName) + 1];
                }

            }
            ApplyTab(currentTabName);
        }
    }
}
