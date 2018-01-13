using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Configuration;
using System.Data.Odbc;
using System.Data;
using System.IO;
using Microsoft.Win32;

namespace CC_WEB_SMS.DBClass
{
    class connection
    {
        private string dsn;
        public string _con_string;
        private string ret_val = null;
        public static OdbcConnection _conn = new OdbcConnection();
        ErrorTracker Track_obj = new ErrorTracker();
        private static readonly object ReaderLock = new object();

        public void db_Connection_open()
        {
            dsn = MTier.RegLoader.Dsn;
            string first = "#websms*321*#";
            string middle = "busopasnosty#";
            string last = "rotartsinimda123*#";
            _con_string = "DSN=" + dsn + ";uid=CCBankingSMS;pwd=" + first + middle + last;
            try
            {
                lock (ReaderLock)
                {
                    _conn = new OdbcConnection(_con_string);
                    if (_conn.State == ConnectionState.Closed || _conn.State == ConnectionState.Broken)
                    {
                        _conn.Open();
                    }
                    else if (_conn.State == ConnectionState.Connecting || _conn.State == ConnectionState.Fetching)
                    {
                        Thread.Sleep(9000);
                        _conn.Open();
                    }
                }
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }

        }

        public void db_connection_close(OdbcConnection con)
        {
            con.Close();
        }
    }
    class Root_Gen
    {
        private static readonly object ReaderLock = new object();
        public static OdbcConnection _conn;
        connection con_obj = new connection();
        ErrorTracker Track_obj = new ErrorTracker();
        private string ret_val = null;    
        public void db_InsertUpdateDelete_Operations(string _sql_string)
        {
            try
            {
                //db_Connection_open();
                _conn = connection._conn;
                if (_conn.State == ConnectionState.Connecting || _conn.State == ConnectionState.Executing || _conn.State == ConnectionState.Fetching)
                {
                    Thread.Sleep(8000);
                }
                else
                {
                    OdbcCommand _command_obj = new OdbcCommand(_sql_string, _conn);
                    _command_obj.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }
            // db_Connection_close();
            // _conn.Close();
        }
        public string db_select_operation(string _sql_string, string return_param)
        {
            string ret_val = null;
            OdbcDataReader new_reader;
            try
            {
                con_obj.db_Connection_open();
                _conn = connection._conn;
                if (_conn.State == ConnectionState.Open && (_conn.State != ConnectionState.Connecting || _conn.State != ConnectionState.Executing || _conn.State != ConnectionState.Fetching))
                {
                    OdbcCommand _command_obj = new OdbcCommand(_sql_string, _conn);
                    lock (ReaderLock)
                    {

                        using (new_reader = _command_obj.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (new_reader.Read())
                            {
                                ret_val = Convert.ToString(new_reader[return_param]);
                            }
                        }
                        return ret_val;
                    }
                }
                else
                    return null;
            }

            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex) + return_param);
                return null;
            }
           
        }
       
        public DataTable db_select_operation_table(string _sql_string)
        {
            DataTable DT_Obj = new DataTable();
            //DT_Obj = null;

            try
            {

                if (_conn.State != ConnectionState.Open)
                {
                    con_obj.db_Connection_open();
                }
                _conn = connection._conn;
                OdbcCommand _command_obj = new OdbcCommand(_sql_string, _conn);
                using (OdbcDataAdapter DA = new OdbcDataAdapter(_command_obj))
                {
                    lock (DA)
                    {
                        DA.Fill(DT_Obj);
                    }
                }
                _conn.Close();

            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
            return DT_Obj;

        }
        public DataTable db_select_operation_smsHistory(string _sql_string)
        {
            DataTable DT_Obj = new DataTable();

            try
            {//#websms*321*#busopasnosty#rotartsinimda123*#
                OdbcConnection conn2 = new OdbcConnection();
                string first = "#websms*321*#";
                string middle = "busopasnosty#";
                string last = "rotartsinimda123*#";
                conn2 = new OdbcConnection("DSN=" + MTier.RegLoader.Dsn + ";uid=CCBankingSMS;pwd=" + first + middle + last);
                //conn2 = new OdbcConnection("DSN=" + MTier.RegLoader.Dsn + ";uid=CCBankingSMS;pwd=CCBankingSMS");
                //if (conn2.State == ConnectionState.Open)
                //    conn2.Close();
                conn2.Open();


                if (conn2.State == ConnectionState.Open)
                {
                    OdbcCommand _command_obj = new OdbcCommand(_sql_string, conn2);
                    using (OdbcDataAdapter DA = new OdbcDataAdapter(_command_obj))
                    {
                        DA.Fill(DT_Obj);
                    }
                    conn2.Close();
                }
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }

            return DT_Obj;

        }

        public void SP_sf_update_sms(string BDate, int priority)
        {
            try
            {
                con_obj.db_Connection_open();
                _conn = connection._conn;
                OdbcParameter parm1 = new OdbcParameter();
                OdbcParameter parm2 = new OdbcParameter();
                OdbcCommand cmd = new OdbcCommand("{call sf_update_sms(?,?)}", _conn);
                cmd.CommandType = CommandType.StoredProcedure;
                parm1 = cmd.Parameters.Add("ad_date", OdbcType.Date);
                parm1.Value = BDate;
                parm2 = cmd.Parameters.Add("ai_priority", OdbcType.Int);
                parm2.Value = priority;
                cmd.ExecuteScalar();
                _conn.Close();

            }
            catch (Exception Ex)
            {
                //MessageBox.Show("Error 104" + Ex.Message, "CCSMS");
                Track_obj.Text_Tracker(Convert.ToString(Ex));
            }
        }



    }
}
