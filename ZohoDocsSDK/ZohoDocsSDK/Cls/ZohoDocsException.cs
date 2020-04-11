using System;

namespace ZohoDocsSDK
{
    public class ZohoDocsException : Exception
    {
        public ZohoDocsException(string errorMesage, int errorCode) : base(errorMesage) { }
    }
}
