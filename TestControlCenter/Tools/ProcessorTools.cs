using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using TestControlCenterDomain;

namespace TestControlCenter.Tools
{
    public class ProcessorTools : IProcessorTools
    {
        public List<ImageSearchResultItem> SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance)
        {
            GetImages(smallBmp, bigBmp, default, out BitmapData smallData, out BitmapData bigData);
            return Process(smallBmp, bigBmp, tolerance, smallData, bigData);
        }

        private static List<ImageSearchResultItem> Process(Bitmap smallBmp, Bitmap bigBmp, double tolerance, BitmapData smallData, BitmapData bigData)
        {
            var smallStride = smallData.Stride;
            var bigStride = bigData.Stride;

            var bigWidth = bigBmp.Width;
            var bigHeight = bigBmp.Height;
            var smallWidth = smallBmp.Width * 3;
            var smallHeight = smallBmp.Height;

            var location = Rectangle.Empty;
            var margin = Convert.ToInt32(255.0 * tolerance);

            var result = new List<ImageSearchResultItem>();

            unsafe
            {
                var pSmall = (byte*)(void*)smallData.Scan0;
                var pBig = (byte*)(void*)bigData.Scan0;

                var smallOffset = smallStride - smallBmp.Width * 3;
                var bigOffset = bigStride - bigBmp.Width * 3;

                var matchFound = true;

                for (var y = 0; y < bigHeight; y++)
                {
                    for (var x = 0; x < bigWidth; x++)
                    {
                        var pBigBackup = pBig;
                        var pSmallBackup = pSmall;

                        for (var i = 0; i < smallHeight; i++)
                        {
                            int j;
                            matchFound = true;
                            for (j = 0; j < smallWidth; j++)
                            {
                                var inf = pBig[0] - margin;
                                var sup = pBig[0] + margin;
                                if (sup < pSmall[0] || inf > pSmall[0])
                                {
                                    matchFound = false;
                                    break;
                                }

                                pBig++;
                                pSmall++;
                            }

                            if (!matchFound) break;

                            pSmall = pSmallBackup;
                            pBig = pBigBackup;

                            pSmall += (smallWidth + smallOffset) * (1 + i);
                            pBig += (bigWidth * 3 + bigOffset) * (1 + i);
                        }

                        if (matchFound)
                        {
                            result.Add(new ImageSearchResultItem
                            {
                                X = x,
                                Y = y,
                                Tolerance = 1
                            });
                            break;
                        }

                        pBig = pBigBackup;
                        pSmall = pSmallBackup;
                        pBig += 3;
                    }

                    if (matchFound) break;

                    pBig += bigOffset;
                }
            }

            bigBmp.UnlockBits(bigData);
            smallBmp.UnlockBits(smallData);

            return result;
        }

        private static void GetImages(Bitmap smallBmp, Bitmap bigBmp, Rectangle searchArea, out BitmapData smallData, out BitmapData bigData)
        {
            smallData = smallBmp.LockBits(new Rectangle(0, 0, smallBmp.Width, smallBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            if(searchArea == default)
            {
                bigData = bigBmp.LockBits(new Rectangle(0, 0, bigBmp.Width, bigBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            }
            else
            {
                bigData = bigBmp.LockBits(searchArea, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            }
        }

        public List<ImageSearchResultItem> SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance, Rectangle searchArea)
        {
            GetImages(smallBmp, bigBmp, searchArea, out BitmapData smallData, out BitmapData bigData);
            return Process(smallBmp, bigBmp, tolerance, smallData, bigData);
        }

        public List<ImageSearchResultItem> SearchImage(string smallImageFilePath, string bigImageFilePath, double tolerance)
        {
            throw new NotImplementedException();
        }
    }
}
