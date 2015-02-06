using NZazu.Contracts;
using NZazu.Factory;

namespace NZazu.Xceed
{
    public class NZazuFieldFactoryXceed : NZazuFieldFactory
    {
        public NZazuFieldFactoryXceed(ICheckFactory checkFactory = null) 
            : base(checkFactory)
        {
            FieldTypes["string"] = typeof(NZazuWatermarkTextBoxField);
            
        }
    }
}
