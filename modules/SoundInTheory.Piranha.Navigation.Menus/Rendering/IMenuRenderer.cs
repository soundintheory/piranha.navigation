using Microsoft.AspNetCore.Html;
using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Rendering
{
    public interface IMenuRenderer
    {
        IHtmlContent GetHtml(Menu menu);

        IHtmlContent GetHtml(Menu menu, Action<MenuRenderOptions> configure);

        void Render(Menu menu, Action<MenuRenderOptions> configure = null);

        IHtmlContent List(MenuListViewModel context, Action<MenuRenderOptions> configure = null);

        void RenderList(MenuListViewModel context, Action<MenuRenderOptions> configure = null);

        IHtmlContent Subnav(MenuItemViewModel context, Action<MenuRenderOptions> configure = null);

        void RenderSubnav(MenuItemViewModel context, Action<MenuRenderOptions> configure = null);

        IHtmlContent ListItem(MenuItemViewModel context, Action<MenuRenderOptions> configure = null);

        void RenderListItem(MenuItemViewModel context, Action<MenuRenderOptions> configure = null);

        IHtmlContent Item(MenuItemViewModel context, Action<MenuRenderOptions> configure = null);

        void RenderItem(MenuItemViewModel context, Action<MenuRenderOptions> configure = null);
    }
}
