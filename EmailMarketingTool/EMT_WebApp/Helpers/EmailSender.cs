using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EMT_WebApp.Helpers
{
    public class EmailSender
    {
        private MailMessage Message = null;
        private SmtpClient smtpClient = null;
        public MailAddress FromAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        //Default constructor
        public EmailSender()
        {
            smtpClient = new SmtpClient();
            smtpClient.Host = ConfigurationManager.AppSettings["MailServer"]; //Configure as email provider
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MailAuthUser"], ConfigurationManager.AppSettings["MailAuthPass"]);
            smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            Message = new MailMessage();
        }

        //constructor with parameter
        public EmailSender(string host, int port, string userName, string password, bool ssl)
: this()
        {
            smtpClient.Host = host;
            smtpClient.Port = port;
            smtpClient.EnableSsl = ssl;
            smtpClient.Credentials = new NetworkCredential(userName, password);
        }

        //function to add To for email
        public void AddToAddress(string email, string name = null)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.Replace(",", ";");
                string[] emailList = email.Split(';');
                for (int i = 0; i < emailList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(emailList[i]))
                        Message.To.Add(new MailAddress(emailList[i], name));
                }
            }
        }

        //add CC for email
        public void AddCcAddress(string email, string name = null)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.Replace(",", ";");
                string[] emailList = email.Split(';');
                for (int i = 0; i < emailList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(emailList[i]))
                        Message.CC.Add(new MailAddress(emailList[i], name));
                }
            }
        }

        //add BCC for email
        public void AddBccAddress(string email, string name = null)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.Replace(",", ";");
                string[] emailList = email.Split(';');
                for (int i = 0; i < emailList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(emailList[i]))
                        Message.Bcc.Add(new MailAddress(emailList[i], name));
                }
            }
        }

        //Send Email
        public async Task SendMailAsync(int messageType)
        {
            Message = GetEmailMessage(messageType);
            try
            {
                await smtpClient.SendMailAsync(Message);
            }
            catch (SmtpException ex)
            {
                throw new SenderInvocationException("Could not send message", ex);
            }

        }

        public MailMessage GetEmailMessage(int messageType)
        {
            if (FromAddress == null || (FromAddress != null && FromAddress.Address.Equals("")))
            {
                throw new Exception("From address not defined");
            }
            else
            {
                if (string.IsNullOrEmpty(FromAddress.DisplayName))
                    FromAddress = new MailAddress(FromAddress.Address, string.Empty);
                Message.From = FromAddress;
            }
            if (Message.To.Count <= 0)
            {
                throw new Exception("To address not defined");
            }
            if (messageType == 1)
            {
                Message.BodyEncoding = Encoding.UTF8;
                Message.SubjectEncoding = Encoding.UTF8;
                Message.Subject = Subject;
                Message.IsBodyHtml = true;
                Message.Body = Body;
                // Message.Headers.Add("Unsubscribe", "|UNSUB:http://localhost:40660.com/List/UnSubscribe" + Message.To + "|");
                Message.Headers.Add("List-Unsubscribe", "<mailto:list - request@host.com ? subject = unsubscribe >< http ://www.host.com/list.cgi?cmd=unsub&lst=list>");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, MediaTypeNames.Text.Html);
                Message.AlternateViews.Add(htmlView);
                return Message;
            }
            else {
                Message.Subject = Subject;
                Message.IsBodyHtml = false;
                Message.Body = Body;
                return Message;
            }
        }
    }
}