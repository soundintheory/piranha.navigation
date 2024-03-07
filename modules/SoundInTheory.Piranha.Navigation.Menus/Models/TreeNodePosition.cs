using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Models
{
    public struct TreeNodePosition
    {
        public Guid? ParentId { get; set; }

        public Guid? AfterId { get; set; }

        public static TreeNodePosition After(Guid? id) => new() { AfterId = id };

        public static TreeNodePosition ChildOf(Guid? id) => new() { ParentId = id };
    }
}
