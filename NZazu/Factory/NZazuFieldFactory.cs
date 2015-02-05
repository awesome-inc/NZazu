using System;
using System.Diagnostics;
using NZazu.Contracts;
using NZazu.Fields;

namespace NZazu.Factory
{
    class NZazuFieldFactory : INZazuFieldFactory
    {
        public INZazuField CreateField(FieldDefinition fieldDefinition)
        {
            if (fieldDefinition == null) throw new ArgumentNullException("fieldDefinition");

            switch (fieldDefinition.Type)
            {
                case "string": return Decorate(new NZazuTextField(fieldDefinition.Key), fieldDefinition);
                case "bool": return Decorate(new NZazuBoolField(fieldDefinition.Key), fieldDefinition);
                case "label":
                default:
                    var field = Decorate(new NZazuLabelField(fieldDefinition.Key), fieldDefinition);
                    if (fieldDefinition.Type != "label")
                        //throw new NotSupportedException("The specified field type is not supported: " + fieldDefinition.Type);
                        Trace.TraceWarning("The specified field type is not supported: " + fieldDefinition.Type);
                    return field;
            }
        }

        private static NZazuField Decorate(NZazuField field, FieldDefinition fieldDefinition)
        {
            field.Prompt = fieldDefinition.Prompt;
            field.Hint = fieldDefinition.Hint;
            field.Description = fieldDefinition.Description;
            field.Checks = fieldDefinition.Checks;
            return field;
        }
    }
}