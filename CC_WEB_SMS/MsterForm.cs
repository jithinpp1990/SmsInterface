using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Timers;


namespace CC_WEB_SMS
{
    public partial class SMS_Server : Form
    {

        //private static System.Timers.Timer PrimaryTimer=new System.Timers.Timer();
        private static System.Timers.Timer Msg_genTimer = new System.Timers.Timer();
        private static System.Timers.Timer Msg_sendTimer = new System.Timers.Timer();
        private static System.Timers.Timer refresh = new System.Timers.Timer();
        //private static System.Windows.Forms.Timer PrimaryTimer = new System.Windows.Forms.Timer();
        private static System.Timers.Timer TimeTimer=new System.Timers.Timer();
        private static System.Timers.Timer SecondaryTimer=new System.Timers.Timer();
        //MTier.OnLoad_Defaults OnLoad_obj = new MTier.OnLoad_Defaults();
        MTier.MessageGenerator Mgen_obj = new MTier.MessageGenerator();
        MTier.Message_sender send_msg_obj = new MTier.Message_sender();
        MTier.GridLoad G_load_obj = new MTier.GridLoad();
        ErrorTracker Track_obj = new ErrorTracker();
        public Int16 messagePriority=0;
        public int priority;
        public SMS_Server()
        {
            InitializeComponent();
        }

        private void SMS_Server_Load(object sender, EventArgs e)
        {
            try
            {
                priority = 2;
                MTier.RegLoader.getregistry();
                Track_obj.Directory_Tracker();
                LoadDefaults.connect_to_db();
                LoadDefaults.get_prameter_provider_values();
                ///LoadDefaults.connect_to_db();
                SMS_History();

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////Connection STATUS//////////////////////////////////////////////////////////////////////////////////////////////
                if (WebRequestTest() == true)
                {
                    pictureBox1.Visible = true;
                    pictureBox2.Visible = false;
                    label3.Text = "Connected To Internet";
                }
                else
                {
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = true;
                    label3.Text = "Not Connected To Internet";
                }
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }

                       
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (LoadDefaults.ProviderName == "gup shup")
            {
                MessageBox.Show("This Service Currently Not Available");
            }
        }
        private void GENTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                Mgen_obj.Generator(priority);
                priority = 1;  
               
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }

        }
        private void SENDTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                
                
                send_msg_obj.MessageSender();
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }

        }
        private void REFRESHTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                //LoadData();
               // SMS_History();
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }

        }       
        private void start_stop_Click(object sender, EventArgs e)
        {
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////Timer Init//////////////////////////////////////////////////////////////////////////////////////////////////
            try
            {

                if (start_stop.Text == "Start Server")
                {
                    priority = 2;
                    ////////////////Message generating timer/////////////////////////////////////////////////////////////////
                    Msg_genTimer.Interval = LoadDefaults.ProcessInterval;
                    Msg_genTimer.Elapsed += new ElapsedEventHandler(GENTimedEvent);
                    Msg_genTimer.Enabled = true;

                    ///////////////message sending timer/////////////////////////////////////////////////////////////////////
                    Msg_sendTimer.Interval = LoadDefaults.ProcessInterval + 7;
                    Msg_sendTimer.Elapsed += new ElapsedEventHandler(SENDTimedEvent);
                    Msg_sendTimer.Enabled = true;

                    //////////////Grid refreshing timer/////////////////////////////////////////////////////////////////////
                    //refresh.Interval = 60000;
                    //refresh.Elapsed += new ElapsedEventHandler(REFRESHTimedEvent);
                    //refresh.Enabled = true;

                    start_stop.Text = "Stop Server";
                    start_stop.ForeColor = System.Drawing.Color.Red;
                }
                else if (start_stop.Text == "Stop Server")
                {
                    start_stop.ForeColor = System.Drawing.Color.Black;
                    Msg_genTimer.Enabled = false;
                    Msg_genTimer.Stop();

                    Msg_sendTimer.Enabled = false;
                    Msg_sendTimer.Stop();
                    start_stop.Text = "Start Server";
                    MessageBox.Show(" SMS SERVER IS STOPPED ", "CC WEB SMS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                Track_obj.Text_Tracker(Convert.ToString(ex));
            }

        }
       
        public bool WebRequestTest()
        {
            string url = "http://www.google.com";
            try
            {
                System.Net.WebRequest myRequest = System.Net.WebRequest.Create(url);
                System.Net.WebResponse myResponse = myRequest.GetResponse();
                return true;
            }
            catch (System.Net.WebException)
            {
                return false;
            }

        }

        public void SMS_History()
        {

            DataTable DT_obj1 = new DataTable();
            DT_obj1 = (DataTable)G_load_obj.sms_history();
            dataGridView1.DataSource = DT_obj1;
            
        }
        private void button3_Click(object sender, EventArgs e)
        {            
            SMS_History();
        }

        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            // Confirm user wants to close
            switch (MessageBox.Show(this, "Are you sure you want to close?", "Closing", MessageBoxButtons.YesNo,MessageBoxIcon.Question))
            {
                case DialogResult.No:
                    e.Cancel = true;
                    break;
                default:
                    break;
            }
        }
        void LoadData()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(InnerLoadData));
        }
        void InnerLoadData()
        {
            SMS_History();
        }

        private void resetToDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            priority = 2;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            MTier.MessageBalance Balance_Obj = new MTier.MessageBalance();
            string providerName = null;
            providerName = LoadDefaults.ProviderName;
            long Balance=0;
            switch (providerName)
            {
                case "green ads sap":
                   Balance= Balance_Obj.get_balance_greenads_sap(LoadDefaults.ProviderUID,LoadDefaults.ProviderPassword);
                   MessageBox.Show(this, Convert.ToString(Balance), "Balance", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
                case "gup shup":
                    MessageBox.Show(this, "This Service Not Available For GUPSHUP","Balance",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
                case "tech soul":
                    MessageBox.Show(this, "This Service Not Available For TechSoul", "Balance", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
                case "green ads":
                    Balance = Balance_Obj.get_balance_greenads(LoadDefaults.ProviderUID, LoadDefaults.ProviderPassword,"T");
                    MessageBox.Show(this, Convert.ToString(Balance), "Balance", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
                default :
                    MessageBox.Show(this, "Error", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
            }


           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show(this, "Are you sure You Want to Generate SMS of "+dateTimePicker3.Text, "!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                case DialogResult.Yes:
                    Mgen_obj.Generator(0,dateTimePicker3.Text);
                    break;
                default:
                    break;
            }
        }
    }
}
