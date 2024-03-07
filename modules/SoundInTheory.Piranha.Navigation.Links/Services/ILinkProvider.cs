using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Services
{
    public interface ILinkProvider
    {
        Task<IEnumerable<Link>> GetAllAsync(Guid siteId);
    }
}
