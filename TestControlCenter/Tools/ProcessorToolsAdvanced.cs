using Accord.Imaging;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TestControlCenterDomain;

namespace TestControlCenter.Tools
{
    public class ProcessorToolsAdvanced : IProcessorTools
    {
        public List<Rectangle> SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance)
        {   
            var tm = new ExhaustiveTemplateMatching(0.95f);

            var matchings = tm.ProcessImage(bigBmp, smallBmp);

            return matchings.Select(x => x.Rectangle).ToList();
        }

        public List<Rectangle> SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance, Rectangle searchArea)
        {
            var tm = new ExhaustiveTemplateMatching(0.95f);

            var matchings = tm.ProcessImage(bigBmp, smallBmp, searchArea);

            return matchings.Select(x => x.Rectangle).ToList();
        }
    }
}
