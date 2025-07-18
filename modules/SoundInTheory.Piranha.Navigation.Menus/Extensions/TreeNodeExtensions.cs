using SoundInTheory.Piranha.Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Extensions
{
    public static class TreeNodeExtensions
    {
        /// <summary>
        /// Gets an item's children of a certain type
        /// </summary>
        public static IList<TChild> GetChildren<T, TChild>(this T item) where T : ITreeNode<T>
        {
            return item.Children.OfType<TChild>().ToList();
        }

        /// <summary>
        /// Gets the breadcrumbs for a given item in the tree
        /// </summary>
        public static IList<T> GetBreadcrumbs<T>(this T item, bool includeSelf = false) where T : ITreeNode<T>
        {
            var output = new List<T>();
            var parent = item.Parent;

            if (includeSelf)
            {
                output.Add(item);
            }

            while (parent != null)
            {
                output.Add(parent);
                parent = parent.Parent;
            }

            output.Reverse();

            return output;
        }

        public static void InsertNode<T>(this IList<T> tree, T item, TreeNodePosition position) where T : ITreeNode<T>
        {
            // Remove any instances of item first
            tree.RemoveRecursive(item);

            var afterItem = position.AfterId.HasValue ? tree.FindRecursive(x => x.Id == position.AfterId.Value) : default;
            var parent = position.ParentId.HasValue ? tree.FindRecursive(x => x.Id == position.ParentId.Value) : default;

            // Fall back to the parent of the item we're inserting after
            if (parent == null && afterItem != null)
            {
                parent = afterItem.Parent;
            }

            if (parent != null && parent.Children == null)
            {
                parent.Children = new List<T>();
            }

            var siblings = parent?.Children ?? tree;

            if (afterItem != null && siblings.Contains(afterItem))
            {
                var index = siblings.IndexOf(afterItem) + 1;
                siblings.Insert(index, item);
            }
            else
            {
                siblings.Add(item);
            }
        }

        public static bool RemoveRecursive<T>(this IList<T> tree, T item) where T : ITreeNode<T>
        {
            var output = tree.Remove(item);

            foreach (var node in tree)
            {
                if (node?.Children?.Count > 0 && node.Children.RemoveRecursive(item))
                {
                    output = true;
                }
            }

            return output;
        }

        public static T FindRecursive<T>(this IList<T> tree, Func<T, bool> predicate) where T : ITreeNode<T>
            => tree.WhereRecursive(predicate).FirstOrDefault();

        /// <summary>
        /// Filters a tree recursively based on a predicate
        /// </summary>
        public static IEnumerable<T> WhereRecursive<T>(this IList<T> tree, Func<T, bool> predicate) where T : ITreeNode<T>
        {
            var output = tree.Where(predicate);

            foreach (var item in tree)
            {
                if (item?.Children?.Count > 0)
                {
                    output = output.Concat(item.Children.WhereRecursive(predicate));
                }
            }

            return output;
        }

        /// <summary>
        /// Execute an action on each item in the tree recursively
        /// </summary>
        public static void ForEachRecursive<T>(this IList<T> list, Action<T> action) where T : ITreeNode<T>
        {
            foreach (var item in list)
            {
                action(item);

                if (item?.Children?.Count > 0)
                {
                    item.Children.ForEachRecursive(action);
                }
            }
        }

        public static void ForEachRecursive<T>(this IList<T> list, Action<T,int> action) where T : ITreeNode<T>
        {
            ForEachRecursiveInternal(list, action, 1);
        }

        private static void ForEachRecursiveInternal<T>(IList<T> list, Action<T, int> action, int level) where T : ITreeNode<T>
        {
            foreach (var item in list)
            {
                action(item, level);

                if (item?.Children?.Count > 0)
                {
                    ForEachRecursiveInternal(item.Children, action, level + 1);
                }
            }
        }

        /// <summary>
        /// Execute an action on each item in the tree recursively
        /// </summary>
        public static async Task ForEachRecursiveAsync<T>(this IList<T> list, Func<T, Task> action) where T : ITreeNode<T>
        {
            foreach (var item in list)
            {
                await action(item);

                if (item?.Children?.Count > 0)
                {
                    await item.Children.ForEachRecursiveAsync(action);
                }
            }
        }

        public static Dictionary<TKey, TValue> ToDictionaryRecursive<TKey, TValue, T>(this IList<T> tree, Func<T, TKey> keySelector, Func<T, TValue> elementSelector = null) where T : ITreeNode<T>
        {
            var dict = new Dictionary<TKey, TValue>();

            tree.ForEachRecursive(node =>
            {
                var key = keySelector(node);
                var element = elementSelector(node);
                dict[key] = element;
            });

            return dict;
        }

        public static List<T> MatchStructure<T>(this IList<T> tree, IList<T> structure, out List<T> removedNodes) where T : ITreeNode<T>
        {
            var newTree = new List<T>();
            var allNodes = tree.ToDictionaryRecursive(x => x.Id, x => x);

            foreach (var item in structure)
            {
                if (allNodes.Remove(item.Id, out var node))
                {
                    newTree.Add(node);
                    node.Children = node.Children.MatchStructureInternal(item.Children, ref allNodes);
                }
            }

            removedNodes = allNodes.Values.ToList();

            return newTree;
        }

        private static IList<T> MatchStructureInternal<T>(this IList<T> tree, IList<T> structure, ref Dictionary<Guid, T> allNodes) where T : ITreeNode<T>
        {
            if (structure == null || structure.Count == 0)
            {
                return new List<T>();
            }

            var newTree = new List<T>();

            foreach (var item in structure)
            {
                if (allNodes.Remove(item.Id, out var node))
                {
                    newTree.Add(node);
                    node.Children = node.Children.MatchStructureInternal(item.Children, ref allNodes);
                }
            }

            return newTree;
        }
    }
}
