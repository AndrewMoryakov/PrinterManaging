namespace Project53
{
    public static class LogMessages
    {
        public static string PrinterJobWatchingWasStarted
        {
            get { return "Отслеживание заданий печати было запущено"; }
            private set{}
        }
        
        public static string JobWasCatched
        {
            get { return "Задание поймано"; }
            private set{}
        }
        
        public static string WorkWithPrinters
        {
            get { return "Используются следующие принтеры:"; }
            private set{}
        }
        
        public static string PrinterJobWasCancel
        {
            get { return "Задание было отменено"; }
            private set{}
        }
    }
}