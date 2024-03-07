using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.Manager;
using Piranha.Manager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Mime;
using System.Text;
using SoundInTheory.Piranha.Navigation.Services;
using SoundInTheory.Piranha.Navigation.Models;

namespace SoundInTheory.Piranha.Navigation.Controllers
{
    [Area("Manager")]
    [Route("manager/api/links")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    [AutoValidateAntiforgeryToken]
    public class LinkApiController : Controller
    {
        private readonly LinkService _linkService;

        public LinkApiController(LinkService linkService)
        {
            _linkService = linkService;
        }

        [Route("all"), HttpGet]
        public async Task<IActionResult> GetAllLinks([FromQuery] LinkListOptions options) 
        {
            var links = await _linkService.GetLinksAsync(options);

            return Content(
                content: JsonConvert.SerializeObject(links, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                contentType: MediaTypeNames.Application.Json,
                contentEncoding: Encoding.UTF8
            );
        }
    }
}
