using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAuthors.Utilities
{
    public class HATEOASFilterAttribute : ResultFilterAttribute
    {
        /// <summary>
        /// Utils Method to know when to include filters
        /// </summary>
        /// <param name="context">Context that execute the filter</param>
        /// <returns></returns>
        protected bool ShouldIncludeHATEOAS(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;

            if(!IsSuccesfullResponse(result))
            {
                return false;
            }

            var header = context.HttpContext.Request.Headers["includeHATEOAS"];

            if(header.Count == 0)
            {
                return false;
            }

            var value = header[0];

            if(!value.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method to validate if the response is from a 200 family
        /// </summary>
        /// <param name="result">Given HTTP response</param>
        /// <returns></returns>
        private bool IsSuccesfullResponse(ObjectResult result)
        {
            if(result == null || result.Value == null)
            {
                return false;
            }

            if(result.StatusCode.HasValue && !result.StatusCode.Value.ToString().StartsWith("2"))
            {
                return false;
            }

            return true;
        }
    }
}
