using System;
using System.Collections.Generic;

namespace SoundInTheory.Piranha.Navigation.Models
{
    public interface ILink
    {
        LinkType Type { get; set; }

        string Url { get; set; }

        string Path { get; }

        string Text { get; set; }

        Guid? Id { get; set; }

        public Dictionary<string, object> Attributes { get; set; }

        string TypeId { get; set; }
    }
}
