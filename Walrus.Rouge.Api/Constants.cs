namespace Walrus.Rouge.Api
{
    public class Constants
    {
        public const string _failureType = "Walrus:RougeApi:FailureType";
        public const string _failureCount = "Walrus:RougeApi:FailureCount";
        public enum FailureTypes
        {
            ShortBursts = 1,
            LongTime
        }
    }
}
