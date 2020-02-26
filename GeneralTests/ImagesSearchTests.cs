using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneralTests
{
    [TestClass]
    public class ImagesSearchTests
    {
        [TestMethod]
        public void SearchImageInsideImage()
        {
            var source = @"C:\Users\Milad\Desktop\tests\1\s1.jpg";
            var wanted = @"C:\Users\Milad\Desktop\tests\1\c3.jpg";

            //var image = new Image<Gray, byte>(source);
            //var innerImage = new Image<Gray, byte>(wanted);

            //var result = image.FindCornerSubPix(innerImage, Emgu.CV.CvEnum.TemplateMatchingType.Sqdiff);

            //ExhaustiveTemplateMatching

            var image = new Bitmap(source);
            var innerImage = new Bitmap(wanted);

            var data = Finder2.ProcessStep.SearchBitmap(innerImage, image, .95);

            Assert.IsTrue(data != default);
        }
    }
}
