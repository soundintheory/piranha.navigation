using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

public static  class MenuItemHtmlHelperExtensions
{
    public static IHtmlContent MenuItem(this IHtmlHelper<MenuViewModel> html, MenuItem item, Action<MenuRenderOptions> configure = null)
    {
        return html.ViewData.Model.Renderer.Item(new MenuItemViewModel(item, html.ViewData.Model), configure);
    }

    public static IHtmlContent MenuListItem(this IHtmlHelper<MenuViewModel> html, MenuItem item, Action<MenuRenderOptions> configure = null)
    {
        return html.ViewData.Model.Renderer.ListItem(new MenuItemViewModel(item, html.ViewData.Model), configure);
    }

    public static IHtmlContent MenuList(this IHtmlHelper<MenuViewModel> html, Menu menu, Action<MenuRenderOptions> configure = null)
    {
        return html.ViewData.Model.Renderer.List(new MenuListViewModel(html.ViewData.Model) { Items = menu.Items }, configure);
    }

    public static IHtmlContent MenuList(this IHtmlHelper<MenuViewModel> html, MenuItem item, Action<MenuRenderOptions> configure = null)
    {
        return html.ViewData.Model.Renderer.List(new MenuListViewModel(html.ViewData.Model) { Items = item.Children, Parent = item }, configure);
    }

    public static IHtmlContent MenuList(this IHtmlHelper<MenuItemViewModel> html, List<MenuItem> items, MenuItem parent, Action<MenuRenderOptions> configure = null)
    {
        return html.ViewData.Model.Renderer.List(new MenuListViewModel(html.ViewData.Model) { Items = items, Parent = parent }, configure);
    }

    public static IHtmlContent MenuSubnav(this IHtmlHelper<MenuViewModel> html, MenuItem item, Action<MenuRenderOptions> configure = null)
    {
        return html.ViewData.Model.Renderer.Subnav(new MenuItemViewModel(item, html.ViewData.Model), configure);
    }

    public static IHtmlContent MenuItem<TItem>(this IHtmlHelper<MenuItemViewModel<TItem>> html, MenuItem item, Action<MenuRenderOptions> configure = null) where TItem : MenuItem
    {
        return html.ViewData.Model.Renderer.Item(new MenuItemViewModel(item, html.ViewData.Model), configure);
    }

    public static IHtmlContent MenuListItem<TItem>(this IHtmlHelper<MenuItemViewModel<TItem>> html, MenuItem item, Action<MenuRenderOptions> configure = null) where TItem : MenuItem
    {
        return html.ViewData.Model.Renderer.ListItem(new MenuItemViewModel(item, html.ViewData.Model), configure);
    }

    public static IHtmlContent MenuList<TItem>(this IHtmlHelper<MenuItemViewModel<TItem>> html, MenuItem item, Action<MenuRenderOptions> configure = null) where TItem : MenuItem
    {
        return html.ViewData.Model.Renderer.List(new MenuListViewModel(html.ViewData.Model) { Items = item.Children, Parent = item }, configure);
    }

    public static IHtmlContent MenuList<TItem>(this IHtmlHelper<MenuItemViewModel<TItem>> html, List<MenuItem> items, MenuItem parent, Action<MenuRenderOptions> configure = null) where TItem : MenuItem
    {
        return html.ViewData.Model.Renderer.List(new MenuListViewModel(html.ViewData.Model) { Items = items, Parent = parent }, configure);
    }

    public static IHtmlContent MenuSubnav<TItem>(this IHtmlHelper<MenuItemViewModel<TItem>> html, MenuItem item, Action<MenuRenderOptions> configure = null) where TItem : MenuItem
    {
        return html.ViewData.Model.Renderer.Subnav(new MenuItemViewModel(item, html.ViewData.Model), configure);
    }
}