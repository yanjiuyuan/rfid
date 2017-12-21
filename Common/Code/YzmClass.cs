using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Code
{
    public class YzmClass
    {
        #region 验证码

        //随机码的长度 
        private const int CODELENGTH = 4;         //随机码 
        private String randomCode = "";

        //更新验证码 
        private void updatePic()
        {
            randomCode = CreateRandomCode(CODELENGTH);
            //CreateImage(randomCode);
        }

        public string CreateRandomCode(int length)
        {
            int rand; char code;
            string randomCode = String.Empty;
            //生成一定长度的验证码 
            System.Random random = new Random(); for (int i = 0; i < length; i++)
            {
                rand = random.Next();
                if (rand % 3 == 0)
                {
                    code = (char)('A' + (char)(rand % 26));
                }
                else
                {
                    code = (char)('0' + (char)(rand % 10));
                }
                randomCode += code.ToString();
            }
            return randomCode;
        }

        //创建随机图片 
        public byte[] CreateImage(string randomCode)
        {
            try
            {
                int randAngle = 45;//随机旋转角度 
                int mapWidth = (int)(randomCode.Length * 21);//存放验证码的图片的长度                
                Bitmap map = new Bitmap(mapWidth, 28);//创建图片背景  
                Graphics graph = Graphics.FromImage(map);
                graph.Clear(Color.AliceBlue);//清除画面，填充背景
                graph.DrawRectangle(new Pen(Color.Black, 0), 0, 0, map.Width - 1, map.Height - 1);//画一个边框 
                graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;//指定模式  
                Random rand = new Random();
                //背景噪点生成 
                Pen blackPen = new Pen(Color.LightGray, 0); for (int i = 0; i < 20; i++)
                {
                    int x = rand.Next(0, map.Width); int y = rand.Next(0, map.Height);
                    graph.DrawRectangle(blackPen, x, y, 1, 1);
                }
                //验证码旋转，防止机器识别 
                char[] chars = randomCode.ToCharArray();//拆散字符串成单字符数组  
                //文字居中 
                StringFormat format = new StringFormat(StringFormatFlags.NoClip);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                //定义颜色 
                Color[] c = { Color.Black, Color.Blue, Color.Red, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan };
                //定义字体 
                string[] font = { "Verdana", "Arial", "宋体" };
                for (int i = 0; i < chars.Length; i++)
                {
                    int cindex = rand.Next(6); int findex = rand.Next(2);
                    Font f = new System.Drawing.Font(font[findex], 13, System.Drawing.FontStyle.Bold);//字体样式，参数2为字体大小 
                    Brush b = new System.Drawing.SolidBrush(c[cindex]);
                    Point dot = new Point(16, 16);
                    float angle = rand.Next(-randAngle, +randAngle);//随机旋转的角度                     
                    graph.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置                     
                    graph.RotateTransform(angle);//旋转角度 
                    graph.DrawString(chars[i].ToString(), f, b, 1, 1, format);
                    graph.RotateTransform(-angle);//转回去 
                    graph.TranslateTransform(2, -dot.Y);//移动光标到指定位置                 
                }
                // pbVerifyCode.Image = map;
                MemoryStream ms = new MemoryStream();
                map.Save(ms, ImageFormat.Jpeg);

                ////输出图片流
                //context.Response.Clear();
                //context.Response.ContentType = "image/jpeg";
                //context.Response.BinaryWrite(ms.ToArray());
                return ms.ToArray();
            }
            catch (ArgumentException)
            {
                return null;
                // MessageBox.Show("创建图片错误！");
            }
        }
    }

    #endregion
}
