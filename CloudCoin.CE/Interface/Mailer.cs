using System;
namespace CloudCoin.CE.Interface
{
    public interface Mailer
    {
        string GetHomeFolder();
        void SendMail(string folder, string[] filename);
    }
}
