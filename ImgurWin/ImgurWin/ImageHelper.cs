using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace ImgurWin
{
	public static class ImageHelper
	{
		public const int tenMegabytes = 10 * 1000 * 1000;
		/// <summary>
		/// Encodes and saves the specified image to a byte array. If the jpegQuality image is between 0 and 100 (inclusive) then it is saved in Jpeg format. Otherwise PNG format.
		/// </summary>
		/// <param name="image">The image to encode and save.</param>
		/// <param name="jpegQuality">If between 0 and 100 (inclusive) the image is saved in Jpeg format at this quality.</param>
		/// <returns>A byte array containing the encoded image.</returns>
		public static byte[] SaveImageToByteArray(Image image, int jpegQuality = -1)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				if (jpegQuality >= 0 && jpegQuality <= 100)
				{
					ImageCodecInfo imageCodecInfo = ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
					EncoderParameters encoderParameters = new EncoderParameters(1);
					encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, jpegQuality);
					image.Save(ms, imageCodecInfo, encoderParameters);
					return ms.ToArray();
				}
				else
				{
					image.Save(ms, ImageFormat.Png);
					return ms.ToArray();
				}
			}
		}
		/// <summary>
		/// Returns an Image object created from compressed image file data.
		/// </summary>
		/// <param name="compressedImage">The content of a jpg, png, gif, or bmp image file.</param>
		/// <returns></returns>
		public static Bitmap BitmapFromCompressedImageData(byte[] compressedImage)
		{
			MemoryStream ms = new MemoryStream(compressedImage);
			return (Bitmap)Bitmap.FromStream(ms); // Do not dispose of ms here because the Image requires it remain available.
		}

		public static string GetMimeType(ImageFormat rawFormat)
		{
			if (ImageFormat.Bmp.Equals(rawFormat))
				return "image/bmp";
			else if (ImageFormat.Gif.Equals(rawFormat))
				return "image/gif";
			else if (ImageFormat.Png.Equals(rawFormat))
				return "image/png";
			else if (ImageFormat.Tiff.Equals(rawFormat))
				return "image/tiff";
			else if (ImageFormat.Jpeg.Equals(rawFormat))
				return "image/jpeg";
			else
				return "application/octet-stream";
		}
		public static byte[] GetEfficientCompressedImageData(Image imgSrc, out string mimeType)
		{
			byte[] imgDataJpeg = SaveImageToByteArray(imgSrc, 80);
			byte[] imgDataPng = SaveImageToByteArray(imgSrc);
			byte[] imgData;

			// Choose PNG format if it is not a significant size penalty, because PNG is lossless.
			if (imgDataPng.Length <= imgDataJpeg.Length * 1.2 && imgDataPng.Length < tenMegabytes)
				imgData = imgDataPng;
			else
				imgData = imgDataJpeg;

			// If the image is too large, reduce its quality until it is not too large
			for (int jpegQuality = 80; imgData.Length >= tenMegabytes && jpegQuality >= 0; jpegQuality -= 10)
				imgData = SaveImageToByteArray(imgSrc, jpegQuality);

			mimeType = imgData == imgDataPng ? "image/png" : "image/jpeg";

			return imgData;
		}
	}
}
