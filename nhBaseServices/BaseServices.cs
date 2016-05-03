using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Xml;
using System.IO;
using System.Web;
using System.Threading;

namespace nhBaseServices
{
    public partial class BaseServices : ServiceBase
    {
        static System.Timers.Timer oTimer_Get = new System.Timers.Timer();

        public BaseServices()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Service Start.");
                try
                {
                    Thread.Sleep(30000);
                    MainEvent();
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "MainEvent Success.");
                }
                catch (Exception ex)
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + (ex.Source + "。" + ex.Message));
                }
                AutoLog = false;
                oTimer_Get.Enabled = true;
                oTimer_Get.Interval = 3600000;
                oTimer_Get.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            }
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Main Start.");
                oTimer_Get.Enabled = false;
                try
                {
                    MainEvent();
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "MainEvent Success.");
                }
                catch (Exception ex)
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + (ex.Source + "。" + ex.Message));
                }
                oTimer_Get.Enabled = true;
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Main End.");
            }
        }

        private void MainEvent()
        {
            //读取配置文件config.xml
            string LinkString = "";
            string pKey = "";
            string strLocalAdd = "C:\\Config.xml";

            if (File.Exists(strLocalAdd))
            {
                try
                {
                    XmlDocument xmlCon = new XmlDocument();
                    xmlCon.Load(strLocalAdd);
                    XmlNode xnCon = xmlCon.SelectSingleNode("Config");
                    LinkString = xnCon.SelectSingleNode("LinkString").InnerText;
                    pKey = xnCon.SelectSingleNode("pKey").InnerText;
                }
                catch
                {
                    LinkString = "Server=localhost;user id=root;password=;Database=chenkuserdb37;Port=3308;charset=utf8;";
                }
            }
            else
            {
                LinkString = "Server=localhost;user id=root;password=;Database=chenkuserdb37;Port=3308;charset=utf8;";
            }

            string strSQL = "select skf97 from skt8 where skf104=1";
            DataSet DSSql = MySqlHelper.MySqlHelper.Query(strSQL, LinkString);
            if(DSSql.Tables[0].Rows.Count>0)
            {
                for(int i=0;i<DSSql.Tables[0].Rows.Count;i++)
                {
                    string priKey = "";
                    string TimeStamp = DateTime.Now.ToString("yyMMdd").ToString();
                    string strInSql = "update skt17 set skf204=0 where skf203='" + DSSql.Tables[0].Rows[i][0].ToString() + "' and skf229!='" + TimeStamp + "' ";
                    int intInSql = MySqlHelper.MySqlHelper.ExecuteSql(strInSql, LinkString);             
                    EnDeCode.EnDeCode EDC = new EnDeCode.EnDeCode();
                    priKey = EDC.DesEncrypt(EDC.GetHexString(16), pKey);

                    strInSql = "select * from skt17 where skf203='" + DSSql.Tables[0].Rows[i][0].ToString() + "' and skf229!='" + TimeStamp + "' ";
                    intInSql = MySqlHelper.MySqlHelper.ExecuteSql(strInSql, LinkString);
                    if(intInSql<=0)
                    {
                        strInSql = "insert into skt17(skf203,skf204,skf205,skf206,skf208,skf229) ";
                        strInSql = strInSql + "value('" + DSSql.Tables[0].Rows[i][0].ToString() + "',1,'" + priKey + "','" + System.DateTime.Now.ToString() + "','System Service','" + TimeStamp + "') ";
                        intInSql = MySqlHelper.MySqlHelper.ExecuteSql(strInSql, LinkString);
                    }
                }
            }
        }

        protected override void OnStop()
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Service Stop.");
            }
        }
    }
}
