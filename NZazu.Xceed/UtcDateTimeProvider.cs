using System;
using NEdifis.Attributes;

namespace NZazu.Xceed
{
    [ExcludeFromConventions("trivial")]
    internal class UtcDateTimeProvider : IActualDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.UtcNow;
        }
    }
}