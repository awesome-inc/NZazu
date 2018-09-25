using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Core.Internal;

namespace NZazu.Contracts.Checks
{
    public class DateTimeComparisonCheck : IValueCheck
    {
        private delegate bool Compare(DateTime value, DateTime valueToCompare);

        private readonly int _rowIdx;
        private readonly string _compareOperator;
        private readonly string _fieldToCompareWith;
        private readonly string _hint;
        private readonly Func<FormData> _formData;
        private readonly INZazuTableDataSerializer _tableSerializer;
        private readonly string _tableKey;
        private readonly IEnumerable<string> _specificDateTimeFormats;

        public DateTimeComparisonCheck(
            string hint, string compareOperator, string fieldToCompareWith, Func<FormData> formData, INZazuTableDataSerializer tableSerializer, string tableKey = null,
            IEnumerable<string> specificDateTimeFormats = null, int rowIdx = -1)
        {
            _rowIdx = rowIdx;
            _compareOperator = compareOperator ?? throw new ArgumentNullException(nameof(compareOperator));
            _fieldToCompareWith = fieldToCompareWith ?? throw new ArgumentNullException(nameof(fieldToCompareWith));
            _formData = formData ?? throw new ArgumentNullException(nameof(formData));
            _tableSerializer = tableSerializer ?? throw new ArgumentNullException(nameof(tableSerializer));
            _tableKey = tableKey;
            _specificDateTimeFormats = specificDateTimeFormats;
            _hint = hint ?? string.Empty;
        }

        public ValueCheckResult Validate(string value, object parsedValue, IFormatProvider formatProvider = null)
        {
            string valueToCompareWith;
            if (_tableKey != null && _rowIdx != -1)
            {
                valueToCompareWith = GetValueToCompareWithFromTable();
            }
            else
            {
                _formData().Values.TryGetValue(_fieldToCompareWith, out valueToCompareWith);
            }
           
            if (value.IsNullOrEmpty() || valueToCompareWith.IsNullOrEmpty()) return ValueCheckResult.Success; // do not mark not yet set values
            
            var compareOperation = GetCompareOperation();
            var dateTimeResults = _specificDateTimeFormats == null 
                ? ParseValuesToDateTime(value, valueToCompareWith) 
                : ParseValuesToDateTimeUsingSpecificFormats(value, valueToCompareWith);

            var result =  compareOperation(dateTimeResults.ValueAsDateTime, dateTimeResults.ValuteToCompareWithAsDateTime) 
                ? ValueCheckResult.Success
                : new ValueCheckResult(false, new ArgumentException(_hint));

            return result;
        }

        private string GetValueToCompareWithFromTable()
        {
            // get value to compare by idx
            _formData().Values.TryGetValue(_tableKey, out var tableData);
            var tableDict = _tableSerializer.Deserialize(tableData);

            var tableFieldToCompareWith = $"{_fieldToCompareWith}__{_rowIdx}";
            var tableFieldValueToCompareWithDoesExist =
                tableDict.TryGetValue(tableFieldToCompareWith, out var valueToCompareWith);
      
            return valueToCompareWith;
        }


        protected class DataToCompare
        {
            public DateTime ValueAsDateTime { get; }
            public DateTime ValuteToCompareWithAsDateTime { get; }

            public DataToCompare(DateTime valueAsDateTime, DateTime valuteToCompareWithAsDateTime)
            {
                ValueAsDateTime = valueAsDateTime;
                ValuteToCompareWithAsDateTime = valuteToCompareWithAsDateTime;
            }
        }

        private static DataToCompare ParseValuesToDateTime(string value, string valueToCompareWith)
        {
            DateTime.TryParse(value, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var valueDateTime);
            DateTime.TryParse(valueToCompareWith, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var valueToCompareWithDateTime);
            return new DataToCompare(valueDateTime, valueToCompareWithDateTime);
        }

        private DataToCompare ParseValuesToDateTimeUsingSpecificFormats(string value, string valueToCompareWith)
        {
            DateTime.TryParseExact(
                value,
                _specificDateTimeFormats.ToArray(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var valueDateTime);
            DateTime.TryParseExact(
                valueToCompareWith,
                _specificDateTimeFormats.ToArray(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var valueToCompareWithDateTime);
            return new DataToCompare(valueDateTime, valueToCompareWithDateTime);
        }

        private Compare GetCompareOperation()
        {
            switch (_compareOperator)
            {
                case "<=": return (value, compare) => value <= compare;
                case "<": return (value, compare) => value < compare;
                case ">=": return (value, compare) => value >= compare;
                case ">": return (value, compare) => value > compare;
                case "=": return (value, compare) => value == compare;
                default: throw new NotSupportedException("Operator not specified!");
            }
        }
    }
}