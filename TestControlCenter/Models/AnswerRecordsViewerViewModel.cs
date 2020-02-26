using System.Collections.Generic;

namespace TestControlCenter.Models
{
    public class ImageData
    {
        public string Address { get; set; }
    }

    public class AnswerRecordsViewerViewModel
    {
        public List<ImageData> Images { get; set; }
    }
}
