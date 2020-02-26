using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralTests
{
    public class Finder2
    {
        public class ProcessStep
        {

            private const double Tolerance = 0.4;

            public static Rectangle SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance)
            {
                var smallData = smallBmp.LockBits(new Rectangle(0, 0, smallBmp.Width, smallBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                var bigData = bigBmp.LockBits(new Rectangle(0, 0, bigBmp.Width, bigBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                var smallStride = smallData.Stride;
                var bigStride = bigData.Stride;

                var bigWidth = bigBmp.Width;
                var bigHeight = bigBmp.Height;
                var smallWidth = smallBmp.Width * 3;
                var smallHeight = smallBmp.Height;

                var location = Rectangle.Empty;
                var margin = Convert.ToInt32(255.0 * tolerance);

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
                                location.X = x;
                                location.Y = y;
                                location.Width = smallBmp.Width;
                                location.Height = smallBmp.Height;
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

                return location;
            }
        }
    }
}
