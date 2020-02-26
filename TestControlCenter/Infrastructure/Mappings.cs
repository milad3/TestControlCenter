using AutoMapper;
using TestControlCenterDomain;
using TestControlCenter.Models;

namespace TestControlCenter.Infrastructure
{
    public class Mappings
    {
        public static MapperConfiguration TestItemMapperConfiguration { get; private set; }

        public static MapperConfiguration TestItemQuestionMapperConfiguration { get; private set; }

        public static void AutoMapperConfig()
        {
            TestItemMapperConfiguration = new MapperConfiguration(x => x.CreateMap<TestItem, TestItemViewModel>());

            TestItemQuestionMapperConfiguration = new MapperConfiguration(x => x.CreateMap<TestItemQuestion, TestItemQuestionViewModel>());
        }
    }
}
