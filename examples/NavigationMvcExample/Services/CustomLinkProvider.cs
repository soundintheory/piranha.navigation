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

        public Task<IEnumerable<Link>> GetAllAsync(Guid siteId)
        {
            return Task.FromResult(Enumerable.Range(1, 20)
                .Select(i => new Link {
                    Id = i.ToString(),
                    Url = $"/my-custom-link-{i}",
                    Text = $"My Custom Link #{i}",
                    Type = LinkType
                }));
        }

        public Task<LinkedObject> GetByIdAsync(string id)
        {
            var link = new Link
            {
                Id = id,
                Url = $"/my-custom-link-{id}",
                Text = $"My Custom Link #{id}",
                Type = LinkType
            };

            return Task.FromResult(new LinkedObject { Link = link });
        }
    }
}
