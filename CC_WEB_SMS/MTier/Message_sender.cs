using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Data;
using System.Web;
using System.Data.Odbc;
using System.Data;
using System.IO;
using Microsoft.Win32;
using System.Net;

namespace CC_WEB_SMS.MTier
{
    class Message_sender
    {
        private string dsn;
        public string _con_string;
        private string ret_val = null;
        public static OdbcConnection _conn;// = new OdbcConnection();
        ErrorTracker Track_obj = new ErrorTracker();
        //OnLoad_Defaults OnLoad_obj = new OnLoad_Defaults();
        DBClass.Queries Q_obj = new DBClass.Queries();
        private static readonly object ReaderLock = new object();
        public string mob_no;
        public string message;
        public int inout_id;
        public string message_status;
        public int counter;
        public string provider_name;
        public string provider_user_name;
        public string provider_password;
        public string provider_SenderName;
        public string provider_workingkey;
        public string message_return_id;

        public void MessageSender()
        {
            DataTable DT_obj = new DataTable();
            provider_name = LoadDefaults.ProviderName;
            provider_user_name = LoadDefaults.ProviderUID;
            provider_password = LoadDefaults.ProviderPassword;
            provider_SenderName = LoadDefaults.SenderName;
            provider_workingkey = LoadDefaults.ProviderKey;
            string sql = "select mobile_no,messages,sms_inout_id,status from cc.sms_inout where in_out='O' and status ='Y' ";
            lock (ReaderLock)
            {
                DT_obj = (DataTable)Connection_DataTable(sql);
                if (DT_obj != null)
                {
                    for (int i = 0; i < DT_obj.Rows.Count; i++)
                    {
                        mob_no = Convert.ToString(DT_obj.Rows[i]["mobile_no"]);
                        message = Convert.ToString(DT_obj.Rows[i]["messages"]);
                        inout_id = Convert.ToInt32(DT_obj.Rows[i]["sms_inout_id"]);
                        message_status = Convert.ToString(DT_obj.Rows[i]["status"]);
                        provider_selector(mob_no, message, inout_id);
                    }
                }
            }

        }

        public void provider_selector(string MobNo, string Messages, int sms_inout_id)
        {
            try
            {

                switch (provider_name)
                {
                    case "gup shup":
                        message_return_id = sendwebmessage_gupshup(MobNo, Messages, provider_user_name, provider_password);
                        break;
                    case "green ads sap":
                        message_return_id = sendwebmessage_greenads_sap(MobNo, Messages, provider_user_name, provider_password, provider_SenderName);
                        break;
                    case "green ads":
                        message_return_id = sendwebmessage_greenads(MobNo, Messages, provider_user_name, provider_password, provider_SenderName);
                        break;
                    case "tech soul":
                        message_return_id = sendwebmessage_techsoul(MobNo, Messages, provider_workingkey, provider_SenderName);
                        break;
                    case "prudent tech":
                        message_return_id = sendwebmessage_prudent_tech(MobNo, Messages, provider_user_name, provider_password, provider_SenderName);
                        break;
                    case "monotone":
                        message_return_id = sendwebmessage_monotone(MobNo, Messages, provider_user_name, provider_password);
                        break;
                    case "bhashsms":
                        message_return_id = sendwebmessage_bhashsms(MobNo, Messages, provider_user_name, provider_password, provider_SenderName);
                        break;
                    case "prpsms":
                        message_return_id = sendwebmessage_prpsms(MobNo, Messages, provider_user_name, provider_password, provider_SenderName);
                        break;
                    default:
                        Track_obj.Text_Tracker("Invalid Provider Name-Message sender class");
                        break;
                }
                if (Convert.ToString(message_return_id) != null && Convert.ToString(message_return_id) != "")
                {
                    DBClass.Root_Sen root_obj_in = new DBClass.Root_Sen();
                    string sql8 = "select sf_update_sms_delivery_status('D','" + message_return_id + "','N','" + sms_inout_id + "') result";
                    // string sql8 = "update cc.sms_inout set delivery_status='D',message_id='" + message_return_id + "',status='N' where sms_inout_id='" + sms_inout_id + "'";
                    root_obj_in.db_select_operation(sql8,"result");
                }
                else
                {
                    DBClass.Root_Sen root_obj_in = new DBClass.Root_Sen();
                    string sql8 = "select sf_update_sms_delivery_status('F','" + message_return_id + "','N','" + sms_inout_id + "') result";
                    // string sql1 = "update cc.sms_inout set delivery_status='F',message_id='" + message_return_id + "',status='N' where sms_inout_id='" + sms_inout_id + "'";
                    root_obj_in.db_select_operation(sql8, "result");
                }
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }

        }

