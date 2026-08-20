using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.AspNetCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Services
{
    public class ApplicationServiceAccessor
    {
        /// <summary>
        /// The default base url that is used for links outside of an http context
        /// </summary>
        public static string DefaultBaseUrl { get; internal set; }

        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IApi _api;

        private IApplicationService _defaultAppService;


        public ApplicationServiceAccessor(IHttpContextAccessor contextAccessor, IApi api)
        {
            _contextAccessor = contextAccessor;
            _api = api;
        }

        /// <summary>
        /// Tries to get the Piranha IApplicationService from the http context services, otherwise falls back to a default instance
        /// with some basic information populated
        /// </summary>
        public IApplicationService ApplicationService
        {
            get
            {
                return DefaultAppService;

                var httpContext = _contextAccessor?.HttpContext;

                if (httpContext != null)
                {
                    return httpContext.RequestServices.GetService<IApplicationService>() ?? DefaultAppService;
                }

                return DefaultAppService;
            }
        }

        private IApplicationService DefaultAppService
        {
            get
            {
                if (_defaultAppService == null)
                {
                    _defaultAppService = new ApplicationService(_api);

                    if (!string.IsNullOrWhiteSpace(DefaultBaseUrl) && Uri.TryCreate(DefaultBaseUrl, UriKind.Absolute, out var uri))
                    {
                        _defaultAppService.Request.Scheme = uri.Scheme;
                        _defaultAppService.Request.Port = uri.Port;
                        _defaultAppService.Request.Host = uri.Host;
                    }
                }
                return _defaultAppService;
            }
        }
    }
}
