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

        public static IModContentHelper Content;

        public static ITranslationHelper Translation;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.WindowResized += this.OnWindowSizeChanged;
            Content = base.Helper.ModContent;
            Translation = base.Helper.Translation;
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
                if (menu.ShopId != null)
                {
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
                            //display all known properties of the item
                            foreach (var prop in item.GetType().GetProperties())
                            {
                                this.Monitor.Log(prop.Name + ": " + prop.GetValue(item), LogLevel.Warn);
                            }
                        }
                    }
                    TabMenuList.Value = new TabMenu(menu, itemStock);
                }
            }

        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            //if (this.Helper.Input.IsDown(SButton.H))
            //{
            //    Utility.TryOpenShopMenu("SeedShop", "P");
            //}
        }

        private void OnWindowSizeChanged(object? sender, WindowResizedEventArgs e)
        {
            if (TabMenuList.Value != null)
            {
                TabMenuList.Value.gameWindowSizeChanged(new Rectangle(Point.Zero, e.OldSize), new Rectangle(Point.Zero, e.NewSize));
            }
        }
    }
}