        public void connection_open()
        {
            dsn = RegLoader.Dsn;
            string first = "#websms*321*#";
            string middle = "busopasnosty#";
            string last = "rotartsinimda123*#";
            _con_string = "DSN=" + dsn + ";uid=CCBankingSMS;pwd="+first+middle+last;
            _conn = new OdbcConnection(_con_string);
            if (_conn.State == ConnectionState.Open)
                _conn.Close();
            _conn.Open();
        }

        public DataTable Connection_DataTable(string _sql_string)
        {

            try
            {
                DataTable DT_Obj = new DataTable();
                lock (ReaderLock)
                {
                    connection_open();
                    using (var _command_obj = new OdbcCommand(_sql_string, _conn))
                    {

                        //connection_open();
                        using (var DA = new OdbcDataAdapter(_command_obj))
                        {
                            _conn.Close();
                            DA.Fill(DT_Obj);
                        }

                    }
                }
                return DT_Obj;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
        public void db_InsertUpdateDelete_Operations(string _sql_string)
        {
            try
            {
                connection_open();
                OdbcCommand _command_obj = new OdbcCommand(_sql_string, _conn);
                _command_obj.ExecuteNonQuery();
                _conn.Close();

            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }
        }
        /// <summary>
        /// MESSAGE SENDING FUNCTIONS
        /// </summary>
        public string sendwebmessage_greenads(string phone, string message, string user_name, string password, string sender_id)
        {
            try
            {

                string ls_url = "http://textsms.greenadsglobal.com/sendsms?uname=" + user_name + "&pwd=" + password + "&senderid=" + sender_id + "&to=" + phone + "&msg=" + message + "&route=T";
                System.Net.HttpWebRequest myrequest = (HttpWebRequest)WebRequest.Create(ls_url);
                myrequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse webResponse = (HttpWebResponse)myrequest.GetResponse();
                //SourceStream = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                string str = reader.ReadLine();
                string sl_message_id = str;
                return sl_message_id;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
        public string sendwebmessage_greenads_sap(string phone, string message, string user_name, string password, string sender_id)
        {
            try
            {

                int l_index;
                string ls_url = "http://sapteleservices.in/SMS_API/sendsms.php?username=" + user_name + "&password=" + password + "&mobile=" + phone + "&sendername=" + sender_id + "&message=" + message + "&routetype=1";
                System.Net.HttpWebRequest myrequest = (HttpWebRequest)WebRequest.Create(ls_url);
                myrequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse webResponse = (HttpWebResponse)myrequest.GetResponse();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                string str = reader.ReadLine();
                l_index = str.LastIndexOf(':') + 2;
                string sl_message_id = str.Substring(l_index, (str.Length - l_index));
                return sl_message_id;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
        public string sendwebmessage_prpsms(string phone, string message, string user_name, string password, string sender_id)
        {
            try
            {

                int l_index;
                string ls_url = "http://103.247.98.91/API/SendMsg.aspx?uname="+user_name+"&pass="+password+"&send="+sender_id+"&dest="+phone+"&msg="+message+"&priority=1";
                System.Net.HttpWebRequest myrequest = (HttpWebRequest)WebRequest.Create(ls_url);
                myrequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse webResponse = (HttpWebResponse)myrequest.GetResponse();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                string str = reader.ReadLine();
                l_index = str.LastIndexOf('-') + 1;
                string sl_message_id = str.Substring(l_index, (str.Length - l_index));
                return sl_message_id;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
        public string sendwebmessage_gupshup(string phone, string message, string user_name, string password)
        {
            try
            {
                string result = "";
                WebRequest request = null;
                HttpWebResponse response = null;
                string _message_id = null, _message_status;
                String url = "http://enterprise.smsgupshup.com/GatewayAPI/rest?method=sendMessage&send_to=" + phone + "&msg=" + message + "&userid=" + user_name + "&password=" + password + "&v=1.1 & msg_type = TEXT & auth_scheme = PLAIN";
                request = WebRequest.Create(url);
                //in case u work behind proxy, uncomment the
                //commented code and provide correct details;
                /*WebProxy proxy = new WebProxy("http://proxy:80/",true);
                proxy.Credentials = new
                NetworkCredential("userId","password", "Domain");
                request.Proxy = proxy;*/
                // Send the 'HttpWebRequest' and wait for response.
                response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding ec = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader reader = new System.IO.StreamReader(stream, ec);
                result = reader.ReadToEnd();
                result = result.Replace('\n', ' ');
                _message_status = result.Substring(0, result.IndexOf('|'));

                if (_message_status.ToLower() == "success ")
                {
                    Int32 _last_index = result.LastIndexOf('|');
                    _last_index = _last_index + 1;
                    phone = phone.Substring(3, phone.Length - 3);
                    _message_id = result.Substring(_last_index, Convert.ToInt32(result.Length) - _last_index);
                    //string sql = query.UpdateSmsinout("delivery_status='D'," + "message_id='" + _message_id + "',status='N'", " where mobile_no='" + phone + "'" + " and messages='" + message + "'");
                    //objcclsSMS.strexecutescalar(sql);

                }
                reader.Close();
                stream.Close();
                return _message_id;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }




        }
        public string sendwebmessage_monotone(string phone, string message, string user_name, string password)
        {
            try
            {
                string result = "";
                WebRequest request = null;
                HttpWebResponse response = null;
                string _message_id = null, _message_status;
                String url = "http://monotone.co.in/GatewayAPI/rest?method=SendMessage&msg_type=TEXT&loginid=" + user_name + "&auth_scheme=plain&password=" + password + "&v=1.1&format=text&send_to="+phone+"&msg="+message+"";
                request = WebRequest.Create(url);               
                response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding ec = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader reader = new System.IO.StreamReader(stream, ec);
                result = reader.ReadToEnd();

                _message_status = result.Substring(0, result.IndexOf('|'));

                if (_message_status.ToLower() == "success ")
                {
                    Int32 _last_index = result.LastIndexOf('|');
                    _last_index = _last_index + 1;
                    phone = phone.Substring(3, phone.Length - 3);
                    _message_id = result.Substring(_last_index, Convert.ToInt32(result.Length) - _last_index);
                    //string sql = query.UpdateSmsinout("delivery_status='D'," + "message_id='" + _message_id + "',status='N'", " where mobile_no='" + phone + "'" + " and messages='" + message + "'");
                    //objcclsSMS.strexecutescalar(sql);

                }
                reader.Close();
                stream.Close();
                return _message_id;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
        public string sendwebmessage_greenads_open_route(string phone, string message, string user_name, string password, string sender_id)
        {
            try
            {
                string ls_url = "http://mobilegateway.in:8080/sendsms/bulksms?username=" + user_name + "&password=" + password + "&type=0&dlr=1&destination=" + phone + "&source=" + sender_id + "&message=" + message;

                //string ls_url = "http://textsms.greenadsglobal.com/sendsms?uname=" + user_name + "&pwd=" + password + "&senderid=" + sender_id + "&to=" + phone + "&msg=" + message + "&route=T";
                System.Net.HttpWebRequest myrequest = (HttpWebRequest)WebRequest.Create(ls_url);
                myrequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse webResponse = (HttpWebResponse)myrequest.GetResponse();
                //SourceStream = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                string str = reader.ReadLine();
                string sl_message_id = str;
                return sl_message_id;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
        public string sendwebmessage_techsoul(string phone, string message, string wkey, string sender)
        {
            try
            {
                //string ls_url = "http://tx.techsoltech.com/api/web2sms.php?workingkey=98450za22cx313444k7r&message=Welcome%20to%20CCBanking.have%20a%20nice%20day&sender=CCBANK&to=9846374729";
                string ls_url = "http://tx.techsoltech.com/api/web2sms.php?workingkey=" + wkey + "&message=" + message + "&sender=" + sender + "&to=" + phone;
                System.Net.HttpWebRequest myrequest = (HttpWebRequest)WebRequest.Create(ls_url);
                myrequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse webResponse = (HttpWebResponse)myrequest.GetResponse();
                //SourceStream = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                string str = reader.ReadLine();
                int a = str.LastIndexOf('=') + 1;
                string sl_message_id = str.Substring(a, (str.Length - a));
                return sl_message_id;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
        public string sendwebmessage_prudent_tech(string phone, string message, string user_name, string password, string sender_id)
        {
            try
            {

                //int l_index;
                //string ls_url = "http://sapteleservices.in/SMS_API/sendsms.php?username=" + user_name + "&password=" + password + "&mobile=" + phone + "&sendername=" + sender_id + "&message=" + message + "&routetype=1";
                string ls_url = "http://api.mVaayoo.com/mvaayooapi/MessageCompose?user="+user_name+":"+password+"&senderID="+sender_id+"&receipientno="+phone+"&msgtxt="+message+"&state=4";
                System.Net.HttpWebRequest myrequest = (HttpWebRequest)WebRequest.Create(ls_url);
                myrequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse webResponse = (HttpWebResponse)myrequest.GetResponse();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                string str = reader.ReadLine();
                //l_index = str.LastIndexOf(':') + 2;
                //string sl_message_id = str.Substring(l_index, (str.Length - l_index));
                return str;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
        public string sendwebmessage_bhashsms(string phone, string message, string user_name, string password, string sender_id)
        {
            try
            {                
                int l_index;
                string ls_url = "http://bhashsms.com/api/sendmsg.php?user=" + user_name + "&pass=" + password + "&sender=" + sender_id + "&phone=" + phone + "&text=" + message + "&priority=ndnd&stype=normal";
                Track_obj.Text_Tracker(ls_url);
                System.Net.HttpWebRequest myrequest = (HttpWebRequest)WebRequest.Create(ls_url);
                myrequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse webResponse = (HttpWebResponse)myrequest.GetResponse();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                string str = reader.ReadLine();
                l_index = str.LastIndexOf('.');
                string sl_message_id = str.Substring(l_index, (str.Length - l_index));
                return sl_message_id;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
        public string delivery_status_techsoul(string wkey, string message_id)
        {
            try
            {
                string url = "http://tx.techsoltech.com/api/status.php?workingkey=" + wkey + "&messageid=" + message_id;
                System.Net.HttpWebRequest myrequest = (HttpWebRequest)WebRequest.Create(url);
                myrequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse webResponse = (HttpWebResponse)myrequest.GetResponse();
                //SourceStream = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                string str = reader.ReadLine();
                return str;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
        public string delivery_status_greenads(string user_name, string password, string message_id)
        {
            try
            {
                //http://textsms.greenadsglobal.com/getdelivery/yourUsername/yourPassword/messageID
                string url = "http://textsms.greenadsglobal.com/getdelivery/" + user_name + "/" + password + "/" + message_id;
                System.Net.HttpWebRequest myrequest = (HttpWebRequest)WebRequest.Create(url);
                myrequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse webResponse = (HttpWebResponse)myrequest.GetResponse();
                //SourceStream = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                string str = reader.ReadLine();
                return str;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
        public long delivery_status_greenads_sap(string user_name, string password, string message_id)
        {
            try
            {

                //http://textsms.greenadsglobal.com/getdelivery/yourUsername/yourPassword/messageID
                string url = "http://sapteleservices.in/getdelivery/" + user_name + "/" + password + "/" + message_id;
                System.Net.HttpWebRequest myrequest = (HttpWebRequest)WebRequest.Create(url);
                myrequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse webResponse = (HttpWebResponse)myrequest.GetResponse();
                //SourceStream = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                long ll_status = 0;
                //
                string str = reader.ReadLine();
                return 0;

            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return 0;
            }

        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////END SENDING FUNCTION REGION////////////////////////////////////////////////////////////////////////////////////////////////




    }

}
