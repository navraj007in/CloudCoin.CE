using System;
namespace CloudCoin.CE.Interface
{
    public interface Mailer
    {
        void SendMail(string folder, string[] filename);
    }
}
