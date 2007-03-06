using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace PractiSES
{
    class Email
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
            message.BodyEncoding = System.Text.Encoding.UTF8;
        }

        public bool Send()
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.su.sabanciuniv.edu");
                client.Credentials = new NetworkCredential("Sysadmin_Username", "Sysadmin_Password");
                client.EnableSsl = true;
                client.Port = 25;
                client.Send(message);
                messageSent = true;
            }
            catch (Exception ex)
            {
                messageSent = false;
                Console.WriteLine("Exception: " + ex.Message);
            }
            return messageSent;

        }
    }
}
