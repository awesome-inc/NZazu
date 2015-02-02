using System;
using System.Windows.Controls;

namespace NZazu
{
    abstract class NZazuField : INZazuField
    {
        public string Type { get; protected set; }
        public Control Control { get; protected set; }

        internal protected NZazuField(string key, string prompt, string description)
        {
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("key");

            Key = key;
            Prompt = prompt;
            Description = description;
        }

        public string Key { get; private set; }
        public string Prompt { get; private set; }
        public string Description { get; private set; }
    }
}