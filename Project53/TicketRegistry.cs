using System;
using System.Collections.Generic;
using System.Linq;

namespace Project53
{
    public static class TicketRegistry
    {
        private static List<string> _usedTickets;
        private static List<string> _freeTickets;

        static TicketRegistry()
        {
            _freeTickets = new List<string>(10);
            _usedTickets = new List<string>(10);
            
            CreateNewTicketsIntoCache();
        }

        private static void CreateNewTicketsIntoCache()
        {
            for (int j = 0; j < 9; j++)
                _freeTickets.Add(Guid.NewGuid().ToString());
        }

        public static string Get()
        {
            var firstFreeTicket = "";
            if (!_freeTickets.Any())
            {
                firstFreeTicket = _freeTickets[0];
                _freeTickets.RemoveAt(0);
            }
            else
            {
                CreateNewTicketsIntoCache();
            }

            return firstFreeTicket;
        }

        public static bool Contains(string ticket)
        {
            return _usedTickets.Contains(ticket);
        }
        
        public static bool ValidateAndRemove(string ticket)
        {
            bool isContain = _usedTickets.Contains(ticket);

            if (isContain)
                _usedTickets.Remove(ticket);

            return isContain;
        }
    }
}