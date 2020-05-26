using System;

namespace DataModels
{
    public class PrintedDocument
    {
        public PrintedDocument()
        {
        }
        
        public PrintedDocument(string docName, int amountOfPages)
        {
            Document = docName;
            AmountOfPages = amountOfPages;
        }

        public string Document { get; set; }
        public int AmountOfPages { get; set; }
    }
}