using System.Collections.Generic;
using System.Drawing;

namespace TestControlCenterDomain
{
    public interface IProcessorTools
    {
        List<Rectangle> SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance);

        List<Rectangle> SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance, Rectangle searchArea);
    }
}
