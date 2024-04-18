using Piranha.AspNetCore.Services;
using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Extensions
{
    public static class PiranhaApplicationExtensions
    {
        public static RoutedContentBase GetCurrentItem(this IApplicationService app)
        {
            if (app.CurrentPost != null)
            {
                return app.CurrentPost;
            }

            return app.CurrentPage;
        }

        public static Guid? GetCurrentItemId(this IApplicationService app)
        {
            return app.GetCurrentItem()?.Id;
        }
    }
}
