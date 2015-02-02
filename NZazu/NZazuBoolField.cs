using System.Windows.Controls;

namespace NZazu
{
    class NZazuBoolField : NZazuField
    {
        public NZazuBoolField(string key, string prompt, string description) : base(key, prompt, description)
        {
            Type = "bool";
            Control = new CheckBox();
        }
    }
}