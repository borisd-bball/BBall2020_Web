using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Text;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Mail;

namespace mr.bBall_Lib
{
    public static class Email
    {
        public static bool Send(string to_mails, string subject, string body)
        {
            return Send(to_mails, subject, body, false);
        }
        public static bool Send(string to_mails, string subject, string body, bool priority)
        {
            string msg;
            return Send(to_mails, subject, body, priority, subject, null, out msg);
        }
        public static bool Send(string to_mails, string subject, string body, bool priority, string title, Dictionary<string, Stream> files, out string msg)
        {
            msg = "";
            bool ret = true;
            try
            {
                string template = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "template.html", Encoding.UTF8);
                foreach (string email in to_mails.Split(','))
                {
                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress(ConfigurationManager.AppSettings["FromEmail"], Nastavitve.Naziv, Encoding.UTF8);
                        message.To.Add(email);
                        message.Subject = subject;
                        message.Body = template
                            .Replace("@title", title)
                            .Replace("@text", body)
                            .Replace("@nazivdolg", Nastavitve.NazivDolg)
                            .Replace("@naslov", Nastavitve.Naslov)
                            .Replace("@emailfrom", Nastavitve.EmailFrom)
                            .Replace("@telefon", Nastavitve.Telefon)
                            .Replace("@skupina", ConfigurationManager.AppSettings["Skupina"])
                            .Replace("@spletnastran", Nastavitve.SpletnaStran)
                            .Replace("@naziv", Nastavitve.Naziv)
                            .Replace("@email", email);
                        message.IsBodyHtml = true;
                        message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                        message.SubjectEncoding = Encoding.UTF8;
                        message.BodyEncoding = Encoding.UTF8;
                        message.HeadersEncoding = Encoding.UTF8;
                        if (files != null)
                        {
                            foreach (var item in files) message.Attachments.Add(new Attachment(item.Value, item.Key));
                        }
                        if (priority) message.Priority = MailPriority.High;
                        using (SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["SmtpHost"], Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"])))
                        {
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;
                            client.UseDefaultCredentials = false;
                            client.EnableSsl = true;
                            client.Timeout = 10000;
                            client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SmtpUsername"], ConfigurationManager.AppSettings["SmtpPassword"]);
                            client.Send(message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                msg = e.ToString();
                ret = false;
            }
            return ret;
        }
    }
}