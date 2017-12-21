using Common.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Register
{
    public class RegisterServer
    {
        /// <summary>
        /// 判断用户名是否存在
        /// </summary>
        /// <param name="strUserName">用户名</param>
        /// <returns></returns>
        public static bool CheckUserName(string strUserName)
        {
            string strSql=string.Format("select * from  hzsk.userinfo where username='{0}'", strUserName);
            DataSet dataset = MySqlHelper.GetDataSet(strSql);
            int iResult = dataset.Tables[0].Rows.Count;
            if (iResult > 0) 
            {
                return true; //用户名存在
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 注册用户信息
        /// </summary>
        /// <param name="strUserName"></param>
        /// <param name="strRealName"></param>
        /// <param name="strphoneNumber"></param>
        /// <param name="strPassword"></param>
        /// <param name="strRegisterTime"></param>
        /// <param name="iStatus"></param>
        /// <param name="strAddress"></param>
        /// <param name="iRole"></param>
        /// <param name="strProvince"></param>
        /// <param name="strCity"></param>
        /// <param name="strTelephone"></param>
        /// <param name="strOtherContact"></param>
        /// <param name="strFax"></param>
        /// <param name="iIsActive"></param>
        /// <param name="strLastLoginIp"></param>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        public static bool  Register(string strUserName, string strRealName,
            string strphoneNumber, string strPassword, string strRegisterTime,string strLastLoginTime,
            int iStatus, string strAddress, int iRole, string strProvince, string strCity,
            string strTelephone, string strOtherContact, string strFax, int iIsActive,
            string strLastLoginIp, string strEmail)
        {
            string strSql = string.Format("insert into hzsk.userinfo set " +
                "userName='{0}',realName='{1}',phoneNumber='{2},password='{3}'," +
                "registertime='{4}',lastLoginTime='{5}',status='{6}',address='{7}'," +
                "role='{8}',province='{9}',city='{10}',telephone='{11}',otherContact='{12}'," +
                "fax='fax',isActive='{14}',lastLoginIp='{15}',email='{16}'",  strUserName,  strRealName,
             strphoneNumber,  strPassword,  strRegisterTime, strLastLoginTime,
             iStatus,  strAddress,  iRole,  strProvince,  strCity,
             strTelephone,  strOtherContact,  strFax,  iIsActive,
             strLastLoginIp,  strEmail);

            bool bResult = MySqlHelper.ExecuteSql(strSql)==1?true:false;
            return bResult;
        }
    }
}
