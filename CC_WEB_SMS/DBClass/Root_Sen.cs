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
    class connection1
    {
        private string dsn;
        private string _con_string;
        private string ret_val = null;
        public static OdbcConnection _conn;

        public OdbcConnection db_Connection_open()
        {
            //#websms*321*#busopasnosty#rotartsinimda123*#
            dsn = MTier.RegLoader.Dsn;
            string first = "#websms*321*#";
            string middle = "busopasnosty#";
            string last = "rotartsinimda123*#";
            _con_string = "DSN=" + dsn + ";uid=CCBankingSMS;pwd=" + first + middle + last;//+ ";InitString=SET TEMPORARY OPTION connection_authentication=Company=Cochin Computing Studio & Research Centre Pvt. Ltd.;Application = CC Banking;Signature = 000fa55157edb8e14d818eb4fe3db41447146f1571g0accaec7105cfb6088d748e067d2213c0bea9e45";
            try
            {
                _conn = new OdbcConnection(_con_string);
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                _conn.Open();
            }
            catch (Exception ex)
            {
                //Track_obj.Text_Tracker(Convert.ToString(ex));
            }
            return _conn;
        }

        public void db_connection_close(OdbcConnection con)
        {
            con.Close();
        }
    }

    class Root_Sen
    {
        static readonly object _object = new object();
        public static OdbcConnection _conn;
        connection1 con_obj = new connection1();
        ErrorTracker Track_obj = new ErrorTracker();
        private string ret_val = null;
        public void db_InsertUpdateDelete_Operations(string _sql_string)
        {

            try
            {
                lock (_object)
                {
                    //db_Connection_open();
                    _conn = con_obj.db_Connection_open();
                    OdbcCommand _command_obj = new OdbcCommand(_sql_string, _conn);
                    _command_obj.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }
            // db_Connection_close();
            _conn.Close();
        }
        public string db_select_operation(string _sql_string, string return_param)
        {
            string ret_val = null;
            OdbcDataReader new_reader;
            try
            {


               _conn= con_obj.db_Connection_open();
                
                if (_conn.State == ConnectionState.Open && (_conn.State != ConnectionState.Connecting || _conn.State != ConnectionState.Executing || _conn.State != ConnectionState.Fetching))
                {
                    OdbcCommand _command_obj = new OdbcCommand(_sql_string, _conn);

                    using (new_reader = _command_obj.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        lock (_object)
                        {

                            if (new_reader.Read())
                            {
                                ret_val = Convert.ToString(new_reader[return_param]);
                            }
                        }

                    }

                    return ret_val;
                }
                else
                    return null;

            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex) + return_param);
                return null;
            }

            //_conn.Close();
            //return ret_val;
        }


        public int Get_Max_Id(string as_tablename, int ai_ids_reqd)
        {
            int max_id = 0;
            try
            {
                string ss = "select cc.sf_get_max_id('" + as_tablename + "','" + ai_ids_reqd + "') max ";
                max_id = Convert.ToInt16(db_select_operation(ss, "max"));

            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }
            return max_id;
        }
        public DataTable db_select_operation_table(string _sql_string)
        {
            DataTable DT_Obj = new DataTable();
            OdbcDataAdapter DA;
            try
            {
                lock (_object)
                {
                    con_obj.db_Connection_open();
                    _conn = connection._conn;
                    if (_conn.State == ConnectionState.Open && (_conn.State != ConnectionState.Connecting || _conn.State != ConnectionState.Executing || _conn.State != ConnectionState.Fetching))
                    {
                        //db_Connection_open();
                        OdbcCommand _command_obj = new OdbcCommand(_sql_string, _conn);
                        using (DA = new OdbcDataAdapter(_command_obj))
                        {
                            _conn.Close();
                            DA.Fill(DT_Obj);
                        }                       
                    }
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }
            return DT_Obj;

        }


    }




}
