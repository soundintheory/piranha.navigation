using Newtonsoft.Json;
using static Piranha.Manager.Models.PageListModel;

namespace SoundInTheory.Piranha.Navigation.Models
{
    public class LinkedObject
    {
        public LinkedObject()
        {
        }

        public LinkedObject(Link link, string text = null)
        {
            Link = link;
            Text = text;
        }

        /// <summary>
        /// The resolved link (URL, text, type, etc.).
        /// </summary>
        public Link Link { get; set; }

        /// <summary>
        /// The text used to display this linked object when searching and filtering in the manager.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        /// <summary>
        /// The underlying content object returned by the provider.
        /// The concrete type depends on the provider (e.g. PageInfo, PostInfo, or a custom type).
        /// Cast to RoutedContentBase where Piranha content is expected.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Content { get; set; }

        /// <summary>
        /// Implicit operator for converting a Link object to a LinkedObject
        /// </summary>
        /// <param name="link">The link object</param>
        public static implicit operator LinkedObject(Link link)
        {
            if (link == null)
            {
                return null;
            }

            return new LinkedObject(link);
        }

        /// <summary>
        /// Implicit operator for converting a Link object to a LinkedObject
        /// </summary>
        /// <param name="linkedObject">The linked object</param>
        public static implicit operator Link(LinkedObject linkedObject)
        {
            if (linkedObject == null)
            {
                return null;
            }

            return linkedObject.Link;
        }
    }
}
