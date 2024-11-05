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
        public static PerScreen<CatagoryMenu> CatagoryMenuList = new PerScreen<CatagoryMenu>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
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
                this.Monitor.Log($"Shop menu opened.", LogLevel.Debug);
                CatagoryMenuList.Value = new CatagoryMenu(menu);
            }
            
        }

    }
}