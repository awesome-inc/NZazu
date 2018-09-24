using System;
using NEdifis.Attributes;

namespace NZazu.Xceed
{
    [ExcludeFromConventions("trivial")]
    internal class NowDateTimeProvider : IActualDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}