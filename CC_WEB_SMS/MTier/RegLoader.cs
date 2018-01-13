using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Microsoft.Win32;

namespace CC_WEB_SMS.MTier
{
    class RegLoader
    {
        public static string dsn;
        public static string Dsn
        {
            set { dsn = value; }
            get { return dsn; }
        }

        public static void getregistry()
        {
            string SoftwareKey = "SOFTWARE\\CC\\CC Banking SMS";
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(SoftwareKey))
                {

                    Dsn = Convert.ToString(key.GetValue("DSN"));
                }
                if (Dsn == null)
                    Dsn = "CCBank";

            }
            catch (Exception ex)
            {

                Dsn = "CCBank";

            }
        }
    }
}
