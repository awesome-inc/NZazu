using System;
using System.Collections.Generic;
using System.Globalization;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    // ReSharper disable once UnusedMember.Global
    public class ReadOnlySample : SampleBase
    {
        public ReadOnlySample() : base(70)
        {
            Sample = new SampleViewModel
            {
                Name = "ReadOnly",
                Fiddle = ToFiddle(new FormDefinition
                    {
                        Fields = new[]
                        {
                            new FieldDefinition {Key = "1", Type = "label", Prompt = "1", Description = "2"},
                            new FieldDefinition {Key = "2", Type = "string", Prompt = "2"},
                            new FieldDefinition {Key = "3", Type = "date", Prompt = "3"},
                            new FieldDefinition {Key = "4", Type = "bool", Prompt = "4"},
                            new FieldDefinition {Key = "5", Type = "double", Prompt = "5"},
                            new FieldDefinition {Key = "6", Type = "int", Prompt = "6"}
                        }
                    },
                    new Dictionary<string, string>
                    {
                        {"2", "Some text"},
                        {"3", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)},
                        {"4", "False"},
                        {"5", "3.1415"},
                        {"6", "42"}
                    })
            };

            Sample.Fiddle.Preview.IsReadOnly = true;
        }
    }
}