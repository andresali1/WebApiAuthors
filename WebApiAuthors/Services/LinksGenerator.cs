using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Services
{
    public class LinksGenerator
    {
        private readonly IAuthorizationService _autorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public LinksGenerator(IAuthorizationService autorizationService,
                              IHttpContextAccessor httpContextAccessor,
                              IActionContextAccessor actionContextAccessor)
        {
            _autorizationService = autorizationService;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
        }

        /// <summary>
        /// Method to know if an user has admin privileges
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsAdmin()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var result = await _autorizationService.AuthorizeAsync(httpContext.User, "isAdmin");
            return result.Succeeded;
        }

        /// <summary>
        /// Support method to get the URL
        /// </summary>
        /// <returns></returns>
        private IUrlHelper BuildURLHelper()
        {
            var factory = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(_actionContextAccessor.ActionContext);
        }

        /// <summary>
        /// Method to generate navigation links for the HATEOAS
        /// </summary>
        /// <param name="authorDTO">Model to handle data</param>
        public async Task GenerateLinks(AuthorDTO authorDTO)
        {
            var isAdmin = await IsAdmin();
            var Url = BuildURLHelper();

            authorDTO.Links.Add(new HATEOASdata(
                link: Url.Link("getAuthor", new { id = authorDTO.Id }),
                description: "self",
                method: "GET")
            );

            if (isAdmin)
            {
                authorDTO.Links.Add(new HATEOASdata(
                    link: Url.Link("updateAuthor", new { id = authorDTO.Id }),
                    description: "author-update",
                    method: "PUT")
                );
                authorDTO.Links.Add(new HATEOASdata(
                    link: Url.Link("deleteAuthor", new { id = authorDTO.Id }),
                    description: "author-delete",
                    method: "DELETE")
                );
            }
        }
    }
}
