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

                if (menu.ShopId != null && menu.ShopId == "SeedShop" || menu.ShopId != "Joja" || menu.ShopId == "Traveller")
                {
                    Dictionary<ISalable, ItemStockInformation> itemStock = menu.itemPriceAndStock;
                    foreach (Item i in itemStock.Keys.Cast<Item>()) {
                        Console.WriteLine(i.Category + i.QualifiedItemId + i.DisplayName);
                    }
                }
            }

        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            this.Monitor.Log($"Key H pressed.", LogLevel.Debug);
            if (this.Helper.Input.IsDown(SButton.H))
            {
                Utility.TryOpenShopMenu("SeedShop", "P");
            }
        }

    }
}