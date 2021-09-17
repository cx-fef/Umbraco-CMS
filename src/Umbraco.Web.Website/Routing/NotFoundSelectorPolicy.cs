using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Routing;

namespace Umbraco.Cms.Web.Website.Routing
{
    /// <summary>
    /// Used to handle 404 routes that haven't been handled by the end user
    /// </summary>
    internal class NotFoundSelectorPolicy : MatcherPolicy, IEndpointSelectorPolicy
    {
        private readonly Lazy<Endpoint> _notFound;
        private readonly EndpointDataSource _endpointDataSource;

        public NotFoundSelectorPolicy(EndpointDataSource endpointDataSource)
        {
            _notFound = new Lazy<Endpoint>(GetNotFoundEndpoint);
            _endpointDataSource = endpointDataSource;            
        }

        // return the endpoint for the RenderController.Index action.
        private Endpoint GetNotFoundEndpoint()
        {
            Endpoint e = _endpointDataSource.Endpoints.First(x =>
            {
                // return the endpoint for the RenderController.Index action.
                ControllerActionDescriptor descriptor = x.Metadata?.GetMetadata<ControllerActionDescriptor>();
                return descriptor?.ControllerTypeInfo == typeof(RenderController)
                    && descriptor?.ActionName == nameof(RenderController.Index);
            });
            return e;
        }

        public override int Order => 0;

        public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
        {
            // Don't apply this filter to any endpoint group that is a controller route
            // i.e. only dynamic routes.            
            foreach(Endpoint endpoint in endpoints)
            {
                ControllerAttribute controller = endpoint.Metadata?.GetMetadata<ControllerAttribute>();
                if (controller != null)
                {
                    return false;
                }
            }

            // then ensure this is only applied if all endpoints are IDynamicEndpointMetadata
            return endpoints.All(x => x.Metadata?.GetMetadata<IDynamicEndpointMetadata>() != null);
        }

        public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
        {
            if (candidates.Count == 1 && candidates[0].Values == null)
            {
                UmbracoRouteValues umbracoRouteValues = httpContext.Features.Get<UmbracoRouteValues>();
                if (umbracoRouteValues?.PublishedRequest != null
                    && !umbracoRouteValues.PublishedRequest.HasPublishedContent()
                    && umbracoRouteValues.PublishedRequest.ResponseStatusCode == StatusCodes.Status404NotFound)
                {
                    // not found/404
                    httpContext.SetEndpoint(_notFound.Value);
                }
            }
            
            return Task.CompletedTask;            
        }
    }
}
