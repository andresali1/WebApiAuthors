using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<HATEOASdata>>> Get()
        {
            var hateoasData = new List<HATEOASdata>();

            var isAdmin = await _authorizationService.AuthorizeAsync(User, "isAdmin");

            hateoasData.Add(new HATEOASdata(link: Url.Link("GetRoot", new { }), description: "self", method: "GET"));
            hateoasData.Add(new HATEOASdata(link: Url.Link("getAuthors", new { }), description: "authors", method: "GET"));

            if (isAdmin.Succeeded)
            {
                hateoasData.Add(new HATEOASdata(link: Url.Link("createAuthor", new { }), description: "author-create", method: "POST"));
                hateoasData.Add(new HATEOASdata(link: Url.Link("createBook", new { }), description: "book-create", method: "POST"));
            }

            return hateoasData;
        }
    }
}
