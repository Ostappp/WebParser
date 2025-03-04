namespace WebParser.Models
{
    class JobInfoModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Phones { get; set; }
        public IEnumerable<string> Emails { get; set; }
        public string Location { get; set; }
    }
}
