using RMS.Common.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections;
using System.Web.Services;

namespace BootstrapDemo.Handler
{
    /// <summary>
    /// ServiceProcessHandler 的摘要说明
    /// </summary>
    public class ServiceProcessHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string pageSize = context.Request["pageSize"];
            string pageIndex = context.Request["pageIndex"];
            string name = context.Request["name"];
            string order = context.Request["order"];
            string limit = context.Request["limit"];
            string service_bill = context.Request["Service_bill"];
            string cust_name = context.Request["Cust_name"];
            pageSize = pageSize == null ? limit : pageSize;
            pageIndex = pageIndex == null ? "1" : pageIndex;
            //string strSql = GetSql(pageSize, pageIndex, name, order);
            DataTable dt = GetList("");
            var total = SQLHelper.GetSingle("SELECT COUNT(1) FROM bi_service_process"); 
            object obj = new { total = total, rows = dt };
            context.Response.Write(JsonConvert.SerializeObject(obj));
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// 获取列表
        /// </summary>
        public DataTable GetList(string condition)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select flow_id,service_bill,process_date,cust_name,tele_phone,product_name");
            strSql.Append(" from bi_service_process ");
            strSql.Append(" where 1>0 " + condition);
            DataTable dt = SQLHelper.ExecuteDt(strSql.ToString());
            return dt;
        }
        public string GetSql(string pageSize, string pageIndex, string name,string order)
        {
            string strSql = string.Empty;
            name = string.IsNullOrEmpty(name) ? "ORDER BY  flow_id DESC" : "ORDER BY " + name + " " + order;
            return strSql = @"SELECT TOP (" + int.Parse(pageSize) + ") * FROM    ( SELECT TOP ( " + int.Parse(pageSize) + " * " + int.Parse(pageIndex) + " ) flow_id,service_bill,process_date,cust_name,tele_phone,product_name FROM  bi_service_process " + name + ") AS product  ORDER BY  flow_id asc";
        }

    }

    public class TBToList<T> where T : new()
    {
        /// <summary>
        /// 获取列名集合
        /// </summary>
        private IList<string> GetColumnNames(DataColumnCollection dcc)
        {
            IList<string> list = new List<string>();
            foreach (DataColumn dc in dcc)
            {
                list.Add(dc.ColumnName);
            }
            return list;
        }

        /// <summary>
        ///属性名称和类型名的键值对集合
        /// </summary>
        private Hashtable GetColumnType(DataColumnCollection dcc)
        {
            if (dcc == null || dcc.Count == 0)
            {
                return null;
            }
            IList<string> colNameList = GetColumnNames(dcc);

            Type t = typeof(T);
            PropertyInfo[] properties = t.GetProperties();
            Hashtable hashtable = new Hashtable();
            int i = 0;
            foreach (PropertyInfo p in properties)
            {
                foreach (string col in colNameList)
                {
                    if (col.ToLower().Contains(p.Name.ToLower()))
                    {
                        hashtable.Add(col, p.PropertyType.ToString() + i++);
                    }
                }
            }

            return hashtable;
        }

        /// <summary>
        /// DataTable转换成IList
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public IList<T> ToList(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            PropertyInfo[] properties = typeof(T).GetProperties();//获取实体类型的属性集合
            Hashtable hh = GetColumnType(dt.Columns);//属性名称和类型名的键值对集合
            IList<string> colNames = GetColumnNames(hh);//按照属性顺序的列名集合
            List<T> list = new List<T>();
            T model = default(T);
            foreach (DataRow dr in dt.Rows)
            {
                model = new T();//创建实体
                int i = 0;
                foreach (PropertyInfo p in properties)
                {
                    if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(model, dr[colNames[i++]], null);
                    }
                    else if (p.PropertyType == typeof(int))
                    {
                        p.SetValue(model, int.Parse(dr[colNames[i++]].ToString()), null);
                    }
                    else if (p.PropertyType == typeof(DateTime))
                    {
                        p.SetValue(model, DateTime.Parse(dr[colNames[i++]].ToString()), null);
                    }
                    else if (p.PropertyType == typeof(float))
                    {
                        p.SetValue(model, float.Parse(dr[colNames[i++]].ToString()), null);
                    }
                    else if (p.PropertyType == typeof(double))
                    {
                        p.SetValue(model, double.Parse(dr[colNames[i++]].ToString()), null);
                    }
                }

                list.Add(model);
            }

            return list;
        }

        /// <summary>
        /// 按照属性顺序的列名集合
        /// </summary>
        private IList<string> GetColumnNames(Hashtable hh)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();//获取实体类型的属性集合
            IList<string> ilist = new List<string>();
            int i = 0;
            foreach (PropertyInfo p in properties)
            {
                ilist.Add(GetKey(p.PropertyType.ToString() + i++, hh));
            }
            return ilist;
        }

        /// <summary>
        /// 根据Value查找Key
        /// </summary>
        private string GetKey(string val, Hashtable tb)
        {
            foreach (DictionaryEntry de in tb)
            {
                if (de.Value.ToString() == val)
                {
                    return de.Key.ToString();
                }
            }
            return null;
        }

    }
    /// <summary>
    /// 售后服务受理单实体
    /// </summary>
    public class T_Service_Process
    {
        public int flow_id { get; set; }
        public string service_bill { get; set; }
        public string process_date { get; set; }
        public string cust_name { get; set; }
        public string tele_phone { get; set; }
        public string buy_date { get; set; }
        public string product_name { get; set; }
        public string seq_no { get; set; }
        public string order_bill { get; set; }
        public string buy_id { get; set; }
        public string buy_way { get; set; }
        public string fault_descri { get; set; }
        public string repair_status { get; set; }
        public string process_case { get; set; }
        public string new_sn { get; set; }
        public string process_memo { get; set; }
        public string oper_name { get; set; }
        public DateTime oper_date { get; set; }
        public string bill_flag { get; set; }
        public string aduit_man { get; set; }
        public DateTime aduit_date { get; set; }
        public string reason_memo { get; set; }
    }
}
