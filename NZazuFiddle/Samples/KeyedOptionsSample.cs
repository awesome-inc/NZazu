using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    // ReSharper disable once UnusedMember.Global
    internal class KeyedOptionsSample : SampleBase
    {
        public KeyedOptionsSample() : base(20)
        {
            Sample = new SampleViewModel
            {
                Name = "Keyed Option",
                Description = "",
                Fiddle = ToFiddle(new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "storekey0101",
                            Type = "keyedoption",
                            Prompt = "Store 1 - 1",
                            Settings = new Dictionary<string, string> {{"storekey", "store1"}}
                        },
                        new FieldDefinition
                        {
                            Key = "storekey0102",
                            Type = "keyedoption",
                            Prompt = "Store 1 - 2",
                            Settings = new Dictionary<string, string> {{"storekey", "store1"}}
                        },
                        new FieldDefinition
                        {
                            Key = "storekey0103",
                            Type = "keyedoption",
                            Prompt = "Store 1 - 3",
                            Settings = new Dictionary<string, string> {{"storekey", "store1"}}
                        },
                        new FieldDefinition
                        {
                            Key = "storekey0201",
                            Type = "keyedoption",
                            Prompt = "Store 2 - 1",
                            Settings = new Dictionary<string, string> {{"storekey", "store2"}}
                        },
                        new FieldDefinition
                        {
                            Key = "storekey020",
                            Type = "keyedoption",
                            Prompt = "Store 2 - 2",
                            Settings = new Dictionary<string, string> {{"storekey", "store2"}}
                        },
                        new FieldDefinition
                        {
                            Type = "datatable",
                            Key = "keyedtable",
                            Fields = new []
                            {
                                new FieldDefinition
                                {
                                    Key = "storekey0104",
                                    Type = "keyedoption",
                                    Prompt = "Store 1 - 4",
                                    Settings = new Dictionary<string, string> {{"storekey", "store1"}}
                                },
                                new FieldDefinition
                                {
                                    Key = "storekey0203",
                                    Type = "keyedoption",
                                    Prompt = "Store 2 - 3",
                                    Settings = new Dictionary<string, string> {{"storekey", "store2"}}
                                },
                            }
                        }, 
                    }
                },
                new FormData(new Dictionary<string, string>() { { "storekey0103", "Horst" } }))
            };
        }
    }
}