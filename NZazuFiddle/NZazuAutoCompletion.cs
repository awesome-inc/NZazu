using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.CodeCompletion;
using NZazu.FieldBehavior;
using Xceed.Wpf.Toolkit;

namespace NZazuFiddle
{
    internal static class NZazuAutoCompletion
    {
        private static readonly ICompletionData KeyComplete =
            new NzazuCompletionData {Text = "\"<key>\"", Description = "A unique key for the field"};

        private static readonly ICompletionData PromptComplete =
            new NzazuCompletionData {Text = "\"<Prompt>\"", Description = "The prompt/label"};

        private static readonly ICompletionData HintComplete =
            new NzazuCompletionData {Text = "\"Enter <data>\"", Description = "A hint to show if (e.g. watermark)"};

        private static readonly ICompletionData DescriptionComplete =
            new NzazuCompletionData
                {Text = "\"The field should be <validation>\"", Description = "A description to be shown as a tooltip"};

        private static readonly List<ICompletionData> Types = new List<ICompletionData>
        {
            // fields
            new NzazuCompletionData {Text = "label", Replacement = "\"label\"", Description = "Adds a label"},
            new NzazuCompletionData {Text = "string", Replacement = "\"string\"", Description = "Adds a string field"},
            new NzazuCompletionData {Text = "bool", Replacement = "\"bool\"", Description = "Adds a boolean field"},
            new NzazuCompletionData {Text = "date", Replacement = "\"date\"", Description = "Adds a date/time field"},
            new NzazuCompletionData {Text = "int", Replacement = "\"int\"", Description = "Adds an integer field"},
            new NzazuCompletionData {Text = "double", Replacement = "\"double\"", Description = "Adds a double field"},
            new NzazuCompletionData
            {
                Text = "option", Replacement = "\"option\"", Description = "Adds an option field (choose from Values)"
            },
            new NzazuCompletionData
                {Text = "richtext", Replacement = "\"richtext\"", Description = "Adds a richt-text field"},
            new NzazuCompletionData {Text = "group", Replacement = "\"group\"", Description = "Adds a group field"},

            // checks
            new NzazuCompletionData
                {Text = "required", Replacement = "\"required\"", Description = "Adds a 'required' check"},
            new NzazuCompletionData
                {Text = "length", Replacement = "\"length\"", Description = "Adds a string length check"},
            new NzazuCompletionData {Text = "range", Replacement = "\"range\"", Description = "Adds a range check"},
            new NzazuCompletionData {Text = "regex", Replacement = "\"regex\"", Description = "Adds a regex checks"}
        };

        private static readonly List<ICompletionData> Layouts = new List<ICompletionData>
        {
            new NzazuCompletionData
                {Text = "grid", Replacement = "\"grid\"", Description = "Grid layout (rows, default)"},
            new NzazuCompletionData
                {Text = "stack", Replacement = "\"stack\"", Description = "Stacked Layout (left to right)"}
        };


        private static readonly List<ICompletionData> Checks = new List<ICompletionData>
        {
            new NzazuCompletionData
            {
                Text = "required", Replacement = "[{ \"Type\": \"required\" }]", Description = "The field is required"
            },
            new NzazuCompletionData
            {
                Text = "length", Replacement = "[{ \"Type\": \"length\", \"Values\": [\"6\", \"8\" ] }]",
                Description = "The field length must be in the specified range"
            },
            new NzazuCompletionData
            {
                Text = "range", Replacement = "[{ \"Type\": \"range\", \"Values\": [\"-50.0\", \"+50.0\" ] }]",
                Description = "The field value must be in the specified range"
            },
            new NzazuCompletionData
            {
                Text = "regex",
                Replacement = "[{ \"Type\": \"regex\", \"Values\": [\"<Hint>\", \"pattern1\", \"pattern2\" ] }]",
                Description = "The field value must match one of the specified patterns"
            }
        };

        private static readonly List<ICompletionData> Settings = GetSettings();

        private static readonly List<ICompletionData> Format = new List<ICompletionData>
        {
            new NzazuCompletionData
                {Text = "date", Replacement = "\"yyyy-MM-dd Z HH:mm:ss.zzz\"", Description = "A date format"},

            new NzazuCompletionData {Text = "custom", Replacement = "\"#.00\"", Description = "Custom"},
            new NzazuCompletionData {Text = "currency (C)", Replacement = "\"C2\"", Description = "Currency format"},
            new NzazuCompletionData {Text = "fixed (F)", Replacement = "\"F2\"", Description = "Fixed point"},
            new NzazuCompletionData {Text = "general (G)", Replacement = "\"G\"", Description = "General"},
            new NzazuCompletionData {Text = "percent (P)", Replacement = "\"P3\"", Description = "Percent"},

            new NzazuCompletionData {Text = "rtf", Replacement = "\"rtf\"", Description = "Rich text format"},
            new NzazuCompletionData {Text = "plain", Replacement = "\"plain\"", Description = "Plain text"},
            new NzazuCompletionData {Text = "xaml", Replacement = "\"xaml\"", Description = "Xaml"}
        };

