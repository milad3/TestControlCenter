using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProccessor
{
    public class ImageSearchResultItem
    {
        public int X { get; set; }

        public int Y { get; set; }

        public double Tolerance { get; set; }
    }

    class Program
    {
        private static List<ImageSearchResultItem> GetSortedResultItems(float[,,] items, double ignoreValue = .5)
        {
            var result = new List<ImageSearchResultItem>();

            for (int y = 0; y < items.GetLength(0); y++)
            {
                for (int x = 0; x < items.GetLength(1); x++)
                {
                    var score = items[y, x, 0];
                    if (score < ignoreValue)
                    {
                        continue;
                    }
                    var newItem = new ImageSearchResultItem
                    {
                        X = x,
                        Y = y,
                        Tolerance = score
                    };
                    result.Add(newItem);
                }
            }

            return result.OrderByDescending(x => x.Tolerance).ToList();
        }

        static void Main(string[] args)
        {
            var time = DateTime.Now;

            var source = @"C:\Users\hosse\OneDrive\Desktop\work test\e68f688b-a275-43f5-ba14-137ee5f9a45b.png";
            var wanted = @"C:\Users\hosse\OneDrive\Desktop\work test\2c.png";

            var image = new Image<Bgr, byte>(source);
            var innerImage = new Image<Bgr, byte>(wanted);

            var result = image.MatchTemplate(innerImage, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);

            var list = new List<ImageSearchResultItem>();

            float[,,] matches = result.Data;
            for (int y = 0; y < matches.GetLength(0); y++)
            {
                for (int x = 0; x < matches.GetLength(1); x++)
                {
                    double matchScore = matches[y, x, 0];
                    if (matchScore > .8)
                    {
                        list.Add(new ImageSearchResultItem { X = x, Y = y, Tolerance = matchScore });
                    }
                }
            }

            if (list.Count == 0)
            {
                list = GetSortedResultItems(matches);
            }


            var output = new List<Rectangle>();

            result.MinMax(out double[] minValues, out double[] maxValues, out Point[] minLocations, out Point[] maxLocations);

            for (int i = 0; i < maxLocations.Length; i++)
            {
                if (maxValues[i] > .5)
                {
                    var match = new Rectangle(maxLocations[i], innerImage.Size);
                    output.Add(match);
                }
            }

            Console.WriteLine(output.Count);

            //var s1 = @"C:\Users\Milad\Desktop\tests\1\s1.jpg";
            //var w1 = @"C:\Users\Milad\Desktop\tests\1\c1.jpg";
            //var i1 = new Image<Bgr, byte>(s1);
            //var ii1 = new Image<Bgr, byte>(w1);
            //var r1 = i1.MatchTemplate(ii1, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
            //r1.MinMax(out double[] m1, out double[] mx1, out Point[] l1, out Point[] lx1);

            //var s2 = @"C:\Users\Milad\Desktop\tests\2\s1.jpg";
            //var w2 = @"C:\Users\Milad\Desktop\tests\2\c1.jpg";
            //var i2 = new Image<Bgr, byte>(s2);
            //var ii2 = new Image<Bgr, byte>(w2);
            //var r2 = i2.MatchTemplate(ii2, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
            //r2.MinMax(out double[] m2, out double[] mx2, out Point[] l2, out Point[] lx2);

            //var s3 = @"C:\Users\Milad\Desktop\tests\1\s1.jpg";
            //var w3 = @"C:\Users\Milad\Desktop\tests\1\c2.jpg";
            //var i3 = new Image<Bgr, byte>(s3);
            //var ii3 = new Image<Bgr, byte>(w3);
            //var r3 = i3.MatchTemplate(ii3, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
            //r3.MinMax(out double[] m3, out double[] mx3, out Point[] l3, out Point[] lx3);

            //for (int i = 0; i < 100; i++)
            //{
            //    var s4 = @"C:\Users\Milad\Desktop\tests\2\s1.jpg";
            //    var w4 = @"C:\Users\Milad\Desktop\tests\2\c3.jpg";
            //    var i4 = new Image<Gray, byte>(s4);
            //    var ii4 = new Image<Gray, byte>(w4);

            //    var r4 = i4.MatchTemplate(ii4, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
            //    r4.MinMax(out double[] m4, out double[] mx4, out Point[] l4, out Point[] lx4);
            //}





            //var s5 = @"C:\Users\Milad\Desktop\tests\2\s2.jpg";
            //var w5 = @"C:\Users\Milad\Desktop\tests\2\c3.jpg";
            //var i5 = new Image<Bgr, byte>(s5);
            //var ii5 = new Image<Bgr, byte>(w5);
            //var r5 = i5.MatchTemplate(ii5, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
            //r5.MinMax(out double[] m5, out double[] mx5, out Point[] l5, out Point[] lx5);


            //var s6 = @"C:\Users\Milad\Desktop\tests\3\s1.jpg";
            //var w6 = @"C:\Users\Milad\Desktop\tests\3\c2.jpg";
            //var i6 = new Image<Bgr, byte>(s6);
            //var ii6 = new Image<Bgr, byte>(w6);
            //var r6 = i6.MatchTemplate(ii6, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
            //r6.MinMax(out double[] m6, out double[] mx6, out Point[] l6, out Point[] lx6);


            //var s7 = @"C:\Users\Milad\Desktop\tests\3\s1-low.jpg";
            //var w7 = @"C:\Users\Milad\Desktop\tests\3\c1.jpg";
            //var i7 = new Image<Bgr, byte>(s7);
            //var ii7 = new Image<Bgr, byte>(w7);
            //var r7 = i7.MatchTemplate(ii7, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
            //r7.MinMax(out double[] m7, out double[] mx7, out Point[] l7, out Point[] lx7);

            //var s8 = @"C:\Users\Milad\Desktop\tests\3\sl1.jpg";
            //var w8 = @"C:\Users\Milad\Desktop\tests\3\cl1.jpg";
            //var i8 = new Image<Bgr, byte>(s8);
            //var ii8 = new Image<Bgr, byte>(w8);
            //var r8 = i8.MatchTemplate(ii8, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
            //r8.MinMax(out double[] m8, out double[] mx8, out Point[] l8, out Point[] lx8);



            Console.WriteLine((DateTime.Now - time).TotalSeconds);

            Console.ReadKey();
        }
    }
}
