using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace WebApiAuthors.DTOs
{
    public class HashResult
    {
        public string Hash { get; set; }
        public byte[] Salt { get; set; }
    }
}
