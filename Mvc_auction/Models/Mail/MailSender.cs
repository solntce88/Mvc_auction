using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;

namespace Mvc_auction.Models
{
    public class MailSender
    {
      static  public void SendMail(int messageType, User user, Lot lot = null)
        {

            Message message = GetMessage(messageType);
            string fullName = String.Concat(user.name, " ", user.lastName); 
            switch (messageType) 
            {
                //Registration
                #region Registration
                case 1: 
                    {
                        string link = HttpContext.Current.Request.Url.Authority+"/Account/Activate/" +  user.userName + "/" + user.newMail;
                        var newMessage = new MailMessage(ConfigurationManager.AppSettings["MailName"], user.mail)
                        {
                            Subject = message.Subject,
                            Body = String.Format(message.Body, link) // 0-activation link
                        };

                        var client = new SmtpClient();
                        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailName"], ConfigurationManager.AppSettings["MailPassword"]);
                        client.Send(newMessage);
                    }
                    break;
                #endregion
                #region Deleted lot
                case 2: // Deleted lot
                    {
                        
                        var newMessage = new MailMessage(ConfigurationManager.AppSettings["MailName"], user.mail)
                        {
                            Subject = message.Subject,
                            Body = String.Format(message.Body, fullName, lot.Name + " (" + lot.Description + ")") 
                        };

                        var client = new SmtpClient();
                        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailName"], ConfigurationManager.AppSettings["MailPassword"]);
                        client.Send(newMessage);
                    }
                    break;
                    #endregion
                #region Deleted user
                case 3: //Deleted user
                    {
                        var newMessage = new MailMessage(ConfigurationManager.AppSettings["MailName"], user.mail)
                        {
                            Subject = message.Subject,
                            Body = String.Format(message.Body, fullName, user.userName) 
                        };

                        var client = new SmtpClient();
                        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailName"], ConfigurationManager.AppSettings["MailPassword"]);
                        client.Send(newMessage);
                    }
                    break;
                    #endregion
                #region To winner customer
                case 4: //To winner customer
                    {
                        var newMessage = new MailMessage(ConfigurationManager.AppSettings["MailName"], user.mail)
                        {
                            Subject = String.Format(message.Subject,lot.Name),
                            Body = String.Format(message.Body, fullName, lot.Name, lot.Price) // 0-
                        };

                        var client = new SmtpClient();
                        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailName"], ConfigurationManager.AppSettings["MailPassword"]);
                        client.Send(newMessage);
                    }
                    break;
                    #endregion
                #region Broken bet to customer
                case 5: //Broken bet to customer
                    {
                        var newMessage = new MailMessage(ConfigurationManager.AppSettings["MailName"], user.mail)
                        {
                            Subject = message.Subject,
                            Body = String.Format(message.Body, fullName, lot.Price, lot.Name) // 0-
                        };

                        var client = new SmtpClient();
                        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailName"], ConfigurationManager.AppSettings["MailPassword"]);
                        client.Send(newMessage);
                    }
                    break;
                    #endregion
                #region To owner successfull end of auction
                case 6:   // To owner successfull end of auction
                    {
                        var newMessage = new MailMessage(ConfigurationManager.AppSettings["MailName"], user.mail)
                        {
                            Subject = String.Format(message.Subject, lot.Name),
                            Body = String.Format(message.Body, fullName, lot.Name, lot.Price) // 0-
                        };

                        var client = new SmtpClient();
                        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailName"], ConfigurationManager.AppSettings["MailPassword"]);
                        client.Send(newMessage);
                    }
                    break;
                    #endregion
                #region To owner auction failed
                case 7:   // To owner auction failed
                    {
                        var newMessage = new MailMessage(ConfigurationManager.AppSettings["MailName"], user.mail)
                        {
                            Subject = message.Subject,
                            Body = String.Format(message.Body, fullName, lot.Name+": "+ lot.Description) // 0-
                        };

                        var client = new SmtpClient();
                        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailName"], ConfigurationManager.AppSettings["MailPassword"]);
                        client.Send(newMessage);
                    }
                    break;
                    #endregion
                #region Restore password
                case 8:
                    {
                        //string link = HttpContext.Current.Request.Url.Authority + "/Account/Activate/" + user.userName + "/" + user.newMail;
                        var newMessage = new MailMessage(ConfigurationManager.AppSettings["MailName"], user.mail)
                        {
                            Subject = String.Format(message.Subject, ConfigurationManager.AppSettings["SiteName"]),
                            Body = String.Format(message.Body, user.newMail) // 0-activation link
                        };

                        var client = new SmtpClient();
                        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailName"], ConfigurationManager.AppSettings["MailPassword"]);
                        client.Send(newMessage);
                    }
                    break;
                #endregion
            }
        }

        #region Getmessage from db

     static   public Message GetMessage(int typeMessage)
        {
            string connString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            string qSelect = "SELECT [Name],[subject],[body] FROM [ASPNET_appDB].[dbo].[Mail_message] where [message_id]=@id";
            Message message = new Message(typeMessage);
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(qSelect, conn);
                SqlParameter id = new SqlParameter("@id", typeMessage);
                cmd.Parameters.Add(id);
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        try { message.Name = (string)reader["Name"]; }
                        catch (InvalidCastException) { message.Name = "noName"; }

                        try
                        { message.Subject = (string)reader["subject"]; }
                        catch (InvalidCastException) { message.Subject = "noSubject"; }

                        try
                        { message.Body = (string)reader["body"]; }
                        catch (InvalidCastException) { message.Body = "noBody"; }
                    }
                }
                conn.Close();
            }
            return message;
        }
        #endregion
    }
}