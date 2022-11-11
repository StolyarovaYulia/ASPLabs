namespace Lab3_.ViewModels
{
    public enum SortState
    {
        No,
        RatingAsc,
        RatingDesc,
        CreationAsc,
        CreationDesc
    }

    public class SortViewModel
    {
        public SortViewModel(SortState sortOrder)
        {
            RatingSort = sortOrder == SortState.RatingAsc ? SortState.RatingDesc : SortState.RatingAsc;
            CreationSort = sortOrder == SortState.CreationAsc ? SortState.CreationDesc : SortState.CreationAsc;
            CurrentState = sortOrder;
        }

        public SortState RatingSort { get; set; }
        public SortState CreationSort { get; set; }
        public SortState CurrentState { get; set; }
    }
}