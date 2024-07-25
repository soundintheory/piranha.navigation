using Microsoft.Extensions.DependencyInjection;
using Piranha.AspNetCore;
using Piranha;
using SoundInTheory.Piranha.Navigation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SoundInTheory.Piranha.Navigation.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Builder;

public static class LinkExtensions
{
    public static IServiceCollection AddLinkServices(this IServiceCollection services)
    {
        services.AddLinkProvider<PageLinkProvider>();
        services.AddLinkProvider<PostLinkProvider>();
        services.TryAddScoped<LinkService>();

        // Return the service collection
        return services;
    }

    public static PiranhaServiceBuilder AddLinkProvider<T>(this PiranhaServiceBuilder serviceBuilder) where T : class, ILinkProvider
    {
        serviceBuilder.Services.AddLinkProvider<T>();

        return serviceBuilder;
    }    

    public static IServiceCollection AddLinkProvider<T>(this IServiceCollection services) where T : class, ILinkProvider
    {
        if (!services.Any(x => x.ServiceType == typeof(ILinkProvider) && x.ImplementationType == typeof(T)))
        {
            services.AddTransient<ILinkProvider, T>();
        }
        return services;
    }

    #region HtmlHelper methods

    public static IHtmlContent Link(this IHtmlHelper html, ILink link) => link.ToHtml(null, null);

    public static IHtmlContent Link(this IHtmlHelper html, ILink link, object attributes) => link.ToHtml(null, attributes);

    public static IHtmlContent Link(this IHtmlHelper html, ILink link, string innerHtml) => link.ToHtml(innerHtml, null);

    public static IHtmlContent Link(this IHtmlHelper html, ILink link, string text, object attributes) => link.ToHtml(text, attributes);

    #endregion

    #region Instance methods returning HTML content

    public static IHtmlContent ToHtml(this ILink link) => link.ToHtml(null, null);

    public static IHtmlContent ToHtml(this ILink link, object attributes) => link.ToHtml(null, attributes);

    public static IHtmlContent ToHtml(this ILink link, string innerHtml) => link.ToHtml(innerHtml, null);

    public static IHtmlContent ToHtml(this ILink link, string innerHtml, object attributes)
    {
        var output = new HtmlContentBuilder();
        link.RenderHtml(output, innerHtml, attributes);
        return output;
    }

    #endregion

    #region Instance methods rendering to IHtmlContentBuilder

    public static void RenderHtml(this ILink link, IHtmlContentBuilder output) => link.RenderHtml(output, null, null);

    public static void RenderHtml(this ILink link, IHtmlContentBuilder output, object attributes) => link.RenderHtml(output, null, attributes);

    public static void RenderHtml(this ILink link, IHtmlContentBuilder output, string innerHtml) => link.RenderHtml(output, innerHtml, null);

    public static void RenderHtml(this ILink link, IHtmlContentBuilder output, string innerHtml, object attributes)
    {
        if (link.IsNullOrEmpty())
        {
            return;
        }

        var tag = link.GetTagBuilder();

        output.AppendHtml(tag);
    }

    public static TagBuilder GetTagBuilder(this ILink link) => link.GetTagBuilder(null, null);

    public static TagBuilder GetTagBuilder(this ILink link, object attributes) => link.GetTagBuilder(null, attributes);

    public static TagBuilder GetTagBuilder(this ILink link, string innerHtml) => link.GetTagBuilder(innerHtml, null);

    public static TagBuilder GetTagBuilder(this ILink link, string innerHtml, object attributes)
    {
        var tag = new TagBuilder("a");
        tag.InnerHtml.AppendHtml(innerHtml ?? link?.Text ?? "");

        if (link != null && !link.IsNullOrEmpty())
        {
            tag.Attributes["href"] = link.Url;
        }

        if (link?.Attributes != null)
        {
            tag.MergeAttributes(link.Attributes, replaceExisting: true);
        }

        if (attributes != null)
        {
            tag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(attributes), replaceExisting: true);
        }

        return tag;
    }

    #endregion

    public static bool IsNullOrEmpty(this ILink link)
    {
        if (link == null)
        {
            return true;
        }

        return string.IsNullOrEmpty(link.Url);
    }
}