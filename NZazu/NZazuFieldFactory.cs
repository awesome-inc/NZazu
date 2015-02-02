using System;
using NZazu.Contracts;

namespace NZazu
{
    class NZazuFieldFactory : INZazuFieldFactory
    {
        public INZazuField CreateField(FieldDefinition fieldDefinition)
        {
            if (fieldDefinition == null) throw new ArgumentNullException("fieldDefinition");
            switch (fieldDefinition.Type)
            {
                case "label": return new NZazuLabelField(fieldDefinition.Key, fieldDefinition.Prompt, fieldDefinition.Description);
                case "string": return new NZazuTextField(fieldDefinition.Key, fieldDefinition.Prompt, fieldDefinition.Description);
                case "bool": return new NZazuBoolField(fieldDefinition.Key, fieldDefinition.Prompt, fieldDefinition.Description);
                default: throw new NotSupportedException("The specified field type is not supported: " + fieldDefinition.Type);
            }
        }
    }
}