        static NZazuAutoCompletion()
        {
            // cf.: http://stackoverflow.com/questions/722868/sorting-a-list-using-lambda-linq-to-objects
            var comparer =
                Comparer<ICompletionData>.Create((i1, i2) =>
                    string.Compare(i1.Text, i2.Text, StringComparison.Ordinal));

            Types.Sort(comparer);
            Checks.Sort(comparer);
            Settings.Sort(comparer);
            Format.Sort(comparer);
            Layouts.Sort(comparer);
        }

        public static void AutoCompleteFor(this ICollection<ICompletionData> data, string word)
        {
            switch (word)
            {
                case "Type\":":
                    Types.ForEach(data.Add);
                    break;
                case "Key\":":
                    data.Add(KeyComplete);
                    break;
                case "Prompt\":":
                    data.Add(PromptComplete);
                    break;
                case "Hint\":":
                    data.Add(HintComplete);
                    break;
                case "Description\":":
                    data.Add(DescriptionComplete);
                    break;
                case "Checks\":":
                    Checks.ForEach(data.Add);
                    break;
                case "Settings\":":
                    Settings.ForEach(data.Add);
                    break;
                case "Format\":":
                    Format.ForEach(data.Add);
                    break;
                //case "Name\":": GetNames().ForEach(data.Add); break;
                case "Behavior\":":
                    GetBehaviors().ForEach(data.Add);
                    break;
                case "Layout\":":
                    Layouts.ForEach(data.Add);
                    break;
            }
        }

        private static List<NzazuCompletionData> GetBehaviors()
        {
            var behaviors = BehaviorExtender.GetBehaviors();
            return behaviors.Select(kvp => new NzazuCompletionData
                {
                    Text = kvp.Key,
                    Replacement = $"{{ \"Name\": \"{kvp.Key}\"}}",
                    Description = $"Add behavior '{kvp.Key}'"
                })
                .OrderBy(c => c.Text)
                .ToList();
        }

        private static List<ICompletionData> GetSettings()
        {
            var map = new Dictionary<string, ICompletionData>();

            // textbox
            CompleteSettingsFor<TextBox>(map);
            // options <- Selector <- ItemsControl <- Control
            CompleteSettingsFor<ComboBox>(map);
            // bool <-> check box
            CompleteSettingsFor<CheckBox>(map);
            // int,double <-> Numeric up down
            CompleteSettingsFor<DoubleUpDown>(map);
            // datetime
            CompleteSettingsFor<DateTimePicker>(map);

            var settings = new List<ICompletionData>
            {
                new NzazuCompletionData
                    {Text = "Format", Replacement = "{ \"Format\": \"\" }", Description = "The field format"},
                new NzazuCompletionData
                {
                    Text = "Height", Replacement = "{ \"Height\": \"64.3\" }",
                    Description = "The field's height in pixel"
                },
                new NzazuCompletionData
                {
                    Text = "Width", Replacement = "{ \"Width\": \"64.3\" }", Description = "The field's width in pixel"
                },
                new NzazuCompletionData
                {
                    Text = "ShowFormatBar", Replacement = "{ \"ShowFormatBar\": \"True\" }",
                    Description = "Adds an optional richtext format bar for text selections"
                }
            };
            foreach (var item in map)
            {
                var existing = settings.FirstOrDefault(s => s.Text == item.Key);
                if (existing == null)
                    settings.Add(item.Value);
            }

            return settings;
        }

        private static void CompleteSettingsFor<TControl>(Dictionary<string, ICompletionData> map)
            where TControl : Control
        {
            var control = (TControl) Activator.CreateInstance(typeof(TControl));
            var items = typeof(TControl).GetProperties()
                .Where(HasPublicGetterAndSetter)
                .ToDictionary(p => p.Name, p => ToAutoCompletion(control, p));

            foreach (var item in items)
                map[item.Key] = item.Value;
        }

        private static bool HasPublicGetterAndSetter(PropertyInfo p)
        {
            // cf. http://stackoverflow.com/questions/3762456/c-sharp-how-to-check-if-property-setter-is-public
            return p.GetGetMethod(true).IsPublic && p.CanWrite && p.GetSetMethod(true).IsPublic;
        }

        private static ICompletionData ToAutoCompletion<TControl>(TControl obj, PropertyInfo p) where TControl : Control
        {
            var value = p.GetValue(obj);
            return new NzazuCompletionData
            {
                Text = p.Name,
                Replacement = $"{{ \"{p.Name}\": \"{value}\" }}",
                Description = GetDescription(p)
            };
        }

        private static string GetDescription(MemberInfo p)
        {
            var attribute = p.GetCustomAttribute(typeof(DescriptionAttribute), true) as DescriptionAttribute;
            return attribute != null ? attribute.Description : string.Empty;

            // TODO: what about metadata extraction, cf.: http://blogs.msdn.com/b/jmstall/archive/2012/08/06/reflection-vs-metadata.aspx.
            // See: p.MetadataToken .... This is way more complicated than simple reflection!
        }
    }
}