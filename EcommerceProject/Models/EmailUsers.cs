namespace EcommerceProject.Models
{
    public class EmailUsers
    {
        public List<String> ToEmails { get; set; }

        public string Subject {  get; set; }

        public string Body {  get; set; }

        public List<KeyValuePair<string,string>> PlaceHolders { get; set;}


    }
}
