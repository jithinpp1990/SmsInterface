using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;

namespace CC_WEB_SMS
{
    class ErrorTracker
    {
        /// <summary>
        /// log path
        /// </summary>
        private string path_file = @"c:\sms_log\sms_log.txt";
        private string path_directory = @"c:\sms_log";
       
      
        /// <summary>
        /// ERROR LOG TRACKER
        /// </summary>

        public void Directory_Tracker()
        {
            try
            {
                if (!Directory.Exists(path_directory))
                {
                    Directory.CreateDirectory(path_directory);

                }

                if (!File.Exists(path_file))
                {

                    using (StreamWriter sw = File.CreateText(path_file))
                    {
                        sw.WriteLine(Convert.ToString(System.DateTime.Now));

                    }
                }

                FileInfo fi = new FileInfo(path_file);
                var size = fi.Length / 1024;
                var path1 = fi.Directory.FullName + System.DateTime.Today.ToString("dd_MM_yyyy");
                if (size > 2050)
                {
                    System.IO.File.Move(path_file, path_file + System.DateTime.Today.ToString("dd_MM_yyyy_HH_mm"));
                }
            }
            catch
            {

            }

        }
        public void Text_Tracker(string message)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(path_file))
                {
                    sw.WriteLine(Convert.ToString(System.DateTime.Now) + "-------" + message);
                }
            }
            catch
            {
            }

        }

    }
}
