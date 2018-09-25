using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using NZazu.Contracts.Checks;
using NZazu.Contracts.FormChecks;

namespace NZazu.Contracts
{
    public class CheckFactory : ICheckFactory
    {
        public IValueCheck CreateCheck(CheckDefinition checkDefinition, Func<FormData> formData = null,
            INZazuTableDataSerializer tableSerializer = null, int rowIdx = -1)
        {
            if (checkDefinition == null) throw new ArgumentNullException(nameof(checkDefinition));
            if (string.IsNullOrWhiteSpace(checkDefinition.Type)) throw new ArgumentException("check type not specified");
            switch (checkDefinition.Type)
            {
                case "required": return new RequiredCheck();
                case "length": return CreateLengthCheck(checkDefinition.Values);
                case "range": return CreateRangeCheck(checkDefinition.Values);
                case "regex": return CreateRegexCheck(checkDefinition.Values);
                case "dateTime": return CreateDateTimeComparisonCheck(checkDefinition.Values, formData, tableSerializer, rowIdx);
                default: throw new NotSupportedException("The specified check is not supported");
            }
        }

        private static IValueCheck CreateLengthCheck(IList<string> values)
        {
            if (values == null || values.Count < 1) throw new ArgumentException("At least a minimum string length must be specified");
            if (values.Count > 2) throw new ArgumentException("At most minimum and maximum string length can be specified");

            if (values.Count == 1)
                return new StringLengthCheck(int.Parse(values[0]));
            return new StringLengthCheck(int.Parse(values[0]), int.Parse(values[1]));
        }

        private static IValueCheck CreateRangeCheck(IList<string> values)
        {
            if (values == null || values.Count != 2) throw new ArgumentException("Must sepcify minimum and maximum");
            return new RangeCheck(double.Parse(values[0], CultureInfo.InvariantCulture), double.Parse(values[1], CultureInfo.InvariantCulture));
        }

        private static IValueCheck CreateRegexCheck(IList<string> values)
        {
            if (values == null || values.Count < 2) throw new ArgumentException("At least a hint and one regex pattern must be specified");
            var rx = values.Skip(1).Select(pattern => new Regex(pattern)).ToArray();
            return new StringRegExCheck(values[0], rx);
        }

        private static IValueCheck CreateDateTimeComparisonCheck(
            IList<string> values, Func<FormData> formData, INZazuTableDataSerializer tableSerializer, int rowIdx)
        {
            if (values == null || values.Count < 3) throw new ArgumentException("Hint, comparison operator and field id to compare values needs to be specified");
            if (values[1] != "<=" && values[1] != ">=" && values[1] != "=" && values[1] != "<" && values[1] != ">") throw new ArgumentException("Only <=, >=, =, < and > are supported operators!");
            var optionalDateFormat = values.Count >= 4 ? values[3].Split('|') : null;
            var tableId = values.Count >= 5 && values[4] != string.Empty? values[4] : null;
            
            return new DateTimeComparisonCheck(values[0], values[1], values[2], formData, tableSerializer, tableId, optionalDateFormat, rowIdx);
        }

        public IFormCheck CreateFormCheck(CheckDefinition checkDefinition)
        {
            if (checkDefinition == null) throw new ArgumentNullException(nameof(checkDefinition));
            if (string.IsNullOrWhiteSpace(checkDefinition.Type)) throw new ArgumentException("form check type not specified");
            switch (checkDefinition.Type)
            {
                case "gt": return CreateGreaterThanFormCheck(checkDefinition.Values);
                default: throw new NotSupportedException("The specified check is not supported");
            }
        }

        private static IFormCheck CreateGreaterThanFormCheck(IList<string> values)
        {
            if (values == null || values.Count < 3) throw new ArgumentException("Hint source and target field needs to be specified");
            if (values.Count > 3) throw new ArgumentException("Only hint source and target field needs to be specified.");
            return new GreaterThanFormCheck(values[0],values[1],values[2]);
        }

    }
}