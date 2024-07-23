namespace Training.Api.Models.Responses.Base
{
    public class PaginationRes
    {
        public PaginationRes(int totalCount, int currentCount, int? skip, int? take)
        {
            TotalCount = totalCount;
            CurrentCount = currentCount;
            ItemsPerPage = take ?? totalCount;

            if (ItemsPerPage == 0) return;

            var remain = totalCount % ItemsPerPage != 0 ? 1 : 0;
            TotalPages = totalCount / ItemsPerPage + remain;
            CurrentPage = (skip ?? 0) / ItemsPerPage + 1;
        }

        public int TotalCount { get; set; }

        public int CurrentPage { get; set; }

        public int ItemsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int CurrentCount { get; set; }
    }
}
