using System.Linq;
using System.Printing;

namespace Project53
{
    public class Printers
    {
        public static string[] Get()
        {
            using (LocalPrintServer localPrinters = new LocalPrintServer())
            {
                //localPrinters.Refresh();
                var printQ = localPrinters.GetPrintQueues();

                var sortedPrintQ = printQ
                    //.Where(el =>
                    //	el.FullName == "Microsoft Print to PDF")
                    //.Where(
                    //	el =>
                    //	//	el.IsOffline == false &&
                    // el.FullName.Contains("RICOH")
                    //|| (el.FullName.Contains("HP")) //|| el.FullName.Contains("Print") || el.FullName.Contains("HP"))
                    //								  ////&& el.FullName != "HP Deskjet 3520 series"
                    //								  //&& el.FullName != "Fax"

                    //||el.FullName == "Microsoft Print to PDF"
                    //&& el.FullName.Contains("HP")
                    //&& el.FullName.Contains("RICOH")

                    //		&& !el.FullName.Contains("One")
                    //		&& !el.FullName.Contains("XPS")
                    //		&& !el.FullName.Contains("Fax")



                    //&& el.FullName != "Microsoft XPS Document Writer"
                    //&& el.FullName != "Отправить в OneNote 2013"
                    //&& el.FullName != "Отправить в OneNote 20p07"
                    //)
                    .Select(el => el.FullName).ToArray();

                return sortedPrintQ;
            }
        }
    }
}