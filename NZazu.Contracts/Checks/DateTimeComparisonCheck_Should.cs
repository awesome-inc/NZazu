using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using FluentAssertions;
using NEdifis;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof(DateTimeComparisonCheck))]
    // ReSharper disable InconsistentNaming
    internal class DateTimeComparisonCheck_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"}
            } as IDictionary<string, string>;
            var formData = new FormData();

            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);

            var sut = ctx.BuildSut();

            sut.Should().NotBeNull();
            sut.GetType().GetCustomAttribute<DisplayNameAttribute>().DisplayName.Should().Be("datetime");
        }

        [Test]
        public void Registered_At_CheckFactory()
        {
            var settings = new Dictionary<string, string> {{"Min", "2"}, {"Max", "6"}} as IDictionary<string, string>;
            var checkType = typeof(RangeCheck);

            var sut = new CheckFactory();

            var checkDefinition = new CheckDefinition {Type = "range", Settings = settings};
            var check = sut.CreateCheck(checkDefinition, new FieldDefinition {Key = "key1"});

            check.Should().NotBeNull();
            check.Should().BeOfType(checkType);
        }

        [Test]
        public void Return_False_For_End_time_Before_Start_time_Using_Greater_Than()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "11/7/2018"},
                {"stopTime", "9/7/2018"}
            };

            var formData = new FormData(testDict);

            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            formData.Values.TryGetValue("stopTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().BeFalse();
        }

        [Test]
        public void Return_True_For_Start_time_Before_End_time_Using_Greater_Than()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "8/7/2018"},
                {"stopTime", "9/7/2018"}
            };

            var formData = new FormData(testDict);

            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"},
                {"SpecificDateTimeFormats", "d/M/yyyy"}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            // we compare 'startTime' as base with 'stopTime' as remove field
            formData.Values.TryGetValue("startTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().BeTrue();
            res.Exception.Should().BeNull();
        }

        [Test]
        public void Return_False_For_End_time_Before_Start_time_Using_Greater_Than_With_Specific_Formats()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "1300"},
                {"stopTime", "11:00"}
            };

            var formData = new FormData(testDict);
            var testFormats = new[] {"HHmm", "HHmmss", "HH:mm", "HH:mm:ss"};

            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"},
                {"SpecificDateTimeFormats", string.Join("|", testFormats)}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            formData.Values.TryGetValue("stopTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().BeFalse();
        }

        [Test]
        public void Return_True_For_Start_time_Before_End_time_Using_Greater_Than_With_Specific_Formats()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "11:00"},
                {"stopTime", "11:30"}
            };

            var formData = new FormData(testDict);
            var testFormats = new[] {"HHmm", "HHmmss", "HH:mm", "HH:mm:ss"};

            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"},
                {"SpecificDateTimeFormats", string.Join("|", testFormats)}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            formData.Values.TryGetValue("startTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().BeTrue();
        }

        [Test]
        public void Return_False_For_End_time_Before_Start_time_Using_Smaller_Than()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "11/7/2018"},
                {"stopTime", "9/7/2018"}
            };

            var formData = new FormData(testDict);

            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            formData.Values.TryGetValue("startTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().BeFalse();
        }

        [Test]
        [TestCase("11/7/2018", "9/7/2018", "<", false)]
        [TestCase("6/7/2018", "9/7/2018", "<", true)]
        [TestCase("9/7/2018", "7/7/2018", "=", false)]
        [TestCase("9/7/2018", "9/7/2018", "=", true)]
        [TestCase("9/7/2018", "7/7/2018", "==", false)]
        [TestCase("9/7/2018", "9/7/2018", "==", true)]
        [TestCase("9/7/2018", "9/7/2018", ">=", true)]
        [TestCase("9/7/2018", "6/7/2018", ">=", true)]
        [TestCase("9/7/2018", "11/7/2018", ">=", false)]
        [TestCase("9/7/2018", "9/7/2018", "<=", true)]
        [TestCase("9/7/2018", "10/7/2018", "<=", true)]
        [TestCase("9/7/2018", "1/7/2018", "<=", false)]
        public void Use_Compare_Strategy(string start, string end, string compare, bool expected)
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", start},
                {"stopTime", end}
            };

            var formData = new FormData(testDict);

            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", compare}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            formData.Values.TryGetValue("startTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().Be(expected);
        }

        [Test]
        public void Return_True_For_Start_time_Before_End_time_Using_Smaller_Than()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "8/7/2018"},
                {"stopTime", "9/7/2018"}
            };
            var formData = new FormData(testDict);
            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"},
                {"SpecificDateTimeFormats", "d/M/yyyy"}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            formData.Values.TryGetValue("startTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().BeTrue();
        }

        [Test]
        public void Return_True_Use_TableKey_And_Row_Index_For_InterTable_Comparison()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "8/7/2018"},
                {"theTable", "foobar"}
            };
            var formData = new FormData(testDict);
            var serializer = Substitute.For<INZazuTableDataSerializer>();
            serializer.Deserialize("foobar").Returns(new Dictionary<string, string> {{"stopTime__0", "9/7/2018"}});
            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"},
                {"SpecificDateTimeFormats", "d/M/yyyy"},
                {"TableKey", "theTable"}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use(serializer);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            formData.Values.TryGetValue("startTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().BeTrue();
        }

        [Test]
        public void Return_False_Use_TableKey_And_Row_Index_For_InterTable_Comparison()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "8/7/2018"},
                {"theTable", "foobar"}
            };
            var formData = new FormData(testDict);
            var serializer = Substitute.For<INZazuTableDataSerializer>();
            serializer.Deserialize("foobar").Returns(new Dictionary<string, string> {{"stopTime__0", "10/10/2018"}});
            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", ">"},
                {"SpecificDateTimeFormats", "d/M/yyyy"},
                {"TableKey", "theTable"}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use(serializer);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            formData.Values.TryGetValue("startTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().BeFalse();
        }

        [Test]
        public void Return_False_If_Cannot_Parse_Source_Date()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "cannot parse"},
                {"stopTime", "9/7/2018"}
            };
            var formData = new FormData(testDict);
            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"},
                {"SpecificDateTimeFormats", "d/M/yyyy"}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            formData.Values.TryGetValue("startTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().BeFalse();
            res.Exception.Message.Should().Be("Cannot parse source value to datetime.");
        }

        [Test]
        public void Return_False_If_Cannot_Parse_CompareWith_Date()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "9/7/2018"},
                {"stopTime", "cannot parse"}
            };
            var formData = new FormData(testDict);
            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"},
                {"SpecificDateTimeFormats", "d/M/yyyy"}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            formData.Values.TryGetValue("startTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().BeFalse();
            res.Exception.Message.Should().Be("Cannot parse 'compareWith' value to datetime.");
        }

        [Test]
        public void Return_False_For_End_time_Before_Start_time_Using_Smaller_Than_With_Specific_Formats()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "1300"},
                {"stopTime", "11:00"}
            };

            var formData = new FormData(testDict);
            var testFormats = new[] {"HHmm", "HHmmss", "HH:mm", "HH:mm:ss"};

            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"},
                {"SpecificDateTimeFormats", string.Join("|", testFormats)}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            formData.Values.TryGetValue("startTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().BeFalse();
        }

        [Test]
        public void Return_True_For_Start_time_Before_End_time_Using_Smaller_Than_With_Specific_Formats()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "11:00"},
                {"stopTime", "11:30"}
            };

            var formData = new FormData(testDict);
            var testFormats = new[] {"HHmm", "HHmmss", "HH:mm", "HH:mm:ss"};

            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"},
                {"SpecificDateTimeFormats", string.Join("|", testFormats)}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            //var dateTimeCheck = new DateTimeComparisonCheck("lorem ipsum", "<", "stopTime", () => formData, tableDataSerializer, specificDateTimeFormats: testFormats);
            formData.Values.TryGetValue("startTime", out var result);
            var res = sut.Validate(result, default(DateTime));

            res.IsValid.Should().BeTrue();
        }

        [Test]
        public void Return_True_For_Start_time_Before_End_time_Using_Smaller_Than_Within_A_Table_field()
        {
            const string tableData = "\"columnStartRow__1\":\"11:00\",\"columnStopRow__1\":\"12:00\"";
            var testDict = new Dictionary<string, string>
            {
                {"tableKey", tableData}
            };

            var formData = new FormData(testDict);
            var tableDataSerializer = Substitute.For<INZazuTableDataSerializer>();
            tableDataSerializer.Deserialize(tableData)
                .Returns(new Dictionary<string, string>
                {
                    {"columnStartRow__1", "11:00"},
                    {"columnStopRow__1", "12:00"}
                });

            var testFormats = new[] {"HHmm", "HHmmss", "HH:mm", "HH:mm:ss"};

            var settings = new Dictionary<string, string>
            {
                {"FieldToCompareWith", "stopTime"},
                {"CompareOperator", "<"},
                {"SpecificDateTimeFormats", string.Join("|", testFormats)}
            } as IDictionary<string, string>;
            var ctx = new ContextFor<DateTimeComparisonCheck>();
            ctx.Use(settings);
            ctx.Use<Func<FormData>>(() => formData);
            var sut = ctx.BuildSut();

            //var dateTimeCheck = new DateTimeComparisonCheck(
            //    "lorem ipsum", "<", "columnStopRow", () => formData, tableDataSerializer,
            //    tableKey: "tableKey", specificDateTimeFormats: testFormats, rowIdx: 1);
            var res = sut.Validate("11:00", default(DateTime));

            res.IsValid.Should().BeTrue();
        }
    }
}