namespace AspIdentity.Models
{
    public class ConfidentialCase
    {
        public string Title { get; set; }
        public string[] UserCanView { get; set; }
        public string CreatedBy { get; set; }
    }
}