namespace WebApiAuthors.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private int recordsByPage = 10;
        private readonly int maxAmountByPage = 50;

        public int RecordsByPage
        {
            get
            {
                return recordsByPage;
            }
            set
            {
                recordsByPage = (value > maxAmountByPage) ? maxAmountByPage : value;
            }
        }
    }
}
