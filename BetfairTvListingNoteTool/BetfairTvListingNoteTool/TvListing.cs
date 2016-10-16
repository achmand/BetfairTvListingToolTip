namespace BetfairTvListingNoteTool
{
    public sealed class TvListing
    {
        public string TimeStart { get; set; }
        public string Sport { get; set; }
        public string Event { get; set; }
        public string Broadcast { get; set; }

        public override string ToString()
        {
            return ($"\n{TimeStart} \n{Sport} \n{Event} \n{Broadcast}");
        }
    }
}
