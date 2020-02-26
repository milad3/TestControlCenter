using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProccessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = @"C:\Users\Milad\Desktop\tests\1\s1.jpg";
            var wanted = @"C:\Users\Milad\Desktop\tests\1\c1.jpg";

            var image = new Image<Gray, byte>(source);
            var innerImage = new Image<Gray, byte>(wanted);

            var result = innerImage.MatchTemplate(image, Emgu.CV.CvEnum.TemplateMatchingType.CcorrNormed);
        }
    }
}
