using System;
using System.Collections.Generic;
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
        public void Return_False_For_Endtime_Before_Starttime_Using_Greater_Than()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "11/7/2018"},
                {"stopTime", "9/7/2018"}
            };

            var formData = new FormData(testDict);
            var tableDataSerializer = Substitute.For<INZazuTableDataSerializer>();

            var dateTimeCheck = new DateTimeComparisonCheck("lorem ipsum", ">", "startTime", () => formData, tableDataSerializer);
            formData.Values.TryGetValue("stopTime", out var result);
            var res = dateTimeCheck.Validate(result);

            res.IsValid.Should().BeFalse();
        }

        [Test]
        public void Return_True_For_Startime_Before_Endtime_Using_Greater_Than()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "8/7/2018"},
                {"stopTime", "9/7/2018"}
            };

            var formData = new FormData(testDict);
            var tableDataSerializer = Substitute.For<INZazuTableDataSerializer>();

            var dateTimeCheck = new DateTimeComparisonCheck("lorem ipsum", ">", "startTime", () => formData, tableDataSerializer);
            formData.Values.TryGetValue("stopTime", out var result);
            var res = dateTimeCheck.Validate(result);

            res.IsValid.Should().BeTrue();
        }

        [Test]
        public void Return_False_For_Endtime_Before_Starttime_Using_Greater_Than_With_Specific_Formats()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "1300"},
                {"stopTime", "11:00"}
            };

            var formData = new FormData(testDict);
            var tableDataSerializer = Substitute.For<INZazuTableDataSerializer>();
            var testFormats = new[] { "HHmm", "HHmmss", "HH:mm", "HH:mm:ss" };

            var dateTimeCheck = new DateTimeComparisonCheck("lorem ipsum", ">", "startTime", () => formData, tableDataSerializer, specificDateTimeFormats:testFormats);
            formData.Values.TryGetValue("stopTime", out var result);
            var res = dateTimeCheck.Validate(result);

            res.IsValid.Should().BeFalse();
        }

        [Test]
        public void Return_True_For_Startime_Before_Endtime_Using_Greater_Than_With_Specific_Formats()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "11:00"},
                {"stopTime", "11:30"}
            };

            var formData = new FormData(testDict);
            var tableDataSerializer = Substitute.For<INZazuTableDataSerializer>();
            var testFormats = new[] { "HHmm", "HHmmss", "HH:mm", "HH:mm:ss" };

            var dateTimeCheck = new DateTimeComparisonCheck("lorem ipsum", ">", "startTime", () => formData, tableDataSerializer, specificDateTimeFormats: testFormats);
            formData.Values.TryGetValue("stopTime", out var result);
            var res = dateTimeCheck.Validate(result);

            res.IsValid.Should().BeTrue();
        }

        [Test]
        public void Return_False_For_Endtime_Before_Starttime_Using_Smaller_Than()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "11/7/2018"},
                {"stopTime", "9/7/2018"}
            };

            var formData = new FormData(testDict);
            var tableDataSerializer = Substitute.For<INZazuTableDataSerializer>();

            var dateTimeCheck = new DateTimeComparisonCheck("lorem ipsum", "<", "stopTime", () => formData, tableDataSerializer);
            formData.Values.TryGetValue("startTime", out var result);
            var res = dateTimeCheck.Validate(result);

            res.IsValid.Should().BeFalse();
        }

        [Test]
        public void Return_True_For_Startime_Before_Endtime_Using_Smaller_Than()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "8/7/2018"},
                {"stopTime", "9/7/2018"}
            };

            var formData = new FormData(testDict);
            var tableDataSerializer = Substitute.For<INZazuTableDataSerializer>();

            var dateTimeCheck = new DateTimeComparisonCheck("lorem ipsum", "<", "stopTime", () => formData, tableDataSerializer);
            formData.Values.TryGetValue("startTime", out var result);
            var res = dateTimeCheck.Validate(result);

            res.IsValid.Should().BeTrue();
        }

        [Test]
        public void Return_False_For_Endtime_Before_Starttime_Using_Smaller_Than_With_Specific_Formats()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "1300"},
                {"stopTime", "11:00"}
            };

            var formData = new FormData(testDict);
            var tableDataSerializer = Substitute.For<INZazuTableDataSerializer>();
            var testFormats = new[] { "HHmm", "HHmmss", "HH:mm", "HH:mm:ss" };

            var dateTimeCheck = new DateTimeComparisonCheck("lorem ipsum", "<", "stopTime", () => formData, tableDataSerializer, specificDateTimeFormats: testFormats);
            formData.Values.TryGetValue("startTime", out var result);
            var res = dateTimeCheck.Validate(result);

            res.IsValid.Should().BeFalse();
        }

        [Test]
        public void Return_True_For_Startime_Before_Endtime_Using_Smaller_Than_With_Specific_Formats()
        {
            var testDict = new Dictionary<string, string>
            {
                {"startTime", "11:00"},
                {"stopTime", "11:30"}
            };

            var formData = new FormData(testDict);
            var tableDataSerializer = Substitute.For<INZazuTableDataSerializer>();
            var testFormats = new[] { "HHmm", "HHmmss", "HH:mm", "HH:mm:ss" };

            var dateTimeCheck = new DateTimeComparisonCheck("lorem ipsum", "<", "stopTime", () => formData, tableDataSerializer, specificDateTimeFormats: testFormats);
            formData.Values.TryGetValue("startTime", out var result);
            var res = dateTimeCheck.Validate(result);

            res.IsValid.Should().BeTrue();
        }

    }
}