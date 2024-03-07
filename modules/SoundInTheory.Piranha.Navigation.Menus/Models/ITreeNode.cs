using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Models
{
    /// <summary>
    /// An interface for objects that are part of a basic tree structure. When you have a list of these,
    /// there are a bunch of extension methods available that can build and navigate the tree structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITreeNode<T> where T : ITreeNode<T>
    {
        Guid Id { get; set; }

        IList<T> Children { get; set; }

        T Parent { get; set; }
    }
}
