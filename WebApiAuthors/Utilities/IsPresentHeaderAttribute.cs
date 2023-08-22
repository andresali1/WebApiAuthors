using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace WebApiAuthors.Utilities
{
    public class IsPresentHeaderAttribute : Attribute, IActionConstraint
    {
        private readonly string _header;
        private readonly string _value;

        public IsPresentHeaderAttribute(string header, string value)
        {
            _header = header;
            _value = value;
        }

        public int Order => 0;

        /// <summary>
        /// Method to handle headers to use versioning in the controllers
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Accept(ActionConstraintContext context)
        {
            var headers = context.RouteContext.HttpContext.Request.Headers;

            if (!headers.ContainsKey(_header))
            {
                return false;
            }

            return string.Equals(headers[_header], _value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
