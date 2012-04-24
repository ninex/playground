using SignalR.Hubs;

namespace SignalRTest.Models
{
    public class TicketHub : Hub
    {
        int TotalTickets = 11;

        public void GetTicketCount()
        {
            Clients.updateTicketCount(TotalTickets);
        }
        public void BuyTicket()
        {
            if (TotalTickets > 0)
                TotalTickets -= 1;
            Clients.updateTicketCount(TotalTickets);
        }
    }
}