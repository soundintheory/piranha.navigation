using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Services
{
    public interface ILinkProvider
    {
        /// <summary>
        /// The link type string this provider owns (e.g. "Page", "Post", "Product").
        /// Used by LinkService to route GetByIdAsync calls to the correct provider.
        /// When multiple providers share the same type string, the first registered provider
        /// handles ID resolution.
        /// </summary>
        string LinkType { get; }

        Task<IEnumerable<Link>> GetAllAsync(Guid siteId);

        /// <summary>
        /// Resolves a stored link by its content ID. Returns null if not found or if this
        /// provider delegates resolution to another provider for the same type.
        /// </summary>
        Task<LinkedObject> GetByIdAsync(string id);
    }
}
