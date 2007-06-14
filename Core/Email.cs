using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PractiSES
{
    public class Email
    {
        private MailMessage message = null;
        private bool messageSent = false;

        public Email(String pstrTo, String pstrSubject, String pstrBody)
        {
            message = new MailMessage();
            message.From = new MailAddress("mergun@su.sabanciuniv.edu", "PractiSES Management Module");
            message.To.Add(pstrTo);
            message.Subject = pstrSubject;
            message.Priority = MailPriority.High;
            message.Body = pstrBody;
            message.BodyEncoding = Encoding.UTF8;
        }

        public bool Send()
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.su.sabanciuniv.edu");
                client.Credentials = new NetworkCredential("username", "password");
                client.EnableSsl = true;
                client.Port = 25;
                client.Send(message);
                messageSent = true;
            }
            catch (Exception e)
            {
                messageSent = false;
                Console.WriteLine("Exception: " + e.Message);
            }
            return messageSent;
        }
    }
}