namespace Project53.New_arhtech
{
    public class JobMeta
    {
        public string DocumentName { get; private set; }
        public int CountOfPages { get; private set; }
        public int Copies { get; private set; }
        
        public JobMeta(string documentName, int countOfPages, int copies)
        {
            DocumentName = documentName;
            CountOfPages = countOfPages;
            Copies = copies;
        }
    }
}