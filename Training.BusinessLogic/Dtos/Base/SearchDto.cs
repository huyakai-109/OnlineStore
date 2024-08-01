namespace Training.BusinessLogic.Dtos.Base
{
    public class SearchDto
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public string? SortColumn { get; set; }

        public bool Ascending { get; set; } = true;
    }
}
