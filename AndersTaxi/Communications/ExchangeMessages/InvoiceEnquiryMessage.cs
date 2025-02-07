namespace Communications.ExchangeMessages;

public class InvoiceEnquiryMessage : ExchangeMessage
{
    public double Amount { get; set; }
    
    public InvoiceEnquiryMessage()
    {
        Type = MessageType.InvoiceEnquiry;
    }
}