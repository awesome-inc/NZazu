using System.Collections.Generic;
using NZazu.Contracts;

namespace NZazuFiddle.Samples
{
    internal class DynamicRowLayout : SampleBase
    {
        public DynamicRowLayout() : base(-10)
        {
            Sample = new SampleViewModel
            {
                Name = "Radio Transmission Incident",
                Description = "Test for a dynamic table with columns defined as fields",
                Fiddle = ToFiddle(new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key="group_activity",
                            Type="group",
                            Description = "Activity",
                            Layout = "stack",
                            Fields = new []
                            {
                                new FieldDefinition
                                {
                                    Key = "group_activity_stack_left",
                                    Type = "group",
                                    Settings = new Dictionary<string, string>() { {"Width","400" } },
                                    Fields = new []
                                    {
                                        new FieldDefinition
                                        {
                                            Key = "dtg",
                                            Type = "date",
                                            Prompt = "DTG",
                                            Description = "Date as date time geoup because it is easier to read",
                                            Settings = new Dictionary<string, string>
                                            {
                                                {"Format", "ddHHmm\\ZMMMyy" }
                                            }
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "sensor",
                                            Type = "string",
                                            Prompt = "Sensor",
                                            Description = "Which sensor had a warnung",
                                            Checks = new []
                                            {
                                                new CheckDefinition
                                                {
                                                    Type = "required"
                                                }
                                            }
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "transmission_mode",
                                            Type = "option",
                                            Values = new []{"Low", "High","Vertical", "Horizontal"},
                                            Prompt = "Transmission Mode",
                                            Description = "Sensor for the signal",
                                        },
                                    }
                                },
                                new FieldDefinition
                                {
                                    Key = "group_activity_stack_right",
                                    Settings = new Dictionary<string, string>() { {"Width","300" } },
                                    Type = "group",
                                    Fields = new []
                                    {
                                        new FieldDefinition
                                        {
                                            Key = "frequency",
                                            Type = "double",
                                            Prompt = "Frequency",
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "bandwidth",
                                            Type = "double",
                                            Prompt = "Bandwidth",
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "email",
                                            Type = "string",
                                            Prompt = "Email",
                                            Description = "Email address",
                                            Checks = new []
                                            {
                                                new CheckDefinition
                                                {
                                                    Type = "required"
                                                },
                                                new CheckDefinition
                                                {
                                                    Type = "regex",
                                                    Values = new  []{"Must be a valid e-mail address","^.+@.+\\..+$"}
                                                }
                                            }
                                        },
                                    }
                                }
                            }
                        },
                        new FieldDefinition
                        {
                            Key="group_modeparameter",
                            Type="group",
                            Description = "Transmission Mode Parameteres",
                            Fields = new []
                            {
                                new FieldDefinition
                                {
                                    Key = "codetable",
                                    Type = "string",
                                    Prompt = "Code Table",
                                    Settings = new Dictionary<string, string>() { {"Width", "300" } }
                                },
                                new FieldDefinition
                                {
                                    Key = "modulationsubcarrier",
                                    Type = "string",
                                    Prompt = "Modulation Subcarrier",
                                    Settings = new Dictionary<string, string>() { {"Width", "300" } }
                                },
                                new FieldDefinition
                                {
                                    Key = "modeparameter_image",
                                    Type = "imageViewer",
                                    Prompt = "Image",
                                }
                            }
                        },
                        new FieldDefinition
                        {
                            Key="group_locatings",
                            Type="group",
                            Description = "Locatings",
                            Layout = "stack",
                            Fields = new []
                            {
                                new FieldDefinition
                                {
                                    Key = "table_locatings",
                                    Type = "datatable",
                                    Fields = new []
                                    {
                                        new FieldDefinition
                                        {
                                            Key = "table_locatings_icon",
                                            Type = "imageViewer",
                                            Values = new []
                                            {
                                                @"http://cliparts.co/cliparts/Acb/rpK/AcbrpKzxi.png",
                                                @"http://cliparts.co/cliparts/di9/rkK/di9rkK7nT.png",
                                                @"http://cliparts.co/cliparts/rTn/rpK/rTnrpK76c.png"
                                            },
                                            Prompt = "Icon",
                                            Settings = new Dictionary<string, string>
                                            {
                                                {"Width", "24" },
                                                {"AllowCustomValues" , "false" },
                                                {"AllowNullValues" , "true" },
                                            }
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_locatings_time",
                                            Type = "date",
                                            Prompt = "Time"
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_locatings_lat",
                                            Type = "double",
                                            Prompt = "Latitude"
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_locatings_lon",
                                            Type = "double",
                                            Prompt = "Longitude"
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_locatings_bearing",
                                            Type = "double",
                                            Prompt = "Bearing"
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_locatings_maj",
                                            Type = "double",
                                            Prompt = "Major"
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_locatings_min",
                                            Type = "double",
                                            Prompt = "Min"
                                        },
                                    }
                                }
                            }
                        },
                        new FieldDefinition
                        {
                            Key="group_bearings",
                            Type="group",
                            Description = "Bearings",
                            Layout = "stack",
                            Fields = new []
                            {
                                new FieldDefinition
                                {
                                    Key = "table_bearings",
                                    Type = "datatable",
                                    Fields = new []
                                    {
                                        new FieldDefinition
                                        {
                                            Key = "table_bearings_icon",
                                            Type = "option",
                                            Prompt = "Icon",
                                            Settings = new Dictionary<string, string> { {"Width", "24" } },
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_bearings_time",
                                            Type = "date",
                                            Prompt = "Time"
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_bearings_bearing",
                                            Type = "double",
                                            Prompt = "Bearing",
                                            Checks = new []
                                                {
                                                    new CheckDefinition
                                                    {
                                                        Type = "required"
                                                    }
                                                }
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_bearings_quality",
                                            Type = "double",
                                            Prompt = "Quality"
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_bearings_lat",
                                            Type = "double",
                                            Prompt = "Latitude",
                                            Checks = new []
                                                {
                                                    new CheckDefinition
                                                    {
                                                        Type = "required"
                                                    }
                                                }
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_bearings_lon",
                                            Type = "double",
                                            Prompt = "Longitude",
                                            Checks = new []
                                                {
                                                    new CheckDefinition
                                                    {
                                                        Type = "required"
                                                    }
                                                }
                                        },
                                    }
                                }
                            }
                        },
                        new FieldDefinition
                        {
                            Key="group_transcript",
                            Type="group",
                            Description = "Transcript",
                            Layout = "stack",
                            Fields = new []
                            {
                                new FieldDefinition
                                {
                                    Key = "table_transcript",
                                    Type = "datatable",
                                    Fields = new[]
                                    {
                                        new FieldDefinition
                                        {
                                            Key = "table_transcript_time",
                                            Type = "date",
                                            Prompt = "Time"
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_transcript_icon",
                                            Type = "option",
                                            Prompt = "Icon",
                                            Settings = new Dictionary<string, string> { {"Width", "24" } }
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_transcript_to",
                                            Type = "string",
                                            Prompt = "To",
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_transcript_from",
                                            Type = "string",
                                            Prompt = "From",
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_transcript_gist",
                                            Type = "string",
                                            Prompt = "Gist",
                                            Behavior = new BehaviorDefinition
                                            {
                                                Name = "OpenUrlOnStringEnter"
                                            }
                                        },
                                        new FieldDefinition
                                        {
                                            Key = "table_transcript_comment",
                                            Type = "string",
                                            Prompt = "Comments",
                                        },
                                    }
                                }
                            }
                        },
                    }
                },
                new FormData(new Dictionary<string, string>
                    {
                        { "modeparameter_image", "http://www.clipartlord.com/wp-content/uploads/2014/11/tank11.png" }
                    })
                )
            };
        }
    }
}