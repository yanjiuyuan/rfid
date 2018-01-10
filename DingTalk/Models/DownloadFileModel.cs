using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class DownloadFileModel:ResponseBaseModel
    {
    /// <summary>  
    /// HTTP响应头  
    /// </summary>  
    public Dictionary<string, string> Header { get; set; }

    /// <summary>  
    /// 获取的数据  
    /// </summary>  
    public byte[] Data { get; set; }

    /// <summary>  
    /// 文件长度  
    /// </summary>  
    public int FileLength { get; set; }

    /// <summary>  
    /// 文件类型  
    /// </summary>  
    public String FileType { get; set; }

}  
}