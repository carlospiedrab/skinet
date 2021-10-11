namespace Core.Specifications
{
    public class ProductSpecParams
    {
        private const int MaxPageZize = 50;

        public int PageIndex {get; set;} = 1;

        private int _pageSize = 6;

        public int PageSize{
            get => _pageSize;
            set => _pageSize = (value > MaxPageZize) ? MaxPageZize : value;
        }

        public int? BrandId { get; set; }

        public int? TypeId { get; set; }

        public string? Sort { get; set; }

        private string? _search { get; set; }

        public string? Search 
        {
            get => _search;
            set => _search = value.ToLower();
        }
    }
}