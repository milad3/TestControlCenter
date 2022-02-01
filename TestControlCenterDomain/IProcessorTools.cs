using System.Collections.Generic;
using System.Drawing;

namespace TestControlCenterDomain
{
    public interface IProcessorTools
    {
        List<ImageSearchResultItem> SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance);

        List<ImageSearchResultItem> SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance, Rectangle searchArea);

        List<ImageSearchResultItem> SearchImage(string smallImageFilePath, string bigImageFilePath, double tolerance);
    }
}
