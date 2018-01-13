using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Net;

namespace CC_WEB_SMS.MTier
{
    class MessageBalance
    {

        public long get_balance_greenads_sap(string username, string password)
        {
            try
            {
                {
                    // http://sapteleservices.in/SMS_API/balanceinfo.php?username=demo&password=
                    //http://textsms.greenadsglobal.com/getdelivery/yourUsername/yourPassword/messageID
                    int l_index;
                    int sl_index;
                    string url = "http://sapteleservices.in/SMS_API/balanceinfo.php?username=" + username + "&password=" + password;
                    System.Net.HttpWebRequest myrequest = (HttpWebRequest)WebRequest.Create(url);
                    myrequest.Credentials = CredentialCache.DefaultCredentials;
                    HttpWebResponse webResponse = (HttpWebResponse)myrequest.GetResponse();
                    StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                    string str = reader.ReadLine();
                    l_index = str.LastIndexOf('<');
                    str = str.Substring(0, l_index);
                    sl_index = str.LastIndexOf('-') + 1;
                    str = str.Substring(sl_index, (str.Length - sl_index));
                    return Convert.ToInt64(str);

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "CCSMS 1111", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Text_Tracker("Error 1010 Balance check error SAP" + ex.ToString());
                return 0;
            }
        }



        public long get_balance_greenads(string username, string password, string rout)
        {
            try
            {
                {
                    //http://textsms.greenadsglobal.com/getdelivery/yourUsername/yourPassword/messageID

                    string url = "http://textsms.greenadsglobal.com/creditsleft/" + username + "/" + password + "/" + rout;
                    System.Net.HttpWebRequest myrequest = (HttpWebRequest)WebRequest.Create(url);
                    myrequest.Credentials = CredentialCache.DefaultCredentials;
                    HttpWebResponse webResponse = (HttpWebResponse)myrequest.GetResponse();
                    StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                    string str = reader.ReadLine();
                    return Convert.ToInt64(str);

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "CCSMS 1011", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Text_Tracker("Error 1009 Balance check error" + ex.ToString());
                return 0;
            }
        }

    }
}
