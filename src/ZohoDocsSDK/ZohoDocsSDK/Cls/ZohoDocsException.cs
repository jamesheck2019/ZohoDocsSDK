using System;

namespace ZohoDocsSDK
{
    public class ZohoDocsException : Exception
    {
        public ZohoDocsException(string errorMessage, int errorCode) : base(errorMessage) { }
    }

    public class ExceptionCls
    {
        public static ZohoDocsException CreateException(string errorMesage, int errorCode)
        {
            return new ZohoDocsException(errorMesage, errorCode);
        }
    }


}
