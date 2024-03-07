using Piranha;
using Piranha.Extend;
using Piranha.Manager;
using Piranha.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoundInTheory.Piranha.Navigation
{
    public class LinksModule : IModule
    {
        /// <summary>
        /// Gets the module author
        /// </summary>
        public string Author => "Sound in Theory";

        /// <summary>
        /// Gets the module name
        /// </summary>
        public string Name => "Links";

        /// <summary>
        /// Gets the module version
        /// </summary>
        public string Version => Utils.GetAssemblyVersion(GetType().Assembly);

        /// <summary>
        /// Gets the module description
        /// </summary>
        public string Description => "A field to link to posts, pages or anywhere";

        /// <summary>
        /// Gets the module package url
        /// </summary>
        public string PackageUrl => "";

        /// <summary>
        /// Gets the module icon url
        /// </summary>
        public string IconUrl => "/manager/LinkField/images/logo.svg";

        public void Init()
        {
        }
    }
}
