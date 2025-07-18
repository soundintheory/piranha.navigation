using Piranha.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Models.Manager
{
    public class MenuEditModel : AsyncResult
    {
        public Menu Menu { get; set; }
        public MenuItemTypeModel[] AvailableMenuItemTypes { get; set; }
    }
}
