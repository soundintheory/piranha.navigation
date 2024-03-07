using Piranha;
using SoundInTheory.Piranha.Navigation.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Models
{
    public class Menu : MenuInfo
    {
        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();

        /// <summary>
        /// Called after the model has been loaded from the database
        /// </summary>
        public virtual async Task Init(IApi api)
        {
            // Initialise all the items
            if (Items != null)
            {
                await Items.ForEachRecursiveAsync(async i =>
                {
                    if (i.Link != null)
                    {
                        await i.Link.Init(api);
                    }
                });
            }
        }

        public virtual bool IsEmpty => Items == null || Items.Count == 0;
    }
}
