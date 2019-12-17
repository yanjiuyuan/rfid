using DingTalk.DingTalkHelper;
using DingTalk.Models.ServerModels;
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
        DingTalkServerAddressConfig _addressConfig = DingTalkServerAddressConfig.GetInstance();
        public DingTalkConfig DTConfig { get; set; } = new DingTalkConfig();

        HttpsClient _client;
        public DingTalkManager()
        {
            _client = new HttpsClient();
            _client.QueryString.Add("access_token", DDApiService.Instance.GetAccessToken());
        }

        public async Task<string> GetAccessToken()
        {
            if (DTConfig.type == "1")
            {
                //新应用免登新参数
                _client.QueryString.Add("appkey", DTConfig.PcAppKey);
                _client.QueryString.Add("appsecret", DTConfig.PcAppSecret);
            }
            else
            {
                _client.QueryString.Add("corpid", DTConfig.CorpId);
                _client.QueryString.Add("corpsecret", DTConfig.CorpSecret);
            }
            var url = _addressConfig.GetAccessTokenUrl;
            var result = await _client.Get(url);
            try
            {
                var accessToken = JsonConvert.DeserializeObject<AccessTokenResponseModel>(result);
                SetAppSettings("AccessToken", accessToken.access_token);
            }
            catch
            {

            }
            return result;
        }
        public Task<string> GetDepartmentList()
        {
            var url = _addressConfig.GetDepartmentListUrl;
            return _client.Get(url);
        }
        public Task<string> AddDepartment(AddDepartmentModel dpt)
        {
            var url = _addressConfig.CreateDepartmentUrl;
            return _client.UploadModel(url, dpt);
        }

        public Task<string> GetDepartmentUserList(int dptId)
        {
            var url = _addressConfig.GetDepartmentUserListUrl;
            _client.QueryString.Add("department_id", dptId.ToString());
            return _client.Get(url);
        }

        public Task<string> GetDeptUserListByDeptId(int dptId)
        {
            var url = _addressConfig.GetDepartmentUserListUrl;
            _client.QueryString.Add("department_id", dptId.ToString());
            return _client.Get(url);
        }

        public Task<string> GetDepartmentByUserId(string userId)
        {
            var url = _addressConfig.GetDepartmentByUserId;
            _client.QueryString.Add("userId", userId);
            return _client.Get(url);
        }

        public Task<string> SingleDepartment(int dptId)
        {
            var url = _addressConfig.GetDepartmentDetailUrl;
            _client.QueryString.Add("id", dptId.ToString());
            return _client.Get(url);
        }
        public Task<string> DeleteDepartment(int dptId)
        {
            var url = _addressConfig.DeleteDepartmentUrl;
            _client.QueryString.Add("id", dptId.ToString());
            return _client.Get(url);
        }
        private void SetAppSettings(string name, string value)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            config.AppSettings.Settings[name].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            ConfigurationManager.RefreshSection("appSettings");
        }

        public Task<string> GetAttendanceList(AttendanceRequestModel query)
        {
            var url = _addressConfig.GetAttendanceListUrl;
            return _client.UploadModel(url, query);
        }

        public Task<string> GetJsapiTicket()
        {
            var url = _addressConfig.GetJsapiTicketUrl;
            return _client.Get(url);
        }

        public Task<string> GetSpaceId(string agentId)
        {
            var url = _addressConfig.GetDingPanSpaceIdUrl;
            _client.QueryString.Add("domain", "test");
            _client.QueryString.Add("agent_id", agentId);
            return _client.Get(url);
        }
        
        public Task<string> SendDingPanFile(string agentId)
        {
            var url = _addressConfig.GetDingPanSpaceIdUrl;
            _client.QueryString.Add("domain", "test");
            _client.QueryString.Add("agent_id", agentId);
            return _client.Get(url);
        }
    }
}