using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Data;

namespace CC_WEB_SMS.MTier
{
    class GridLoad
    {
        DBClass.Queries Q_obj = new DBClass.Queries();
        DataTable DT_obj = new DataTable();
        public DataTable sms_history()
        {
            DT_obj = (DataTable)Q_obj.get_sms_history();
            return DT_obj;
        }
        
    }
}
