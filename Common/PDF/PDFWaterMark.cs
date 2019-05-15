using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.PDF
{
   public class PDFWaterMark
    {
        public string Mark(string WorkingDirectory)
        {
            string Copyright = "Copyright Zalubowski";

            System.Drawing.Image imgPhoto = System.Drawing.Image.FromFile(WorkingDirectory + "tmp.jpg");
            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;

            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            System.Drawing.Image imgWatermark = new Bitmap(WorkingDirectory + "\\43.bmp");
            int wmWidth = imgWatermark.Width;
            int wmHeight = imgWatermark.Height;

            grPhoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            grPhoto.DrawImage(
            imgPhoto,
            new Rectangle(0, 0, phWidth, phHeight),
            0,
            0,
            phWidth,
            phHeight,
            GraphicsUnit.Pixel);


            int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };
            Font crFont = null;
            SizeF crSize = new SizeF();
            for (int i = 0; i < 7; i++)
            {
                crFont = new Font("arial", sizes[i], FontStyle.Bold);
                crSize = grPhoto.MeasureString(Copyright, crFont);

                if ((ushort)crSize.Width < (ushort)phWidth)
                    break;
            }


            int yPixlesFromBottom = (int)(phHeight * .05);
            float yPosFromBottom = ((phHeight - yPixlesFromBottom) - (crSize.Height / 2));
            float xCenterOfImg = (phWidth / 2);

            StringFormat StrFormat = new StringFormat();
            StrFormat.Alignment = StringAlignment.Center;

            SolidBrush semiTransBrush2 = new SolidBrush(Color.FromArgb(153, 0, 0, 0));

            grPhoto.DrawString(Copyright,
            crFont,
            semiTransBrush2,
            new PointF(xCenterOfImg + 1, yPosFromBottom + 1),
            StrFormat);

            SolidBrush semiTransBrush = new SolidBrush(
            Color.FromArgb(153, 255, 255, 255));

            grPhoto.DrawString(Copyright,
            crFont,
            semiTransBrush,
            new PointF(xCenterOfImg, yPosFromBottom),
            StrFormat);

            Bitmap bmWatermark = new Bitmap(bmPhoto);
            bmWatermark.SetResolution(
            imgPhoto.HorizontalResolution,
            imgPhoto.VerticalResolution);

            Graphics grWatermark =
            Graphics.FromImage(bmWatermark);

            ImageAttributes imageAttributes =
            new ImageAttributes();
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable,
            ColorAdjustType.Bitmap);


            float[][] colorMatrixElements = {
new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
new float[] {0.0f,  0.0f,  0.0f,  0.3f, 0.0f},
new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
};

            ColorMatrix wmColorMatrix = new
            ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(wmColorMatrix,
            ColorMatrixFlag.Default,
            ColorAdjustType.Bitmap);

            int xPosOfWm = ((phWidth - wmWidth) - 10);
            int yPosOfWm = 10;

            grWatermark.DrawImage(imgWatermark,
            new Rectangle(xPosOfWm, yPosOfWm, wmWidth,
            wmHeight),
            0,
            0,
            wmWidth,
            wmHeight,
            GraphicsUnit.Pixel,
            imageAttributes);

            imgPhoto = bmWatermark;
            grPhoto.Dispose();
            grWatermark.Dispose();

            //watermark_final.jpg", 
            imgPhoto.Save(WorkingDirectory + "ImageFormat.Jpeg");
            imgPhoto.Dispose();
            imgWatermark.Dispose();
            return "";
        }
    }
}
