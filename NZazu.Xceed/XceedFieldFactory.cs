using NZazu.Contracts;
using NZazu.Factory;

namespace NZazu.Xceed
{
    public class XceedFieldFactory : NZazuFieldFactory
    {
        public XceedFieldFactory(ICheckFactory checkFactory = null) 
            : base(checkFactory)
        {
            FieldTypes["string"] = typeof(XceedTextBoxField);
            FieldTypes["date"] = typeof(XceedDateTimeField);
            FieldTypes["double"] = typeof(XceedDoubleField);
            FieldTypes["int"] = typeof(XceedIntegerField);
        }
    }
}
