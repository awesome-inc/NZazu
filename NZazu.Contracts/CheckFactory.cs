using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using NZazu.Contracts.Checks;

namespace NZazu.Contracts
{
    public class CheckFactory : ICheckFactory
    {
        public IValueCheck CreateCheck(CheckDefinition checkDefinition)
        {
            if (checkDefinition == null) throw new ArgumentNullException("checkDefinition");
            if (string.IsNullOrWhiteSpace(checkDefinition.Type)) throw new ArgumentException("check type not specified");
            switch (checkDefinition.Type)
            {
                case "required": return new RequiredCheck();
                case "length": return CreateLengthCheck(checkDefinition.Values);
                case "range": return CreateRangeCheck(checkDefinition.Values);
                case "regex": return CreatRegexCheck(checkDefinition.Values);
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

        private static IValueCheck CreatRegexCheck(IList<string> values)
        {
            if (values == null || values.Count < 2) throw new ArgumentException("At least a hint and one regex pattern must be specified");
            var rx = values.Skip(1).Select(pattern => new Regex(pattern)).ToArray();
            return new StringRegExCheck(values[0], rx);
        }
    }
}