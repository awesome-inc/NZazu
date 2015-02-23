using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.CodeCompletion;
using NZazu.FieldBehavior;

namespace NZazuFiddle
{
    static class NZazuAutoCompletion
    {
        public static void AutoCompleteFor(this ICollection<ICompletionData> data, string word)
        {
            switch (word)
            {
                case "Type\":": Types.ForEach(data.Add); break; 
                case "Key\":": data.Add(KeyComplete); break;
                case "Prompt\":": data.Add(PromptComplete); break;
                case "Hint\":": data.Add(HintComplete); break;
                case "Description\":": data.Add(DescriptionComplete); break;
                case "Checks\":": Checks.ForEach(data.Add); break;
                case "Settings\":": Settings.ForEach(data.Add); break;
                case "Format\":": Format.ForEach(data.Add); break;
                //case "Name\":": GetNames().ForEach(data.Add); break;
                case "Behavior\":": GetBehaviors().ForEach(data.Add); break;
            }
        }

        private static List<NzazuCompletionData> GetBehaviors()
        {
            var behaviors = BehaviorExtender.GetBehaviors();
            return
                behaviors.Select(kvp => new NzazuCompletionData
                {
                    Text = kvp.Key, 
                    Replacement = String.Format("{{ \"Name\": \"{0}\"}}", kvp.Key),
                    Description = String.Format("Add behavior '{0}'", kvp.Key)
                }).ToList();
        }

        private static readonly List<ICompletionData> Types = new List<ICompletionData>(new[]
        {
            // fields
            new NzazuCompletionData { Text = "label", Replacement = "\"label\"", Description = "Adds a label"},
            new NzazuCompletionData { Text = "string", Replacement = "\"string\"", Description = "Adds a string field"},
            new NzazuCompletionData { Text = "bool", Replacement = "\"bool\"", Description = "Adds a boolean field"},
            new NzazuCompletionData { Text = "date", Replacement = "\"date\"", Description = "Adds a date/time field"},
            new NzazuCompletionData { Text = "int", Replacement = "\"int\"", Description = "Adds an integer field"},
            new NzazuCompletionData { Text = "double", Replacement = "\"double\"", Description = "Adds a double field"},
            new NzazuCompletionData { Text = "option", Replacement = "\"option\"", Description = "Adds an option field (choose from Values)"},
            new NzazuCompletionData { Text = "richtext", Replacement = "\"richtext\"", Description = "Adds a richt-text field"},
            new NzazuCompletionData { Text = "group", Replacement = "\"richtext\"", Description = "Adds a group field"},

            // checks
            new NzazuCompletionData { Text = "required", Replacement = "\"required\"", Description = "Adds a 'required' check"},
            new NzazuCompletionData { Text = "length", Replacement = "\"length\"", Description = "Adds a string length check"},
            new NzazuCompletionData { Text = "range", Replacement = "\"range\"", Description = "Adds a range check"},
            new NzazuCompletionData { Text = "regex", Replacement = "\"regex\"", Description = "Adds a regex checks"}
        });

        private static readonly List<ICompletionData> Checks = new List<ICompletionData>(new[]
        {
            new NzazuCompletionData { Text = "required", Replacement = "[{ \"Type\": \"required\" }]", Description = "The field is required"},
            new NzazuCompletionData { Text = "length", Replacement = "[{ \"Type\": \"length\", \"Values\": [\"6\", \"8\" ] }]", Description = "The field length must be in the specified range"},
            new NzazuCompletionData { Text = "range", Replacement = "[{ \"Type\": \"range\", \"Values\": [\"-50.0\", \"+50.0\" ] }]", Description = "The field value must be in the specified range"},
            new NzazuCompletionData { Text = "regex", Replacement = "[{ \"Type\": \"regex\", \"Values\": [\"<Hint>\", \"pattern1\", \"pattern2\" ] }]", Description = "The field value must match one of the specified patterns"}
        });

        private static readonly List<ICompletionData> Settings = new List<ICompletionData>(new[]
        {
            new NzazuCompletionData { Text = "Format", Replacement = "{ \"Format\": \"\" }", Description = "The field format"},
            new NzazuCompletionData { Text = "Height", Replacement = "{ \"Height\": \"64.3\" }", Description = "The field's height in pixel"},
            new NzazuCompletionData { Text = "Width", Replacement = "{ \"Width\": \"64.3\" }", Description = "The field's width in pixel"},
            new NzazuCompletionData { Text = "ShowFormatBar", Replacement = "{ \"ShowFormatBar\": \"True\" }", Description = "Adds an optional richtext format bar for text selections"}
        });

        private static readonly List<ICompletionData> Format = new List<ICompletionData>(new []
        {
            new NzazuCompletionData { Text = "date", Replacement = "\"yyyy-MM-dd Z HH:mm:ss.zzz\"", Description = "A date format"},

            new NzazuCompletionData { Text = "custom", Replacement = "\"#.00\"", Description = "Custom"},
            new NzazuCompletionData { Text = "currency (C)", Replacement = "\"C2\"", Description = "Currency format"},
            new NzazuCompletionData { Text = "fixed (F)", Replacement = "\"F2\"", Description = "Fixed point"},
            new NzazuCompletionData { Text = "general (G)", Replacement = "\"G\"", Description = "General"},
            new NzazuCompletionData { Text = "percent (P)", Replacement = "\"P3\"", Description = "Percent"},
           
            new NzazuCompletionData { Text = "rtf", Replacement = "\"rtf\"", Description = "Rich text format"},
            new NzazuCompletionData { Text = "plain", Replacement = "\"plain\"", Description = "Plain text"},
            new NzazuCompletionData { Text = "xaml", Replacement = "\"xaml\"", Description = "Xaml"}
        }); 

        private static readonly ICompletionData KeyComplete = 
            new NzazuCompletionData { Text = "\"<key>\"", Description = "A unique key for the field" };

        private static readonly ICompletionData PromptComplete =
            new NzazuCompletionData { Text = "\"<Prompt>\"", Description = "The prompt/label" };

        private static readonly ICompletionData HintComplete =
            new NzazuCompletionData { Text = "\"Enter <data>\"", Description = "A hint to show if (e.g. watermark)" };

        private static readonly ICompletionData DescriptionComplete =
            new NzazuCompletionData { Text = "\"The field should be <validation>\"", Description = "A description to be shown as a tooltip" };

    }
}