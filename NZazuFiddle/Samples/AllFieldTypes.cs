using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    // ReSharper disable once UnusedMember.Global
    internal class AllFieldTypes : SampleBase
    {
        public AllFieldTypes() : base(-10)
        {
            Sample = new SampleViewModel
            {
                Name = "All Available Fields",
                Description = "Awesomeness Incremented with Awesome Inc's NZazu",
                Fiddle = ToFiddle(new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "label",
                            Type = "label",
                            Description = "Caption or Heading!",
                            Settings = new Dictionary<string, string>()
                            {
                                { "FontWeight", "Bold" } ,
                                { "FontSize", "24" } ,
                                { "HorizontalAlignment", "Center" }
                            }
                        },
                        new FieldDefinition()
                        {
                            Key="group",
                            Type="group",
                            Description = "group description",
                            Prompt = "group",
                            Fields = new []{
                                new FieldDefinition
                                {
                                    Key = "group_label",
                                    Type = "label",
                                    Prompt = "Settings",
                                    Description = "You can manage your account here."
                                },
                                new FieldDefinition
                                {
                                    Key = "group_name",
                                    Type = "string",
                                    Prompt = "group name",
                                    Hint = "Enter group name",
                                    Description = "Your account name. Only alpha-numeric ..."
                                },
                            }
                        },
                        new FieldDefinition()
                        {
                            Key="datatable",
                            Type="datatable",
                            Description = "datatable description",
                            Prompt = "datatable",
                            Fields = new []{
                                new FieldDefinition
                                {
                                    Key = "datatable_label",
                                    Type = "label",
                                    Prompt = "Settings",
                                    Description = "who are you"
                                },
                                new FieldDefinition
                                {
                                    Key = "datatable_name",
                                    Type = "string",
                                    Prompt = "datatable name",
                                    Hint = "Enter table name",
                                    Description = "your name"
                                },
                                new FieldDefinition
                                {
                                    Key = "datatable_location",
                                    Type = "location",
                                    Prompt = "datatable location",
                                    Hint = "50.8 7.2",
                                    Description = "where are you born"
                                },
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "string",
                            Type = "string",
                            Prompt = "string",
                            Hint = "Enter a single line string",
                            Description = "Single line string with whatever you want to"
                        },
                        new FieldDefinition
                        {
                            Key = "location",
                            Type = "location",
                            Prompt = "location",
                            Description = "Select where you are",
                            Hint = "50.8 7.2"
                        },
                        new FieldDefinition
                        {
                            Key = "autocompleteV",
                            Type = "autocomplete",
                            Prompt = "autocomplete from values",
                            Hint = "Enter 'a' and wait for autocomplete",
                            Description = "Enter a single line string and get suggestion from any source using a provider",
                            Settings = new Dictionary<string, string>
                            {
                                {"dataconnection", "value://anton|adam|abraham|anna|annika|astrid"}
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "autocompleteF",
                            Type = "autocomplete",
                            Prompt = "autocomplete from file",
                            Hint = "Enter 'b' and wait for autocomplete",
                            Description = "Enter a single line string and get suggestion from any source using a provider",
                            Settings = new Dictionary<string, string>
                            {
                                {"dataconnection", "file://./cities.txt"}
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "autocompleteE",
                            Type = "autocomplete",
                            Prompt = "autocomplete from elasticsearch",
                            Hint = "Fill data in elasticsearch on localhost and add data",
                            Description = "Enter a single line string and get suggestion from any source using a provider",
                            Settings = new Dictionary<string, string>
                            {
                                {"dataconnection", "elasticsearch://nzazu/autocomplete|firstname,lastname"}
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "int",
                            Type = "int",
                            Prompt = "int",
                            Hint = "Enter a number",
                            Description = "Some number you would like to enter"
                        },
                        new FieldDefinition
                        {
                            Key = "bool",
                            Type = "bool",
                            Prompt = "bool",
                            Hint = "check if you want me to be true",
                            Description = "I know for certain it is 'true' or 'false'"
                        },
                        new FieldDefinition
                        {
                            Key = "bool_tristate",
                            Type = "bool",
                            Prompt = "bool",
                            Hint = "check if you want me to be true or false or dont-know",
                            Description = "I know it is 'true' or 'false' or I don't know",
                            Settings = new Dictionary<string, string>()
                            {
                                { "IsThreeState", "True"}
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "date",
                            Type = "date",
                            Prompt = "date",
                            Description = "enter a date with a custom format",
                            Hint = "dd-MM-yyyy",
                            Settings = new Dictionary<string, string>
                            {
                                {"Format","dd-MM-yyyy"}
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "double",
                            Type = "double",
                            Prompt = "double",
                            Hint = "type your weight with 2 digits after comma",
                            Description = "Weight must be between 35 and 200 kg",
                            Settings = new Dictionary<string, string>
                            {
                                { "Format" , "#.00" },
                                { "Minimum" , "35" },
                                { "Maximum" , "200" },
                                { "ClipValueToMinMax" , "True" }
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "option",
                            Type = "option",
                            Prompt = "option",
                            Values = new []{ "Value 1", "Value 2", "Value 3", "Value 4", "Value 5"},
                            Settings = new Dictionary<string, string>
                            {
                                { "IsEditable" , "True" }
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "option_fixed",
                            Type = "option",
                            Prompt = "option",
                            Values = new []{ "Value 1", "Value 2", "Value 3", "Value 4", "Value 5"},
                            Settings = new Dictionary<string, string>
                            {
                                { "IsEditable" , "False" }
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "keyedoption1",
                            Type = "keyedoption",
                            Prompt = "keyedoption",
                            Description = "keyedoption share the entered values, but no value list is allowed",
                            Settings = new Dictionary<string, string>
                            {
                                {"storekey", "store1"}
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "keyedoption2",
                            Type = "keyedoption",
                            Prompt = "keyedoption",
                            Description = "keyedoption share the entered values, but no value list is allowed",
                            Settings = new Dictionary<string, string>
                            {
                                {"storekey", "store1"}
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "keyedoption3",
                            Type = "keyedoption",
                            Prompt = "keyedoption",
                            Description = "keyedoption share the entered values, but no value list is allowed",
                            Settings = new Dictionary<string, string>
                            {
                                {"storekey", "store1"},
                                { "IsEditable" , "False" }
                            }
                        },
                        new FieldDefinition
                        {
                            Key = "imageViewer",
                            Type = "imageViewer",
                            Prompt = "imageViewer",
                            Hint = "shows an image",
                            Description = "use mouse click or space to toggle images. a single value just displays the image",
                            Values = new []
                            {
                                "https://upload.wikimedia.org/wikipedia/commons/5/57/Gemmules_a1.jpg",
                                "https://upload.wikimedia.org/wikipedia/commons/4/4c/Gemmules_a2.jpg",
                            },
                            Settings = new Dictionary<string, string>
                            {
                                { "Width", "200" },
                                { "Height", "120" }
                            }
                        },
                    }
                },
                new Dictionary<string, string>
                {
                    { "string", "John" },
                    { "location", "53.2 7.3" },
                    { "date", "20-02-1980"},
                    { "double", "75.5"},
                    { "int", "3"}
                })
            };
        }
    }
}