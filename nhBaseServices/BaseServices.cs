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

            //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Service Start.");
            SW("Service Start.");
            try
            {
                Thread.Sleep(30000);        //30秒等待
                MainEvent();
                //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "MainEvent Success.");
                SW("MainEvent Success");
            }
            catch (Exception ex)
            {
                //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + (ex.Source + "。" + ex.Message));
                SW(ex.Source + "。" + ex.Message);
            }
            AutoLog = false;
            oTimer_Get.Enabled = true;
            oTimer_Get.Interval = 1800000;      //30分钟轮询一次
            oTimer_Get.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Main Start.");
            SW("Main Start.");
            oTimer_Get.Enabled = false;
            try
            {
                MainEvent();
                //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "MainEvent Success.");
                SW("MainEvent Success.");
            }
            catch (Exception ex)
            {
                //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + (ex.Source + "。" + ex.Message));
                SW(ex.Source + "。" + ex.Message);
            }
            oTimer_Get.Enabled = true;
            //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Main End.");
            SW("Main End.");
        }

        private void MainEvent()
        {
            //读取配置文件config.xml
            string LinkString = "";
            string pKey = "";
            string strLocalAdd = "C:\\Config.xml";
            string TimeStamp = DateTime.Now.ToString("yyMMdd").ToString();

            if (File.Exists(strLocalAdd))
            {
                try
                {
                    XmlDocument xmlCon = new XmlDocument();
                    xmlCon.Load(strLocalAdd);
                    XmlNode xnCon = xmlCon.SelectSingleNode("Config");
                    LinkString = xnCon.SelectSingleNode("LinkString").InnerText;
                    pKey = xnCon.SelectSingleNode("pKey").InnerText;
                    //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Read Config Success.");
                    SW("Read Config Success.");
                }
                catch
                {
                    LinkString = "Server=localhost;user id=root;password=;Database=chenkuserdb37;Port=3308;charset=utf8;";
                    //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Read Config Error.");
                    SW("Read Config Error.");
                }
            }
            else
            {
                LinkString = "Server=localhost;user id=root;password=;Database=chenkuserdb37;Port=3308;charset=utf8;";
                //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Read Config Error.");
                SW("Read Config Error.");
            }
            string strInSql = "update skt17 set skf204=0 where skf229!='" + TimeStamp + "' ";
            int intInSql = MySqlHelper.MySqlHelper.ExecuteSql(strInSql, LinkString);
            //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Update All hisData Success.");
            SW("Update All hisData Success.");
            EnDeCode.EnDeCode EDC = new EnDeCode.EnDeCode();
            string strSQL = "select skf97 from skt8 where skf104=1";
            DataSet DSSql = MySqlHelper.MySqlHelper.Query(strSQL, LinkString);
            if (DSSql.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < DSSql.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        string priKey = "";
                        //string strInSql = "update skt17 set skf204=0 where skf203='" + DSSql.Tables[0].Rows[i][0].ToString() + "' and skf229!='" + TimeStamp + "' ";
                        //int intInSql = MySqlHelper.MySqlHelper.ExecuteSql(strInSql, LinkString);
                        //EnDeCode.EnDeCode EDC = new EnDeCode.EnDeCode();
                        priKey = EDC.DesEncrypt(EDC.GetHexString(16), pKey);
                        strInSql = "select * from skt17 where skf204=1 and skf203='" + DSSql.Tables[0].Rows[i][0].ToString() + "' and skf229='" + TimeStamp + "' ";
                        DataSet dsInSql = MySqlHelper.MySqlHelper.Query(strInSql, LinkString);
                        if (dsInSql.Tables[0].Rows.Count <= 0)
                        {
                            strInSql = "insert into skt17(skf203,skf204,skf205,skf206,skf208,skf229) ";
                            strInSql = strInSql + "value('" + DSSql.Tables[0].Rows[i][0].ToString() + "',1,'" + priKey + "','" + System.DateTime.Now.ToString() + "','System Service','" + TimeStamp + "') ";
                            intInSql = MySqlHelper.MySqlHelper.ExecuteSql(strInSql, LinkString);
                            //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:: ") + DSSql.Tables[0].Rows[i][0].ToString() + "::Insert new data Success.");
                            SW(DSSql.Tables[0].Rows[i][0].ToString() + "::Insert new data Success.");
                        }
                        else
                        {
                            //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:: ") + DSSql.Tables[0].Rows[i][0].ToString() + "::No date insert.");
                            SW(DSSql.Tables[0].Rows[i][0].ToString() + "::No date insert.");
                        }

                    }
                    catch
                    {
                        //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Insert new data Error.");
                        SW("Insert new data Error.");
                    }
                }
                if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 3)
                {
                    string strinSQL = "select * from skt6 where skf91=1 ";
                    DataSet dsinSQL = MySqlHelper.MySqlHelper.Query(strinSQL, LinkString);
                    if (dsinSQL.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsinSQL.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                string strExSQL = "insert into skt7(skf73, skf74, skf75, skf78, skf79, skf82, skf199, skf200,skf80,skf81)";
                                strExSQL = strExSQL + " value('" + dsinSQL.Tables[0].Rows[i]["skf64"].ToString() + "', '" + dsinSQL.Tables[0].Rows[i]["skf63"].ToString() + "', '" + dsinSQL.Tables[0].Rows[i]["skf63"].ToString() + "', '" + dsinSQL.Tables[0].Rows[i]["skf66"].ToString() + "', '" + int.Parse(dsinSQL.Tables[0].Rows[i]["skf66"].ToString()) + int.Parse(dsinSQL.Tables[0].Rows[i]["skf67"].ToString()) + "', '" + System.DateTime.Now.ToString() + "', '" + "System Auto" + "', 0,'" + dsinSQL.Tables[0].Rows[i]["skf67"].ToString() + "',0) ";
                                int intExSQL = MySqlHelper.MySqlHelper.ExecuteSql(strExSQL, LinkString);
                                //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Insert JF History data Success.");
                                SW("Insert JF History data Success.");
                            }
                            catch
                            {
                                //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Insert JF History data Error.");
                                SW("Insert JF History data Error.");
                            }
                        }
                    }
                    strinSQL = "update skt6 set skf66=skf66+skf67,skf67=0 where skf91=1";
                    int intinSQL = MySqlHelper.MySqlHelper.ExecuteSql(strinSQL, LinkString);
                }
            }
        }

        protected override void OnStop()
        {
            //using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt", true))
            //{
            //    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Service Stop.");
            //}
            SW("Service Stop.");
        }

        private void SW(string strT)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + strT);
            }
        }
    }
}
