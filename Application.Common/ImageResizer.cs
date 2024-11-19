using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Application.Common
{
	public static class ImageResizer
	{
		public static void Resize_AspectRatio(string targetPath, string sourcePath, int Width, int Height)
		{
			Image imgPhoto = Image.FromFile(sourcePath, true);

			int sourceWidth = imgPhoto.Width;
			int sourceHeight = imgPhoto.Height;
			int sourceX = 0;
			int sourceY = 0;
			int destX = 0;
			int destY = 0;

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = ((float)Width / (float)sourceWidth);
			nPercentH = ((float)Height / (float)sourceHeight);
			if (nPercentH < nPercentW)
			{
				nPercent = nPercentH;
				destX = System.Convert.ToInt16((Width -
							  (sourceWidth * nPercent)) / 2);
			}
			else
			{
				nPercent = nPercentW;
				destY = System.Convert.ToInt16((Height -
							  (sourceHeight * nPercent)) / 2);
			}

			int destWidth = (int)(sourceWidth * nPercent);
			int destHeight = (int)(sourceHeight * nPercent);

			Bitmap bmPhoto = new Bitmap(Width, Height,
							  PixelFormat.Format24bppRgb);
			bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
							 imgPhoto.VerticalResolution);

			Graphics grPhoto = Graphics.FromImage(bmPhoto);
			grPhoto.Clear(Color.White);
			grPhoto.InterpolationMode =
					InterpolationMode.HighQualityBicubic;

			grPhoto.DrawImage(imgPhoto,
				new Rectangle(destX, destY, destWidth, destHeight),
				new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
				GraphicsUnit.Pixel);

			grPhoto.Dispose();

			bmPhoto.Save(targetPath, ImageFormat.Jpeg);			
		}

        public static bool Resize(string source, string target, int maxWidth, int maxHeight, bool trimImage, ImageFormat saveFormat)
        {
            using (Image src = Image.FromFile(source, true))
            {
                // Check that we have an image
                if (src != null)
                {
                    int origX, origY, newX, newY;
                    int trimX = 0, trimY = 0;

                    // Default to size of source image
                    newX = origX = src.Width;
                    newY = origY = src.Height;

                    // Does image exceed maximum dimensions?
                    if (origX > maxWidth || origY > maxHeight)
                    {
                        // Need to resize image
                        if (trimImage)
                        {
                            // Trim to exactly fit maximum dimensions
                            double factor = Math.Max((double)maxWidth / (double)origX,
                                (double)maxHeight / (double)origY);
                            newX = (int)Math.Ceiling((double)origX * factor);
                            newY = (int)Math.Ceiling((double)origY * factor);
                            trimX = newX - maxWidth;
                            trimY = newY - maxHeight;
                        }
                        else
                        {
                            // Resize (no trim) to keep within maximum dimensions
                            double factor = Math.Min((double)maxWidth / (double)origX,
                                (double)maxHeight / (double)origY);
                            newX = (int)Math.Ceiling((double)origX * factor);
                            newY = (int)Math.Ceiling((double)origY * factor);
                        }
                    }

                    // Create destination image
                    using (Image dest = new Bitmap(newX - trimX, newY - trimY))
                    {
                        Graphics graph = Graphics.FromImage(dest);
                        graph.InterpolationMode =
                            System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graph.DrawImage(src, -(trimX / 2), -(trimY / 2), newX, newY);
                        dest.Save(target, saveFormat);
                        // Indicate success
                        return true;
                    }
                }
            }
            // Indicate failure
            return false;
        }

    }
}
