using Piranha;
using Piranha.Extend.Fields;

namespace NavigationMvcExample.Models.SelectItems
{
    public class ExampleSelectItem
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public static ExampleSelectItem GetById(string id)
        {
            var item = GetList().FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return null;
            }
            
            return new ExampleSelectItem
            {
                Id = item.Id,
                Title = item.Name
            };
        }

        public static IEnumerable<DataSelectFieldItem> GetList()
        {
            return new List<DataSelectFieldItem>
            {
                new() { Id = "one", Name = "One" },
                new() { Id = "two", Name = "Two" },
                new() { Id = "three", Name = "Three" },
                new() { Id = "four", Name = "Four" },
            };
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
