using System;

namespace TestControlCenterDomain
{
    public enum LogMessageType
    {
        Error,
        Information,
        Warning,
        Unknown,
        Data
    }

    public class LogMessage
    {
        public long Id { get; set; }

        public DateTime DateTime { get; set; }

        public string Message { get; set; }

        public string Ref { get; set; }

        public string Details { get; set; }

        public LogMessageType Type { get; set; }

        public string Temp { get; set; }

        public string ObjectJson { get; set; }
    }
}
