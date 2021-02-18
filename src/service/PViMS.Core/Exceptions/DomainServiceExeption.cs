using System;

namespace PVIMS.Core.Exceptions
{
    public class DomainServiceException : Exception
    {
        public DomainServiceException(string key, string message)
            :base(message)
        {
            this.Key = key;
        }

        public string Key { get; private set; }
    }
}
