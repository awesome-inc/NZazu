using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    // ReSharper disable once UnusedMember.Global
    class PrimitivesSample : SampleBase
    {
        public PrimitivesSample() : base(10)
        {
            Sample = new SampleViewModel
            {
                Name = "Primitives",
                Description = "",
                Fiddle = ToFiddle(new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "caption",
                            Type = "label",
                            Description = "A fancy caption!"
                        },
                        new FieldDefinition
                        {
                            Key = "settings",
                            Type = "label",
                            Prompt = "Settings",
                            Description = "You can manage your account here."
                        },
                        new FieldDefinition
                        {
                            Key = "name",
                            Type = "string",
                            Prompt = "Name:",
                            Hint = "Enter name",
                            Description = "Your account name. Only alpha-numeric ..."
                        },
                        new FieldDefinition
                        {
                            Key = "isAdmin",
                            Type = "bool",
                            //Prompt = "Is Admin",
                            Hint = "Is Admin",
                            Description = "Check to grant administrator permissions"
                        },
                        new FieldDefinition
                        {
                            Key = "birthday",
                            Type = "date",
                            Prompt = "Birthday:",
                            Hint = "dd-MM-yyyy",
                            Description = "Enter your birthday (dd-MM-yyyy)",
                            Settings = new Dictionary<string, string>{{"Format","dd-MM-yyyy"}}
                        },
                        new FieldDefinition
                        {
                            Key = "weight",
                            Type = "double",
                            Prompt = "Weight [kg]:",
                            Hint = "type your weight with 2 digits after comma",
                            Description = "Weight must be between 35 and 200 kg",
                            Settings = new Dictionary<string, string>
                            {
                                {"Format","#.00"}
                                // cf.: https://wpftoolkit.codeplex.com/wikipage?title=ByteUpDown&referringTitle=NumericUpDown-derived%20controls
                                ,{"Minimum","35"},{"Maximum","200"}
                                ,{"ClipValueToMinMax","True"}
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "ranking",
                            Type = "option",
                            Prompt = "Rank",
                            Values = new []{"1","2","3","4","5"},
                            Settings = new Dictionary<string, string>{{"IsEditable","True"}}
                        }
                    }
                },
                new Dictionary<string, string>
                {
                    { "name", "John" }, 
                    { "isAdmin", "true" },
                    { "birthday", "01-06-1990"},
                    { "weight", "75.5"},
                    { "ranking", "3"}
                })
            };
        }
    }
}