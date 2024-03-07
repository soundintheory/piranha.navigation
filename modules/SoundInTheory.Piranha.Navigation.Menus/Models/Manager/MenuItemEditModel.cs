using Piranha.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Models.Manager
{
    public class MenuItemEditModel : AsyncResult
    {
        public MenuItem Item { get; set; }

        public TreeNodePosition? Position { get; set; }
    }
}
