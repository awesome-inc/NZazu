﻿using System.Collections.Generic;
using System.Configuration;
using ICSharpCode.AvalonEdit.CodeCompletion;

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
            }
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
            new NzazuCompletionData { Text = "richtext", Replacement = "\"richtext\"", Description = "Adds a richt-text field"},
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
            new NzazuCompletionData { Text = "Format: Date", Replacement = "[{ \"Format\": \"yyyy-MM-dd Z HH:mm:ss.zzz\" }]", Description = "A date format"},
            new NzazuCompletionData { Text = "Format: Custom", Replacement = "[{ \"Format\": \"#.00\" }]", Description = "Custom"},
            new NzazuCompletionData { Text = "Format: Currency", Replacement = "[{ \"Format\": \"C2\" }]", Description = "Currency format"},
            new NzazuCompletionData { Text = "Format: Fixed Point", Replacement = "[{ \"Format\": \"F2\" }]", Description = "Fixed point"},
            new NzazuCompletionData { Text = "Format: General", Replacement = "[{ \"Format\": \"C2\" }]", Description = "General"},
            new NzazuCompletionData { Text = "Format: Percent", Replacement = "[{ \"Format\": \"P3\" }]", Description = "Percent"},
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