namespace Training.BusinessLogic.Dtos.Base
{
    public class SearchDto
    {
        public int Skip { get; set; } = 0;

        public int Take { get; set; } = 20;

        public string? SortColumn { get; set; }

        public bool Ascending { get; set; } = true;
    }
}
