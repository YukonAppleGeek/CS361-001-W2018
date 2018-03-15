using System;

namespace StudyUp.Tests
{
    public class TestClock : IClock
    {
        private static DateTime dateTime = new DateTime(2018, 6, 1);
        public DateTime Now => dateTime;
        public static DateTime StaticNow => dateTime;
    }
}
