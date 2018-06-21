using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DingTalkServer
{
    public class HttpsClient:WebClient
    {
       // public WebClient this { get; set; }

        
        Uri _url;
        public HttpsClient()
        {
            IntialRequest();
        }

        private void IntialRequest()
        {
            this.UseDefaultCredentials = true;
            this.Encoding = Encoding.UTF8;
            this.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
        }

        public HttpsClient(string url) : this()
        {
            _url = new Uri(url);

        }
        public Task<string> Get(string url = null)
        {
            if (url != null)
                _url = new Uri(url);

            var result = this.DownloadStringTaskAsync(_url);
            this.Dispose();
            return result;
        }
        public async Task<string> GetFile(string url,string fileName)
        {
            if (url != null)
                _url = new Uri(url);
            //var fileStream = await this.OpenReadTaskAsync(_url);
            //var fileBytes = new byte[fileStream.Length];
            //fileStream.Read(fileBytes, 0, fileBytes.Length);

            //HttpWebRequest request = WebRequest.CreateHttp(_url);
            //request.Method = "get";
            //var strStream = Encoding.UTF8.GetString(fileBytes);
            //var result = request.GetResponseAsync();
            //var 
            await this.DownloadFileTaskAsync(_url, fileName);
            this.Dispose();
            return "下载成功";
        }

        public Task<string> UploadModel<T>(string url, T data)
        {
            if (url != null)
                _url = new Uri(url);

            var dataStr = JsonConvert.SerializeObject(data);
            var result = this.UploadStringTaskAsync(_url, "post", dataStr);
            this.Dispose();
            return result;
        }
        public async Task<string>  UploadOneFile(string url, string fileName)
        {
            if (url != null)
                _url = new Uri(url);
            var boundary = "";
            var mimeByte =BuildContent(out boundary,fileName);
            var mimeStr = Encoding.UTF8.GetString(mimeByte);
            this.Headers["Content-Type"] = string.Format("multipart/form-data; boundary={0}", boundary);
            var result = await this.UploadDataTaskAsync(_url,"post", mimeByte);
            
            var retStr = Encoding.UTF8.GetString(result);
            this.Dispose();
            return retStr;
        }

        private byte[] BuildContent(out string boundary,string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("需要上传的文件不存在");
            boundary = GenerateRadomStr();
            var contentTypeAndName = GetFileNameAndContentType(fileName);
            var contentHead = $"--{boundary}\r\n" +
                              $"Content-Disposition: form-data; name=\"media\"; filename=\"{contentTypeAndName.Item1}\"\r\n" +
                              $"Content-Type: {contentTypeAndName.Item2}\r\n\r\n";

            var headBuffer = Encoding.UTF8.GetBytes(contentHead);
            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            var fileBuffer = new byte[fileStream.Length];
            fileStream.Read(fileBuffer, 0, fileBuffer.Length);
            fileStream.Dispose();
            var contentEnd = $"\r\n--{boundary}--\r\n";
            var contentEndBuffer = Encoding.UTF8.GetBytes(contentEnd);
            var contentBuffer = new byte[headBuffer.Length + fileBuffer.Length + contentEnd.Length];
            int offset = 0;
            Buffer.BlockCopy(headBuffer, 0, contentBuffer, offset, headBuffer.Length);
            offset += headBuffer.Length;
            Buffer.BlockCopy(fileBuffer, 0, contentBuffer, offset, fileBuffer.Length);
            offset += fileBuffer.Length;
            Buffer.BlockCopy(contentEndBuffer, 0, contentBuffer, offset, contentEndBuffer.Length);
            return contentBuffer;
        }

        private Tuple<string,string> GetFileNameAndContentType(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("需要上传的文件不存在");
            FileInfo info = new FileInfo(fileName);
            string contentType = "application/octet-stream";
            switch (info.Extension)
            {
                case ".jpg":
                    contentType = "image/jpeg";
                    break;
                default:
                    break;
            }
            return new Tuple<string, string>(info.Name, contentType);
        }

        public Task<byte[]> DownloadFile(string url,string fileName)
        {
            if (url != null)
                _url = new Uri(url);

            //var result = this.DownloadFileTaskAsync(url,fileName);
            //this.Headers[HttpRequestHeader.ContentType] = "";
            var result = this.DownloadDataTaskAsync(url);
            return result;
        }

        public  async Task<string> UploadFile(string url,string fileName,int timeout)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("需要上传的文件不存在");
            var fileStream = new FileStream(fileName,FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            var fileBuffer = new byte[fileStream.Length];
            fileStream.Read(fileBuffer, 0,fileBuffer.Length);
            fileStream.Dispose();
            var boundary = GenerateRadomStr();
            this.Headers["Content-Type"]= string.Format("multipart/form-data; boundary={0}", boundary);
            string fileFormdataTemplate =
                            "\r\n--" + boundary +
                            "\r\nContent-Disposition:form-data;name=\"{0}\";filename=\"{1}\"" +
                            "\r\nContent-Type:image/jpeg" +
                            "\r\n\r\n";
            string formDataHeader = String.Format(fileFormdataTemplate, "media", fileName);
            byte[] formDataHeaderBuffer = Encoding.UTF8.GetBytes(formDataHeader);

            string begin = $"--{boundary}\r\n";
            byte[] beginBuffer = Encoding.UTF8.GetBytes(begin);

            string end = $"\r\n--{boundary}--\r\n";
            byte[] endBuffer = Encoding.UTF8.GetBytes(end); 

            byte[] dataStream = new byte[formDataHeaderBuffer.Length + beginBuffer.Length + fileBuffer.Length + endBuffer.Length];
            formDataHeaderBuffer.CopyTo(dataStream, 0);
            beginBuffer.CopyTo(dataStream, formDataHeaderBuffer.Length);
            fileBuffer.CopyTo(dataStream, formDataHeaderBuffer.Length + begin.Length);
            endBuffer.CopyTo(dataStream, formDataHeaderBuffer.Length + begin.Length + fileBuffer.Length);
            var returnBuffer = await this.UploadDataTaskAsync(url, "POST", dataStream);
            string resultJson = Encoding.UTF8.GetString(returnBuffer);
            return resultJson;
        }

        public  string GenerateRadomStr(int length = 16)
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string str = "-------------------------";
            Random rad = new Random();
            for (int i = 0; i < length; i++)
            {
                str += chars.Substring(rad.Next(0, chars.Length - 1), 1);
            }
            return str;
        }
       
        //public byte[] BuildMultipart()
        //{

        //}
    }
}