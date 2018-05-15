using DingTalkServer;
using DingTalkServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DingTalk.Controllers
{
    [RoutePrefix("api/dt")]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]

    public class DingTalkServersController : ApiController
    {
        DingTalkManager dtManager;
        public DingTalkConfig DTConfig { get; set; } = new DingTalkConfig();

        public DingTalkServersController()
        {
            dtManager = new DingTalkManager();
        }
        [Route("accessToken")]
        public async Task<string> AccessToken()
        {
            var result = await dtManager.GetAccessToken();
            return result;
        }

        #region department curd
        [Route("departmentList")]
        public async Task<string> DepartmentList()
        {
            var result = await dtManager.GetDepartmentList();
            return result;
        }
        [Route("addDepartment")]
        public async Task<string> DepartmentAdd()
        {
            var userListStr = await dtManager.GetDepartmentUserList(1);
            var userList = JsonConvert.DeserializeObject<DepartmentUserResponseModel>(userListStr);
            var dpt = new AddDepartmentModel();
            string userId = userList.UserList.FirstOrDefault().UserId;

            dpt.Name = dpt.Name + GetRandomNum();
            //dpt.userPerimits = userId;
            //dpt.outerPermitUsers = userId;
            var result = await new DingTalkManager().AddDepartment(dpt);
            return result;
        }
        [Route("SingleDepartment")]
        public async Task<string> SingleDepartment()
        {
            var allDptStr = await dtManager.GetDepartmentList();
            var allDpt = JsonConvert.DeserializeObject<DepartmentResponseModel>(allDptStr);
            var result = await dtManager.SingleDepartment(allDpt.Department.LastOrDefault().Id);
            return result;
        }
        private string GetRandomNum()
        {
            var random = new Random(DateTime.Now.Millisecond);
            return random.Next(1, 1000000).ToString();
        }

        [Route("delDepartment")]
        public async Task<string> DelDepartment()
        {
            var allDptStr = await dtManager.GetDepartmentList();
            var allDpt = JsonConvert.DeserializeObject<DepartmentResponseModel>(allDptStr);
            var result = await dtManager.DeleteDepartment(allDpt.Department.LastOrDefault().Id);
            return result;
        }

        #endregion

        #region user curd
        [Route("getUserDetail")]
        [HttpPost]
        public async Task<string> GetUserDetail()
        {
            //string dptId = "0935455445756597";
            //var departmentUserStr = await dtManager.GetDepartmentUserList(dptId);
            //var departmentUser = JsonConvert.DeserializeObject<DepartmentUserResponseModel>(departmentUserStr);
            //string userId = departmentUser.UserList.Last().UserId;
            string userId = "0935455445756597";
            var result = await dtManager.GetUserDetail(userId);
            return result;
        }
        [Route("getDepartmentUserList")]
        [HttpPost]
        public async Task<string> GetDepartmentUserList()
        {
            string dptId = "56943182";
            var result = await dtManager.GetDepartmentUserList(dptId);
            return result;
        }

        [Route("getDepartmentUserDetailList")]
        [HttpPost]
        public async Task<string> GetDepartmentUserDetailList()
        {
            string dptId = "1";
            var result = await dtManager.GetDepartmentUserDetailList(dptId);
            return result;
        }
        [Route("createUser")]
        [HttpPost]
        public async Task<string> CreateUser()
        {
            var newUser = new AddUserRequestModel();
            newUser.Name = "test" + GetRandomNum();
            newUser.Mobile = GetRandomMobile();
            newUser.Department = new[] { 1 };
            var result = await dtManager.CreateUser(newUser);
            return result;
        }
        [Route("updateUser")]
        [HttpPost]
        public async Task<string> UpdateUser()
        {
            string dptId = "1";
            var departmentUserListStr = await dtManager.GetDepartmentUserList(dptId);
            var departmentUserList = JsonConvert.DeserializeObject<DepartmentUserResponseModel>(departmentUserListStr);
            var newUser = new AddUserRequestModel();
            newUser.Name = "testUpdate" + GetRandomNum();
            newUser.Userid = departmentUserList.UserList.Last().UserId;
            var result = await dtManager.UpdateUser(newUser);
            return result;
        }

        [Route("deleteUser")]
        [HttpPost]
        public async Task<string> DeleteUser()
        {
            string dptId = "1";
            var departmentUserListStr = await dtManager.GetDepartmentUserList(dptId);
            var departmentUserList = JsonConvert.DeserializeObject<DepartmentUserResponseModel>(departmentUserListStr);
            var userId = departmentUserList.UserList.Last().UserId;
            var result = await dtManager.DeleteUser(userId);
            return result;
        }
        [Route("batchDeleteUser")]
        [HttpPost]
        public async Task<string> BatchDeleteUser()
        {
            string dptId = "1";
            var departmentUserListStr = await dtManager.GetDepartmentUserList(dptId);
            var departmentUserList = JsonConvert.DeserializeObject<DepartmentUserResponseModel>(departmentUserListStr);
            var idList = departmentUserList.UserList.Select(p => p.UserId);
            idList = idList.Except(idList.Where(p => p.StartsWith("manager")));
            var deleteModel = new BatchDeleteUserModel(idList);

            var result = await dtManager.BatchDeleteUser(deleteModel);
            return result;
        }
        [Route("getAttendanceList")]
        [HttpPost]
        public async Task<string> GetAttendanceList()
        {
            string dptId = "1";
            var departmentUserListStr = await dtManager.GetDepartmentUserList(dptId);
            var departmentUserList = JsonConvert.DeserializeObject<DepartmentUserResponseModel>(departmentUserListStr);
            var userId = departmentUserList.UserList.FirstOrDefault()?.UserId;
            var attendanceQuery = new AttendanceRequestModel()
            {
                WorkDateFrom = DateTime.Now.AddDays(-7),
                WorkDateTo = DateTime.Now
            };

            var result = await dtManager.GetAttendanceList(attendanceQuery);
            return result;
        }

        [Route("getJsapiTicket")]
        [HttpPost]
        public async Task<string> GetJsapiTicket()
        {
            return await dtManager.GetJsapiTicket();
        }



        private string GetRandomMobile()
        {
            string mobile = "13";
            for (int i = 0; i < 9; i++)
            {
                var random = new Random(DateTime.Now.Millisecond);
                mobile += random.Next(1, 9).ToString();
                Thread.Sleep(10);
            }
            return mobile;
        }

        #endregion

        #region 发送企业消息
        [Route("sendTextMessage")]
        [HttpPost]
        public async Task<string> SendTextMessage()
        {
            var msgModel = new TextMsgModel()
            {
                Agentid = DTConfig.AgentId,
                Content = "测试一条消息",
                //Toparty = "32760351"
                Touser = "manager3312",
            };
            return await dtManager.SendMessage(msgModel);
        }

        [Route("sendImageMessage")]
        [HttpPost]
        public async Task<string> SendImageMessage()
        {
            var msgModel = new ImageMsgModel()
            {
                Agentid = "86624962",
                MediaId = "@lADOuMXP4cyWzMg",
                Touser = "manager9585"
            };
            return await dtManager.SendMessage(msgModel);
        }
        [Route("sendVoiceMessage")]
        [HttpPost]
        public async Task<string> SendVoiceMessage()
        {
            var voiceFileName = HttpContext.Current.Server.MapPath("~/测试媒体文件/声音测试.amr");
            var msgModel = new VoiceMsgModel()
            {
                Agentid = "86624962",
                Voice = new Voice()
                {
                    Media_id = "@lATOuNF5hM5IqrCXzi8wWuE",
                    Duration = dtManager.GetAMRFileDuration(voiceFileName).ToString()
                },
                Touser = "manager9585"
            };
            return await dtManager.SendMessage(msgModel);
        }
        [Route("sendFileMessage")]
        [HttpPost]
        public async Task<string> SendFileMessage()
        {
            var msgModel = new FileMsgModel()
            {
                Agentid = "86624962",
                MediaId = "@lAjOuNSfk84SSGNIzhXriIw",
                Touser = "manager9585"
            };
            return await dtManager.SendMessage(msgModel);
        }

        [Route("sendLinkMessage")]
        [HttpPost]
        public async Task<string> SendLinkMessage()
        {
            var msgModel = new LinkMsgModel()
            {
                Agentid = "86624962",
                Link = new Link()
                {
                    MessageUrl = "http://test.xiaogj.com",
                    PicUrl = "@lADOuMXP4cyWzMg",
                    Title = "测试",
                    Text = "测试内容"
                },
                Touser = "manager9585"
            };
            return await dtManager.SendMessage(msgModel);
        }


        [Route("getMessageStatus")]
        [HttpPost]
        public async Task<string> GetMessageStatus()
        {
            var messageId = "604946e0c80f36b3b523cc14ac8282e5";
            return await dtManager.GetMessageStatus(messageId);
        }

        [Route("uploadMedia")]
        [HttpPost]
        public async Task<string> UploadMedia()
        {

            //var imageFileName =HttpContext.Current.Server.MapPath("~/测试媒体文件/图片测试.jpg");
            //var uploadModel = new UploadMediaRequestModel()
            //{
            //    FileName = imageFileName,
            //    MediaType = UploadMediaType.Image
            //};

            //var voiceFileName = HttpContext.Current.Server.MapPath("~/测试媒体文件/声音测试.amr");
            //var uploadVoiceModel = new UploadMediaRequestModel()
            //{
            //    FileName = voiceFileName,
            //    MediaType = UploadMediaType.Voice
            //};

            var fileName = HttpContext.Current.Server.MapPath("~/测试媒体文件/测试文本.txt");
            var uploadVoiceModel = new UploadMediaRequestModel()
            {
                FileName = fileName,
                MediaType = UploadMediaType.File
            };
            return await dtManager.UploadFile(uploadVoiceModel);
        }
        [Route("downloadFile")]
        [HttpPost]
        public async Task<string> DownloadFile()
        {
            var fileName = HttpContext.Current.Server.MapPath("~/down.jpg");
            string mediaId = "@lADOuMXP4cyWzMg";
            var result = await dtManager.DownloadFile(mediaId, fileName);
            return result;
        }
        #endregion
    }
}
