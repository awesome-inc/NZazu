using System;
using System.Windows;
using NZazu.Contracts;

namespace NZazu
{
    abstract class NZazuField : INZazuField
    {
        internal protected FrameworkElement Control { get; set; }

        public abstract string Type { get; }

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