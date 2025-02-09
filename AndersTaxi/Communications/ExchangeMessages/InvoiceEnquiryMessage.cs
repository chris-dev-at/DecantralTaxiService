using Communications.Models;

namespace Communications.ExchangeMessages;

public class InvoiceEnquiryMessage : ExchangeMessage
{
    public Passenger Passenger { get; set; }
    public double Amount { get; set; }
    
    public InvoiceEnquiryMessage()
    {
        Type = MessageType.InvoiceEnquiry;
    }
}