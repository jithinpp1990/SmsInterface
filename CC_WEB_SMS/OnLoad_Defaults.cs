using System;
using System.Collections.Generic;
using System.Text;
//using System.Threading.Tasks;
using System.Collections;



namespace CC_WEB_SMS.MTier
{
   class OnLoad_Defaults
    {
        

/// <summary>
/// parameter
/// </summary>
        public static  int provider_id;
        public static  int ProviderId
        {
            set { provider_id = value; }
            get { return provider_id; }
        }
        public static  string sender_name;
        public static  string SenderName
        {
            set { sender_name = value; }
            get { return sender_name; }
        }
        public static  int max_attempt;
        public static  int MaxAttempt
        {
            set { max_attempt = value; }
            get { return max_attempt; }
        }
        public static  int process_interval;
        public static  int ProcessInterval
        {
            set { process_interval = value; }
            get { return process_interval; }
        }
        public static  int delivery_delay;
        public static  int DeliveryDelay
        {
            set { delivery_delay = value; }
            get { return delivery_delay; }
        }
        public static  string provider_name;
        public static  string ProviderName
        {
            set { provider_name = value; }
            get { return provider_name; }
        }
        public static  string provider_key;
        public static  string ProviderKey
        {
            set { provider_key = value; }
            get { return provider_key; }
        }
        public static  string provider_uid;
        public static  string ProviderUID
        {
            set { provider_uid = value; }
            get { return provider_uid; }
        }
        public static  string provider_password;
        public static  string ProviderPassword
        {
            set { provider_password = value; }
            get { return provider_password; }
        }

/// <summary>
/// load parameter values
/// </summary>
    public static void get_prameter_provider_values()
    {
        DBClass.Queries Q_Obj = new DBClass.Queries();
        ProviderId = Q_Obj.get_provider_id();
        SenderName = Q_Obj.get_sender_name();
        MaxAttempt = Q_Obj.get_max_attempt();
        ProcessInterval = Q_Obj.get_process_interval();
        DeliveryDelay = Q_Obj.get_delivery_delay();
        Q_Obj.get_provider_det(ProviderId);
        ProviderName = Q_Obj.Provider_name;
        ProviderKey = Q_Obj.provider_key;
        ProviderUID = Q_Obj.provider_uid;
        ProviderPassword = Q_Obj.provider_password;
    }
    public static void connect_to_db()
    {
        DBClass.connection con_Obj = new DBClass.connection();
        con_Obj.db_Connection_open();
    }




      
    }
}
