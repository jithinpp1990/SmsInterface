using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Configuration;
using System.Data.Odbc;
using System.Data;
using System.IO;
using Microsoft.Win32;
using System.Globalization;

namespace CC_WEB_SMS.MTier
{
    class MessageGenerator
    {

        private string dsn;
        public string _con_string;
        private string ret_val = null;
        public static OdbcConnection _conn = new OdbcConnection();
        ErrorTracker Track_obj = new ErrorTracker();
        DBClass.Root_Gen root_obj_in = new DBClass.Root_Gen();
        private static readonly object ReaderLock = new object();
        public string bussinessDate;
        public void Generator(int priority, string bdate = null)
        {
            string ret;
            try
            {
                if (bdate == null)
                {


                    string sql1 = "select dateformat(max(bank_date),'yyyy-MM-dd') BussinessDay from cc.day_details where closed_by is null";
                    //bussinessDate = root_obj_in.db_select_operation("select dateformat(max(bank_date),'yyyy-MM-dd') BussinessDay from cc.day_details where closed_by is null", "BussinessDay");
                    bussinessDate = Connections(sql1, "BussinessDay");
                    if (bussinessDate != null || bussinessDate != "")
                    {
                        
                        string sql2 = "select sf_update_sms('" + bussinessDate.Substring(0, 10) + "','" + priority + "') sf_update_sms";
                        ret = Connections(sql2, "sf_update_sms");
                        //ret = root_obj_in.db_select_operation(sql, "sf_update_sms");
                    }
                    else
                    {
                        Console.WriteLine("null");
                    }
                }
                else
                {
                    bdate = bdate.Substring(6, 4) + "-" + bdate.Substring(3, 2) + "-" + bdate.Substring(0,2);
                    string sql2 = "select sf_update_sms('" + bdate + "','" + priority + "') sf_update_sms";
                    ret = Connections(sql2, "sf_update_sms");
                }
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }
        }
        public string Connections(string _sql_string, string return_param)
        {
            dsn = RegLoader.Dsn;
            string first = "#websms*321*#";
            string middle = "busopasnosty#";
            string last = "rotartsinimda123*#";
            _con_string = "DSN=" + dsn + ";uid=CCBankingSMS;pwd=" + first + middle + last;
            _conn = new OdbcConnection(_con_string);
            try
            {                
                //if (_conn.State == ConnectionState.Open)
                //    _conn.Close();
                //_conn.Open();
                //OdbcCommand _command_obj = new OdbcCommand(_sql_string, _conn);
                using (var _command_obj =new OdbcCommand(_sql_string, _conn))
                {
                    lock (ReaderLock)
                    {
                        if (_conn.State == ConnectionState.Open)
                            _conn.Close();
                        _conn.Open();
                        using (var new_reader = _command_obj.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (new_reader.Read())
                            {
                                ret_val = Convert.ToString(new_reader[return_param]);
                            }
                        }
                    }
                }
                return ret_val;
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }


    }
}
