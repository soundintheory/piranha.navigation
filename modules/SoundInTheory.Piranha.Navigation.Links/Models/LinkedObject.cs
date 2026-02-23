namespace SoundInTheory.Piranha.Navigation.Models
{
    public class LinkedObject
    {
        /// <summary>
        /// The resolved link (URL, text, type, etc.).
        /// </summary>
        public Link Link { get; set; }

        /// <summary>
        /// The underlying content object returned by the provider.
        /// The concrete type depends on the provider (e.g. PageInfo, PostInfo, or a custom type).
        /// Cast to RoutedContentBase where Piranha content is expected.
        /// </summary>
        public object Content { get; set; }
    }
}
