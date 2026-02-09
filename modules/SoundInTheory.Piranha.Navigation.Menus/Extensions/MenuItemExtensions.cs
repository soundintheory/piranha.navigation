using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Piranha;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Extensions
{
    public static class MenuItemExtensions
    {
        public static async Task InitFields(this List<MenuItem> items, IServiceProvider services, bool managerInit = false)
        {
            using var scope = services.CreateScope();

            if (items == null)
            {
                return;
            }

            await items.ForEachRecursiveAsync(async (item, level) =>
            {
                item.Level = level;

                var itemDefinition = MenuModule.Instance.MenuItems.GetByType(item.GetType());
                var itemType = item.GetType();

                if (itemDefinition != null)
                {
                    // Initialize all fields
                    foreach (var fieldType in itemDefinition.Fields)
                    {
                        var field = itemType.GetPropertyValue(fieldType.Id, item);

                        if (field != null)
                        {
                            await InitFieldAsync(scope, field, managerInit).ConfigureAwait(false);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Initializes the given field.
        /// </summary>
        /// <param name="scope">The current service scope</param>
        /// <param name="field">The field</param>
        /// <param name="managerInit">If this is initialization used by the manager</param>
        /// <returns>The initialized field</returns>
        private static async Task InitFieldAsync(IServiceScope scope, object field, bool managerInit)
        {
            var appField = App.Fields.GetByType(field.GetType());

            if (appField != null)
            {
                await appField.Init.InvokeAsync(field, scope, managerInit);
            }
        }
    }
}
