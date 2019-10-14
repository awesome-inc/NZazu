using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    // ReSharper disable once UnusedMember.Global
    internal class FocusSample : SampleBase
    {
        public FocusSample() : base(60)
        {
            Sample = new SampleViewModel
            {
                Name = "Focus",
                Fiddle = ToFiddle(new FormDefinition
                    {
                        Fields = new[]
                        {
                            new FieldDefinition {Key = "label", Type = "label", Prompt = "Focus should be on 'value2'"},
                            new FieldDefinition {Key = "key1", Type = "string", Prompt = "key1"},
                            new FieldDefinition {Key = "key2", Type = "string", Prompt = "key2"}
                        }
                    },
                    new Dictionary<string, string>
                    {
                        {"key1", "value1"},
                        {"key2", "value2"},
                        // this sets the focus!!
                        {"__focusOn", "key2"}
                    })
            };
        }
    }
}