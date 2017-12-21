using Common.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness
{
    public class LoginServer
    {
        public static int ChekLogin(string strUserName, string strPassword)
        {
            string strSql = string.Format("select username,password from hzsk.userInfo where username='{0}' and password = '{1}' and  status='1'",
                strUserName, strPassword);
            //int iResult= MySqlHelper.ExecuteSql(strSql);
            DataSet dataset = MySqlHelper.GetDataSet(strSql);
            int iResult = dataset.Tables[0].Rows.Count;
            return iResult;
        }


        /// <summary>
        /// role:用户角色
        //用户角色影响用户权限，用户权限大概有以下几种
        //1）浏览平台基础信息，平台要展示给普通用户的一般信息。如产品介绍信息（普通用户）
        //2）浏览所有设备的运行状态，查看报表。（厂管理）
        //3）查看与操作所管理设备（设备管理）-》需要添加字段映射用户和设备
        //4）查看与操作所有设备（管理员）
        /// </summary>
        /// <param name="strUserName"></param>
        /// <returns></returns>
        public static int GetRole(string strUserName)
        {
            int iRole = 0;
            string strSql = string.Format("select role from hzsk.userInfo where username='{0}'", strUserName);
            DataSet dataset = MySqlHelper.GetDataSet(strSql);
            iRole= Int32.TryParse(dataset.Tables[0].Rows[0]["role"].ToString(),out iRole) ?iRole:0;
            return iRole;
        }
    }
}
