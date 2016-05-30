using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using System.Data.SqlClient;
using EMT_Exception;
using System.Net.Mail;

namespace EMT_EmailSender
{
    public class SendGridMailer
    {
        private string Key;
        Web transportWeb;
        EMTException exp = null;
        public SendGridMailer()
        {
            try
            {
                AppSettingsReader MyReader = new AppSettingsReader();
                Key = MyReader.GetValue("SendGridKey", typeof(string)).ToString();
                transportWeb = new Web(Key);
            }
            catch (SmtpException ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.SMTPExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SMTPExceptions.ToString());
                exp.LogException();
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
        }
        
        public void SendHtmlMail(string ToEmail, string ToName, string FromEmail, string FromName,string EmailSubject, string EmailBody)
        {
            var SGMsg = new SendGridMessage();
            SGMsg.From = new System.Net.Mail.MailAddress(FromName + "<" + FromEmail + ">");
            SGMsg.Subject = EmailSubject;
            SGMsg.Html = EmailBody;
            SGMsg.AddTo(ToName + "<" + ToEmail + ">");
            try
            {
                transportWeb.DeliverAsync(SGMsg);
            }
            catch (SmtpException ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.SMTPExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SMTPExceptions.ToString());
                exp.LogException();
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
           
        }
        public void SendPlainTextMail(string ToEmail, string ToName, string FromEmail, string FromName, string EmailSubject, string EmailBody)
        {
            var SGMsg = new SendGridMessage();
            SGMsg.From = new System.Net.Mail.MailAddress(FromName + "<" + FromEmail + ">");
            SGMsg.Subject = EmailSubject;
            SGMsg.Text = EmailBody;
            SGMsg.AddTo(ToName + "<" + ToEmail + ">");
            try
            {
                transportWeb.DeliverAsync(SGMsg);
            }
            catch (SmtpException ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.SMTPExceptions, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SMTPExceptions.ToString());
                exp.LogException();
            }
            catch (Exception ex)
            {
                exp = new EMTException((int)EMTException.ErorrTypes.others, ex.InnerException.Message, ex.StackTrace, EMTException.ErorrTypes.SqlExceptions.ToString());
                exp.LogException();
            }
           
        }

        
    }

    
}
