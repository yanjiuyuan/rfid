using DingTalkServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace DingTalkServer
{
    public partial class DingTalkManager
    {
        public async Task<string> SendMessage(MessageRequestBaseModel msgModel)
        {
            
            var url = _addressConfig.SendMessageUrl;
            var result = await _client.UploadModel(url,msgModel);
            return result;
        }
        
        public async Task<string> UploadFile(UploadMediaRequestModel uploadModel)
        {
            var url = _addressConfig.UploadMediaUrl;
            _client.QueryString.Add("type", uploadModel.MediaType.ToString().ToLower());
            var result= await _client.UploadOneFile(url, uploadModel.FileName);
            return result;

        }

        public async Task<string> DownloadFile(string mediaId, string fileName)
        {
            var url = _addressConfig.DownloadFileUrl;
            _client.QueryString.Add("media_id", mediaId);
            ////var result= _client.Get(url);
            //var result = await _client.DownloadFile(url, fileName);
            //var fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //fileStream.Write(result, 0, result.Length);
            //fileStream.Close();
            //var reStr = Encoding.Unicode.GetString(result);
            //var media = await FetchMediaFile(mediaId);
            var result=await _client.GetFile(url,fileName);
            if (File.Exists(fileName))
                return "{\"errcode\": 0,\"errmsg\": \"ok\"}";
            else
            {
                return result;
            }
        }

        public Task<string> GetMessageStatus(string msgId)
        {
            var url = _addressConfig.GetMessageListStatusUrl;
            var msgIdDic = new Dictionary<string, string>() { { "messageId", msgId } };
            return _client.UploadModel(url, msgIdDic);
        }

        public long GetAMRFileDuration(string fileName)
        {
            long duration = 0;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            {
                byte[] packed_size = new byte[16] { 12, 13, 15, 17, 19, 20, 26, 31, 5, 0, 0, 0, 0, 0, 0, 0 };
                int pos = 0;
                pos += 6;
                long lenth = fs.Length;
                byte[] toc = new byte[1];
                int framecount = 0;
                byte ft;
                while (pos < lenth)
                {
                    fs.Seek(pos, SeekOrigin.Begin);
                    if (1 != fs.Read(toc, 0, 1))
                    {
                        duration = lenth > 0 ? ((lenth - 6) / 650) : 0;
                        fs.Close();
                        break;
                    }
                    ft = (byte)((toc[0] / 8) & 0x0F);
                    pos += packed_size[ft] + 1;
                    framecount++;
                }
                duration = framecount * 20 / 1000;
            }
            fs.Close();
            return duration;
        }

        public  async Task<DownloadFileModel> FetchMediaFile(string mediaId)
        {
            DownloadFileModel result = null;

            var url = _addressConfig.DownloadFileUrl;
            _client.QueryString.Add("media_id", mediaId);
            //var result= _client.Get(url);
            _client.Headers[System.Net.HttpRequestHeader.ContentType] = "";
            var data = await _client.DownloadDataTaskAsync(url);

            int testHeaderMaxLength = 100;
            var testHeaderBuffer = new byte[(data.Length < testHeaderMaxLength ? data.Length : testHeaderMaxLength)];
            Array.Copy(data, 0, testHeaderBuffer, 0, testHeaderBuffer.Length);
            Encoding encoder = Encoding.UTF8;
            String testHeaderStr = encoder.GetString(testHeaderBuffer);
            if (testHeaderStr.StartsWith("--"))
            {//正常返回数据时，第一行数据为分界线，而分界线必然以"--"开始.  
                var tempArr = testHeaderStr.Split(new String[] { Environment.NewLine }, StringSplitOptions.None);
                string boundary = tempArr[0] + Environment.NewLine;
                int boundaryByteLength = encoder.GetBytes(boundary).Length;
                byte[] destData = new byte[data.Length - boundaryByteLength];
                Array.Copy(data, boundaryByteLength, destData, 0, destData.Length);
                result = new DownloadFileModel();
                
                result.Data = destData;

                const string Content_Length = "Content-Length";
                if (_client.ResponseHeaders == null || (!_client.ResponseHeaders.AllKeys.Contains(Content_Length)))
                {
                    result.FileLength = -1;
                }

                var lengthStr = _client.ResponseHeaders[Content_Length];
                int length = 0;
                if (int.TryParse(lengthStr, out length))
                {
                    result.FileLength = length;
                }
                else
                {
                    result.FileLength = 0;
                }

                const string Content_Type = "Content-Type";
                if (_client.ResponseHeaders == null || (!_client.ResponseHeaders.AllKeys.Contains(Content_Type)))
                {
                    result.FileType = "unknown";
                }
                else
                {
                    result.FileType = _client.ResponseHeaders[Content_Type];
                }
            }
            else
            {
                string resultJson = encoder.GetString(data);
               
            }
            return result;
        }
    }
}