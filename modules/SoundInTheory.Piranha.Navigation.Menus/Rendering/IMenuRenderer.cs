using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Rendering
{
    public interface IMenuRenderer
    {
        IHtmlContent RenderHtml(Models.Menu menu);

        IHtmlContent RenderHtml(Models.Menu menu, Action<MenuRenderOptions> configure);

        void RenderHtml(Models.Menu menu, IHtmlContentBuilder output, Action<MenuRenderOptions> configure);
    }
}
