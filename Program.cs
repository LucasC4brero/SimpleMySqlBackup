using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Net;

namespace backupPainel
{
    internal class Program
    {
        #region Settings

        //Backup settings:
        string ServerAdress = "";
        string SqlUser = "";
        string SqlPassword = "";
        string DataBaseName = "";
        string outputFilePath = @"";
        string outputFileName = "" + DateTime.Now.ToString("yy.M.d") + ".sql";

        //Email settings:
        string emailAdress = "";
        string emailPassword = "";
        string emailHost = "";
        int outputPort = 0;

        string recipient = "";
        string subject = "";
        string body /* backup ok */ = $"";
        string body1 /* backup not ok */ = $"";


        //               Example:


        //            Backup settings:

        //      string ServerAdress = "192.168.10.27";
        //      string SqlUser = "LucasC4brero";
        //      string SqlPassword = "C4brero@1234";
        //      string DataBaseName = "cabreirices";
        //      string outputFilePath = @"C:\mySqlBackup\";
        //      string outputFileName = "My backup Today's Date is " + DateTime.Now.ToString("yy.M.d") + ".sql";

        //            Email settings:

        //      string emailAdress = "lucasc4brero@gmail.com";
        //      string emailPassword = "C4brero@9999999";
        //      string emailHost = "smtp.gmail.com;
        //      int outputPort = 465;

        //      string recipient = "public@gmail.com";
        //      string subject = "MySql Backup ("+ServerAdress+")";
        //      string body = $"Backup finished as expected u.u ";
        //      string body1 = $"Backup didn't worked *panic* ";


        #endregion Settings

        static bool bkpOk = true;

        static void Main(string[] args)
        {
            var open = new Program();
            open.doBackup();
        }

        void Program_Load()
        {
            doBackup();
        }

        void doBackup()
        {

            try
            {
                using (MySqlConnection conexão = new MySqlConnection("server = " + ServerAdress + "; user = " + SqlUser + "; pwd = " + SqlPassword + "; database = " + DataBaseName + ";"))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mySqlBackup = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conexão;
                            conexão.Open();
                            mySqlBackup.ExportToFile(outputFilePath + outputFileName);
                            conexão.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
                bkpOk = false;
                throw;
            }
            finally
            {
                sendMail();
            }
        }

        void sendMail()
        {

            var dateTime = DateTime.Now;
            TimeZoneInfo.ConvertTimeToUtc(dateTime);

            using SmtpClient email = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = outputPort,
                UseDefaultCredentials = false,
                Host = emailHost,
                EnableSsl = false,
                Credentials = new NetworkCredential(emailAdress, emailPassword),
            };

            if (bkpOk == true)
            {
                email.Send(emailAdress, recipient, subject, body);
            }
            else
            {
                email.Send(emailAdress, recipient, subject, body1);
            }
        }
    }
}