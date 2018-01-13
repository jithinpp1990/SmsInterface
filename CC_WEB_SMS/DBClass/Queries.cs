using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Data;

namespace CC_WEB_SMS.DBClass
{
    class Queries
    {
        public int return_integer=0;
        public string return_string=null;
        //public string sql=null;
        public string Provider_name;
        public string provider_key;
        public string provider_uid;
        public string provider_password;
        ErrorTracker trcker_obj = new ErrorTracker();
        Root_Gen root_obj_out = new Root_Gen();
        public int get_provider_id()
        {
            string ret;
            string sql1 = "select parameter_value from cc.cc_parameters where parameter_name='Outgoing SMS Provider id'";
            ret = root_obj_out.db_select_operation(sql1, "parameter_value");
            if (ret != null)
                return_integer = Convert.ToInt16(ret);
            else
                return_integer = 0;
            return return_integer;
        }
        public string get_sender_name()
        {
            string ret;
            string sql2 = "select parameter_value from cc.cc_parameters where parameter_name='SMS Sender Name'";
            ret=root_obj_out.db_select_operation(sql2, "parameter_value");
            if (ret != null)
                return_string = ret;
            else
                return_string = null;
            return return_string;
        }
        public int get_max_attempt()
        {
            string ret;
            string sql3 = "select parameter_value from cc.cc_parameters where parameter_name='Maximum Attempt For outgoing SMS'";
            ret = root_obj_out.db_select_operation(sql3, "parameter_value");
            if (ret != null)
                return_integer = Convert.ToInt16(ret);
            else
                return_integer = 0;
            return return_integer;
        }
        public int get_process_interval()
        {
            string ret;
            string sql4 = "select parameter_value from cc.cc_parameters where parameter_name='SMS Process Interval'";
            ret = root_obj_out.db_select_operation(sql4, "parameter_value");
            if (ret != null)
                return_integer = Convert.ToInt32(ret);
            else
                return_integer = 0;
            return return_integer;
        }
        public int get_delivery_delay()
        {
            string ret;
            string sql5 = "select parameter_value from cc.cc_parameters where parameter_name='SMS Delivery Delayed time'";
            ret = root_obj_out.db_select_operation(sql5, "parameter_value");
            if (ret != null)
                return_integer = Convert.ToInt16(ret);
            else
                return_integer = 0;
            return return_integer;
        }
        public string get_bussiness_date()
        {
            Root_Gen root_obj_in = new Root_Gen();
            string ret;
            string sql6 = "select dateformat(max(bank_date),'yyyy-MM-dd') BussinessDay from cc.day_details where closed_by is null";
            ret = root_obj_in.db_select_operation(sql6, "BussinessDay");
            if (ret != null)
                return_string = ret.ToString();
            else
                return_string=null;
            return return_string;
        }
        public void select_sf_update_sms_fn(string date, int priority)
        {
            Root_Gen root_obj_in = new Root_Gen();
            string ret;
            string sql7 = "select sf_update_sms('" + date + "','" + priority + "') sf_update_sms";
            ret = root_obj_in.db_select_operation(sql7, "sf_update_sms");
            return_integer = Convert.ToInt16(ret);
        }
        public void update_sms_return_id(int sms_inout_id,string sms_return_id)
        {
            Root_Gen root_obj_in = new Root_Gen();
            string sql8 = "update cc.sms_inout set delivery_status='D',message_id='" + sms_return_id + "',status='N' where sms_inout_id='"+sms_inout_id+"'";
            root_obj_in.db_InsertUpdateDelete_Operations(sql8);
        }
        public void get_provider_det(int provider_id)
        {
            try
            {
                DataTable DT_Obj = new DataTable();
                string sql9 = "select Provider_name,provider_key,provider_uid,provider_password from cc.sms_provider where provider_id=" + provider_id;
                DT_Obj = (DataTable)root_obj_out.db_select_operation_table(sql9);
                Provider_name =Convert.ToString(DT_Obj.Rows[0]["Provider_name"]);
                provider_key = Convert.ToString(DT_Obj.Rows[0]["Provider_key"]);
                provider_uid = Convert.ToString(DT_Obj.Rows[0]["Provider_uid"]);
                provider_password = Convert.ToString(DT_Obj.Rows[0]["Provider_password"]);
            }
            catch(Exception ex)
            {
                trcker_obj.Text_Tracker(Convert.ToString(ex));
            }

        }
        public DataTable get_messages_to_send()
        {
            Root_Gen root_obj_in = new Root_Gen();
            try
            {
                DataTable DT_Obj = new DataTable();
                string sql10 = "select mobile_no,messages,sms_inout_id,status from cc.sms_inout where in_out='O' and status ='Y' ";
                DT_Obj = (DataTable)root_obj_in.db_select_operation_table(sql10);
                return DT_Obj;
            }
            catch(Exception ex)
            {
                trcker_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
        public DataTable get_sms_history()
        {
            Root_Gen root_obj_in = new Root_Gen();
            try
            {
                DataTable DT_Obj = new DataTable();
                string sql11 = "select 0+number(*) SlNo,mobile_no Mobile_Number,messages Messages,in_out InOut,status Status from cc.sms_inout where in_out='O' order by sms_inout_id desc";
                DT_Obj = (DataTable)root_obj_in.db_select_operation_smsHistory(sql11);
                return DT_Obj;
            }
            catch(Exception ex)
            {
                trcker_obj.Text_Tracker(Convert.ToString(ex));
                return null;
            }
        }
    }
}
