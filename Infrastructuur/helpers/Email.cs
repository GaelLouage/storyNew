using Infrastructuur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.helpers
{
    public static class Email
    {
        public static void SendEmail(ContactEntity contact, out MailMessage message, out SmtpClient smtpClient)
        {
            var credentials = ReadJson.GetEmailCredentials(@"C:/Users/louag/Desktop/storyContactCredentials/credentials.json");
            contact.EmailAddress = credentials.EmailAddress;
            contact.PassWord = credentials.PassWord;
            // Set the email details
            message = new MailMessage();
            message.From = new MailAddress(contact.UserEmail);
            message.To.Add(contact.EmailAddress);
            message.Subject = "Contact";
            message.Body = contact.Message;

            // Set the server details
            smtpClient = new SmtpClient();
            // use hotmail to send emails
            smtpClient.Host = "smtp.office365.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(contact.EmailAddress, credentials.PassWord);
            // Send the email
            smtpClient.Send(message);
        }
    }
}
