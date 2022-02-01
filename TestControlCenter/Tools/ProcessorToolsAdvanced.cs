using Accord.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TestControlCenterDomain;

namespace TestControlCenter.Tools
{
    public class ProcessorToolsAdvanced : IProcessorTools
    {
        public List<ImageSearchResultItem> SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance)
        {   
            var tm = new ExhaustiveTemplateMatching(0.95f);
            var matchings = tm.ProcessImage(bigBmp, smallBmp);

            var result = new List<ImageSearchResultItem>();
            foreach (var item in matchings)
            {
                result.Add(new ImageSearchResultItem
                {
                    X = item.Rectangle.X,
                    Y = item.Rectangle.Y,
                    Tolerance = item.Similarity
                });
            }

            return result;
        }

        public List<ImageSearchResultItem> SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance, Rectangle searchArea)
        {
            var tm = new ExhaustiveTemplateMatching(0.95f);
            var matchings = tm.ProcessImage(bigBmp, smallBmp, searchArea);

            var result = new List<ImageSearchResultItem>();
            foreach (var item in matchings)
            {
                result.Add(new ImageSearchResultItem
                {
                    X = item.Rectangle.X,
                    Y = item.Rectangle.Y,
                    Tolerance = item.Similarity
                });
            }

            return result;
        }

        private List<ImageSearchResultItem> GetSortedResultItems(float[,,] items, double ignoreValue = .5)
        {
            var result = new List<ImageSearchResultItem>();

            for (int y = 0; y < items.GetLength(0); y++)
            {
                for (int x = 0; x < items.GetLength(1); x++)
                {
                    var score = items[y, x, 0];
                    if(score < ignoreValue)
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

        public List<ImageSearchResultItem> SearchImage(string smallImageFilePath, string bigImageFilePath, double tolerance)
        {
            using (var big = new Image<Gray, byte>(bigImageFilePath))
            {
                using (var small = new Image<Gray, byte>(smallImageFilePath))
                {
                    var data = big.MatchTemplate(small, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);

                    var sorted = GetSortedResultItems(data.Data, tolerance);

                    return sorted;
                    //data.MinMax(out _, out double[] maxValues, out _, out Point[] maxLocations);

                    //for (int i = 0; i < maxLocations.Length; i++)
                    //{
                    //    if (maxValues[i] > tolerance)
                    //    {
                    //        var match = new Rectangle(maxLocations[i], small.Size);
                    //        result.Add(match);
                    //    }
                    //}
                }
            }
        }
    }
}
