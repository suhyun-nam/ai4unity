﻿// AForge Image Processing Library
// Portable AForge.NET framework
// https://github.com/cureos/aforge
//
// Shim.System.Drawing
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace System.Drawing
{
	public sealed partial class Bitmap
    {
        #region FIELDS

	    private const double Dpi = 96.0;

	    private static readonly Dictionary<System.Drawing.Imaging.PixelFormat, System.Windows.Media.PixelFormat>
	        PixelFormatTranslator = new Dictionary<System.Drawing.Imaging.PixelFormat, Windows.Media.PixelFormat>
	        {
	            { System.Drawing.Imaging.PixelFormat.Format32bppPArgb, PixelFormats.Pbgra32 },
	            { System.Drawing.Imaging.PixelFormat.Format32bppArgb, PixelFormats.Bgra32 },
	            { System.Drawing.Imaging.PixelFormat.Format24bppRgb, PixelFormats.Bgr24 },
	            { System.Drawing.Imaging.PixelFormat.Format32bppRgb, PixelFormats.Bgr32 },
	            { System.Drawing.Imaging.PixelFormat.Format16bppRgb555, PixelFormats.Bgr555 },
	            { System.Drawing.Imaging.PixelFormat.Format16bppRgb565, PixelFormats.Bgr565 },
	            { System.Drawing.Imaging.PixelFormat.Format1bppIndexed, PixelFormats.Indexed1 },
	            { System.Drawing.Imaging.PixelFormat.Format4bppIndexed, PixelFormats.Indexed4 },
	            { System.Drawing.Imaging.PixelFormat.Format8bppIndexed, PixelFormats.Indexed8 },
	            { System.Drawing.Imaging.PixelFormat.Format16bppGrayScale, PixelFormats.Gray16 },
	            { System.Drawing.Imaging.PixelFormat.Format48bppRgb, PixelFormats.Rgb48 },
	            { System.Drawing.Imaging.PixelFormat.Format64bppPArgb, PixelFormats.Prgba64 },
	            { System.Drawing.Imaging.PixelFormat.Format64bppArgb, PixelFormats.Rgba64 }
	        };

        #endregion

        #region OPERATORS

        public static implicit operator BitmapSource(Bitmap bitmap)
		{
            var width = bitmap.Width;
            var height = bitmap.Height;
            var pixelFormat = GetBitmapSourcePixelFormat(bitmap.PixelFormat);
            var stride = bitmap._stride;

            var bitmapSource = new WriteableBitmap(bitmap.Width, bitmap.Height, Dpi, Dpi, pixelFormat,
                GetBitmapSourcePalette(pixelFormat));
            bitmapSource.WritePixels(new Int32Rect(0, 0, width, height), bitmap._scan0, height * stride, stride);

            return bitmapSource;
		}

	    public static implicit operator Bitmap(BitmapSource bitmapSource)
		{
            var width = bitmapSource.PixelWidth;
			var height = bitmapSource.PixelHeight;
			var format = GetBitmapPixelFormat(bitmapSource.Format);
		    var stride = GetStride(width, format);

			var bitmap = new Bitmap(width, height, format);
			var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, format);
            bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), data.Scan0, height * stride, stride);
			bitmap.UnlockBits(data);

			return bitmap;
		}

		#endregion

        #region METHODS

        public static Bitmap FromStream(Stream stream)
        {
            var bitmapImage = new BitmapImage { StreamSource = stream };
            return bitmapImage;
        }

        private static System.Windows.Media.PixelFormat GetBitmapSourcePixelFormat(System.Drawing.Imaging.PixelFormat bitmapPixelFormat)
        {
            if (!PixelFormatTranslator.ContainsKey(bitmapPixelFormat))
                throw new ArgumentOutOfRangeException("bitmapPixelFormat", bitmapPixelFormat,
                    "Unsupported pixel format.");

            return PixelFormatTranslator[bitmapPixelFormat];
        }

        private static System.Drawing.Imaging.PixelFormat GetBitmapPixelFormat(System.Windows.Media.PixelFormat bitmapSourcePixelFormat)
        {
            if (!PixelFormatTranslator.ContainsValue(bitmapSourcePixelFormat))
                throw new ArgumentOutOfRangeException("bitmapSourcePixelFormat", bitmapSourcePixelFormat,
                    "Unsupported pixel format.");

            return PixelFormatTranslator.Single(kv => kv.Value == bitmapSourcePixelFormat).Key;
	    }

        private static BitmapPalette GetBitmapSourcePalette(System.Windows.Media.PixelFormat pixelFormat)
        {
            if (pixelFormat == PixelFormats.Indexed1)
                return BitmapPalettes.BlackAndWhite;
            if (pixelFormat == PixelFormats.Indexed4)
                return BitmapPalettes.Gray16;
            if (pixelFormat == PixelFormats.Indexed8)
                return BitmapPalettes.Gray256;

            return null;
        }

        #endregion
    }
}