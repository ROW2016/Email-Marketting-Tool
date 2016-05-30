using EMT_DataAccessLayer;
using EMT_EmailSender;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EMT_EmailHelper
{
   public class MailComposer
    {
   
        EmailDetails emailDetails;
        CampaignDetails campaign = null;
        List<SubscriberDetails> subs = null;
        SubscriberDetails sub = null;
        /// <summary>
        /// Compose plain text mail message
        /// </summary>
        public string ComposePlainTextMail(string emailContent, string name, string subscriberidentity)
        {

            string content = null;

            content = prepareTEXTMailMessage(emailContent, name, subscriberidentity);

            return content;
        }

        /// <summary>
        /// Compose HTML mail message
        /// </summary>
        public string ComposeHTMLMail(string emailContent, string name, string subscriberidentity)
        {
           
            string content = null;
          
            content = prepareHTMLMailMessage(emailContent, name, subscriberidentity);
              
            return content;
        }

        /// <summary>
        /// prepare HTML mail message with body and footer for sending
        /// </summary>
        /// <param name="emailContent"></param>
        /// <param name="name"></param>
        /// <param name="subIdentity"></param>
        /// <returns></returns>
        public string prepareHTMLMailMessage(string emailContent, string name, string subscriberidentity)
        {
            string message = null;
            string pattern = "{}";
            Regex reg = new Regex(pattern);
            message = reg.Replace(emailContent, name);
            message = WebUtility.HtmlDecode(message);

            message += getFooter(subscriberidentity);
            return message;
        }

        /// <summary>
        /// prepare TEXT mail message with body and footer for sending
        /// </summary>
        /// <param name="emailContent"></param>
        /// <param name="name"></param>
        /// <param name="subIdentity"></param>
        /// <returns></returns>
        public string prepareTEXTMailMessage(string emailContent, string name, string subscriberidentity)
        {
            string message = null;
            string pattern = "{}";
            Regex reg = new Regex(pattern);
            message = reg.Replace(emailContent, name);
            message += getFooter(subscriberidentity);
            return message;
        }
        /// <summary>
        /// returns customized footer for mail message
        /// </summary>
        /// <param name="subscriberidentity"></param>
        /// <returns></returns>
        public string getFooter(string subscriberidentity)
        {
            string foot, imgNm;
            // string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            imgNm = subscriberidentity+".png";

            foot = "<br><br><br><br><br><br>";
            foot += @"<table width='100%' style='background-color:#ffffff;border-top:1px solid #e5e5e5' > 
            <tr>    
            <td align='center'>";

            foot += " <img src='http://uemttrk.azurewebsites.net/image/";
            foot += imgNm;// imgNm;
            foot += "'  height='30px' alt='Logo' />";

            foot += @"
            <br />     
            <p style='font-size:10px'>Copyright © 2016 Return On Web pvt Ltd. All rights reserved.</p>
            </td>
            </tr>
            <tr>
            <td align='center' style='font-size:8px'>
            <p>Don't want it in your inbox ? <a href='" + "http://emailertool.azurewebsites.net/Subscriber/UnsubscribeEmail?subid=" + subscriberidentity;
            //foot += subscriber;
            foot += @"'><unsubscribe>Unsubscribe</unsubscribe></a>.
            </td>
            </tr>
            </table>";
            return foot;
        }
      

    }
}
