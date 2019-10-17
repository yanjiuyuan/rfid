using DingTalkServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace DingTalkServer
{
    public partial class DingTalkManager
    {
        public async Task<string> GetUserDetail(string userId)
        {
            _client.QueryString.Add("userid", userId);
            var url = _addressConfig.GetUserDetailUrl;
            var result = await _client.Get(url);
            return result;

        }
        public async Task<string> GetDepartmentUserList(string dptId)
        {
            _client.QueryString.Add("department_id", dptId);
            _client.QueryString.Add("order", "entry_desc");
            var url = _addressConfig.GetDepartmentUserListUrl;
            var result = await _client.Get(url);
            return result;
        }

        public  Task<string> GetDepartmentUserDetailList(string dptId)
        {
            _client.QueryString.Add("department_id", dptId);
            var url = _addressConfig.GetDepartmentUserDetailListUrl;
            var result =  _client.Get(url);
            return result;
        }

        public Task<string> GetChildDeptByDeptId(string dptId)
        {
            _client.QueryString.Add("id", dptId);
            var url = _addressConfig.GetChildDeptByDeptIdUrl;
            var result = _client.Get(url);
            return result;
        }

        

        public  Task<string> CreateUser(AddUserRequestModel user)
        {
            var url = _addressConfig.CreateUserUrl;
            var result =  _client.UploadModel(url,user);
            return result;
        }

        public Task<string> UpdateUser(AddUserRequestModel user)
        {
            var url = _addressConfig.UpdateUserUrl;
            var result = _client.UploadModel(url, user);
            return result;

        }
        public Task<string> DeleteUser(string userId)
        {
            var url = _addressConfig.DeleteUserUrl;
            _client.QueryString.Add("userid", userId);
            var result = _client.Get(url);
            return result;
        }

        public Task<string> BatchDeleteUser(BatchDeleteUserModel deleteModel)
        {
            var url = _addressConfig.BatchDeleteUserUrl;
            var result = _client.UploadModel(url,deleteModel);
            return result;

        }
    }
}