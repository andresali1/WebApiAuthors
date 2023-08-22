using WebApiAuthors.DTOs;

namespace WebApiAuthors.Utilities
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Static method to return a certain ammount of records of any type from DB
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="paginationDTO"></param>
        /// <returns></returns>
        public static IQueryable<T> Page<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.Page - 1) * paginationDTO.RecordsByPage)
                .Take(paginationDTO.RecordsByPage);
        }
    }
}
