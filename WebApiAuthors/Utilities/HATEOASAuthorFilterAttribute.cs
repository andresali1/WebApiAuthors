using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAuthors.DTOs;
using WebApiAuthors.Services;

namespace WebApiAuthors.Utilities
{
    public class HATEOASAuthorFilterAttribute : HATEOASFilterAttribute
    {
        private readonly LinksGenerator _linksGenerator;

        public HATEOASAuthorFilterAttribute(LinksGenerator linksGenerator)
        {
            _linksGenerator = linksGenerator;
        }

        public override async Task OnResultExecutionAsync(
            ResultExecutingContext context,
            ResultExecutionDelegate next
        )
        {
            var shouldInclude = ShouldIncludeHATEOAS(context);

            if (!shouldInclude)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;

            var authorDTO = result.Value as AuthorDTO;
            if(authorDTO == null)
            {
                var authorsDTO = result.Value as List<AuthorDTO> ?? throw new ArgumentException("Se esperaba una instancia de AuthorDTO o List<AuthorDTO>");

                authorsDTO.ForEach(async author => await _linksGenerator.GenerateLinks(author));
                result.Value = authorsDTO;
            }
            else
            {
                await _linksGenerator.GenerateLinks(authorDTO);
            }

            await next();
        }
    }
}
