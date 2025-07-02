using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MenuRenderableAttribute : Attribute
    {

        public string Name { get; set; }

        public string Shorthand { get; set; }
    }
}
