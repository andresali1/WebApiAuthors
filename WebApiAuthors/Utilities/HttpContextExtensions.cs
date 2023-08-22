using Microsoft.EntityFrameworkCore;

namespace WebApiAuthors.Utilities
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Static method to be used in any endopoint to insert the pagination header
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpContext"></param>
        /// <param name="queryable"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async static Task InsertPaginationParamsInHeaders<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if(httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double amount = await queryable.CountAsync();
            httpContext.Response.Headers.Add("totalAmountData", amount.ToString());
        }
    }
}
