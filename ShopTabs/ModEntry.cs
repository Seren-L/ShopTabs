using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Content;

namespace ShopTabs
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        public static PerScreen<TabMenu> TabMenuList = new();
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/

        /// <summary>
        /// Logs when a menu change.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (Game1.activeClickableMenu != null && e.NewMenu is ShopMenu menu)
            {
                this.Monitor.Log($"Shop menu {menu.ShopId} opened.", LogLevel.Debug);

                if (menu.ShopId != null && menu.ShopId == "SeedShop" || menu.ShopId == "Joja" || menu.ShopId == "Traveler")
                {
                    this.Monitor.Log($"Detected SeedShop or Joja.", LogLevel.Debug);
                    Dictionary<ISalable, ItemStockInformation> itemStock = menu.itemPriceAndStock;
                    foreach (var item in itemStock.Keys)
                    {
                        if (item is StardewValley.Object obj)
                        {
                            Console.WriteLine(obj.Category + obj.Type + obj.DisplayName);
                        }
                        else
                        {
                            this.Monitor.Log("Encountered a non-object item in item stock.", LogLevel.Warn);
                        }
                    }
                    TabMenuList.Value = new TabMenu(menu, itemStock);
                }
            }

        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (this.Helper.Input.IsDown(SButton.H))
            {
                this.Monitor.Log($"Key H pressed.", LogLevel.Debug);
                Utility.TryOpenShopMenu("Traveler", "P");
            }
        }

    }
}