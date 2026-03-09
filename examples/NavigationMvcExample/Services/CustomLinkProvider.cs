using Piranha.Manager.Services;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Services;
using NavigationLinkType = SoundInTheory.Piranha.Navigation.Models.LinkType;

namespace NavigationMvcExample.Services
{
    public class CustomLinkProvider : ILinkProvider
    {
        private readonly PageService _pageService;

        public CustomLinkProvider(PageService pageService)
        {
            _pageService = pageService;
        }

        public string LinkType => "MyCustomLink";

        public Task<IEnumerable<LinkedObject>> GetAllAsync(Guid siteId)
        {
            return Task.FromResult(Enumerable.Range(1, 20)
                .Select(i =>
                {
                    var link = new Link
                    {
                        Id = i.ToString(),
                        Url = $"/my-custom-link-{i}",
                        Text = $"Custom Link {i}",
                        Type = LinkType
                    };

                    return new LinkedObject(link, $"My Custom Link #{i}");
                }));
        }

        public Task<LinkedObject> GetByIdAsync(string id)
        {
            var link = new Link
            {
                Id = id,
                Url = $"/my-custom-link-{id}",
                Text = $"Custom Link {id}",
                Type = LinkType
            };

            return Task.FromResult(new LinkedObject(link, $"My Custom Link #{id}"));
        }
    }
}
