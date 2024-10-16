using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;
using ImageMagick;
using ImageMagick.Drawing;

namespace AdminBot.Net.Extra
{
    public class ImageSymmetry
    {
        public static Bitmap SymmetryL(Bitmap bmp)
        {
            Rectangle CropRect = new(0, 0, bmp.Width / 2, bmp.Height);
            Bitmap CroppedImage = bmp.Clone(CropRect, bmp.PixelFormat);
            Bitmap CroppedImageOrigin = (Bitmap)CroppedImage.Clone();
            Bitmap Bg = new(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(Bg);
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipY);
            g.DrawImage(CroppedImageOrigin, new System.Drawing.Point(0, 0));
            g.DrawImage(CroppedImage, new System.Drawing.Point(bmp.Width / 2, 0));
            bmp.Dispose();
            CroppedImage.Dispose();
            CroppedImageOrigin.Dispose();
            return Bg;
        }
        public static Bitmap SymmetryR(Bitmap bmp)
        {
            Rectangle CropRect = new(bmp.Width / 2, 0, bmp.Width / 2, bmp.Height);
            Bitmap CroppedImage = bmp.Clone(CropRect, bmp.PixelFormat);
            Bitmap CroppedImageOrigin = (Bitmap)CroppedImage.Clone();
            Bitmap Bg = new(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(Bg);
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipY);
            g.DrawImage(CroppedImageOrigin, new System.Drawing.Point(bmp.Width / 2, 0));
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, 0));
            bmp.Dispose();
            CroppedImage.Dispose();
            CroppedImageOrigin.Dispose();
            return Bg;
        }
        public static Bitmap SymmetryU(Bitmap bmp)
        {
            Rectangle CropRect = new(0, 0, bmp.Width, bmp.Height / 2);
            Bitmap CroppedImage = bmp.Clone(CropRect, bmp.PixelFormat);
            Bitmap CroppedImageOrigin = (Bitmap)CroppedImage.Clone();
            Bitmap Bg = new(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(Bg);
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            g.DrawImage(CroppedImageOrigin, new System.Drawing.Point(0, 0));
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, bmp.Height / 2));
            bmp.Dispose();
            CroppedImage.Dispose();
            CroppedImageOrigin.Dispose();
            return Bg;
        }
        public static Bitmap SymmetryD(Bitmap bmp)
        {
            Rectangle CropRect = new(0, bmp.Height / 2, bmp.Width, bmp.Height / 2);
            Bitmap CroppedImage = bmp.Clone(CropRect, bmp.PixelFormat);
            Bitmap CroppedImageOrigin = (Bitmap)CroppedImage.Clone();
            Bitmap Bg = new(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(Bg);
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            g.DrawImage(CroppedImageOrigin, new System.Drawing.Point(0, bmp.Height / 2));
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, 0));
            bmp.Dispose();
            CroppedImage.Dispose();
            CroppedImageOrigin.Dispose();
            return Bg;
        }
    }

    public class PicConvert
    {
        public static Bitmap PicTransform(Bitmap PicImage, string Method)
        {
            var TransformMethod = ImageSymmetry.SymmetryL;
            if (Method.Equals("右左"))
            {
                TransformMethod = ImageSymmetry.SymmetryR;
            }
            else if (Method.Equals("上下"))
            {
                TransformMethod = ImageSymmetry.SymmetryU;
            }
            else if (Method.Equals("下上"))
            {
                TransformMethod = ImageSymmetry.SymmetryD;
            }
            return TransformMethod(PicImage);
        }
    }

    public class GifConvert
    {

        private static readonly byte[] DefaultBytes = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0];

        public static uint GetGifFrameDelay(Image image)
        {
            try
            {
                return (uint)BitConverter.ToInt32(image.GetPropertyItem(0x5100)?.Value ?? DefaultBytes, 4);
            }
            catch
            {
                return 0;
            }
        }
        public static MagickImageCollection GifTransform(Image GifImage, string Method)
        {
            var TransformMethod = ImageSymmetry.SymmetryL;
            if (Method.Equals("右左"))
            {
                TransformMethod = ImageSymmetry.SymmetryR;
            }
            else if (Method.Equals("上下"))
            {
                TransformMethod = ImageSymmetry.SymmetryU;
            }
            else if (Method.Equals("下上"))
            {
                TransformMethod = ImageSymmetry.SymmetryD;
            }
            FrameDimension Dimension = new(GifImage.FrameDimensionsList[0]);
            int FrameCount = GifImage.GetFrameCount(Dimension);
            uint Delay = GetGifFrameDelay(GifImage);
            var Ncollection = new MagickImageCollection();
            for (int i = 0; i < FrameCount; i++)
            {
                GifImage.SelectActiveFrame(Dimension, i);
                Bitmap frame = new(GifImage);
                MemoryStream FMemoryStream = new();
                TransformMethod(frame).Save(FMemoryStream, ImageFormat.Bmp);
                FMemoryStream.Position = 0;
                MagickImage MagickFrame = new(FMemoryStream)
                {
                    AnimationDelay = Delay,
                    GifDisposeMethod = GifDisposeMethod.Background
                };
                Ncollection.Add(MagickFrame);
                FMemoryStream.Close();
                frame.Dispose();
            }
            Ncollection[0].AnimationIterations = 0;
            GifImage.Dispose();
            return Ncollection;
        }
    }
}
