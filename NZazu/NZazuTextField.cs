using System.Windows.Controls;

namespace NZazu
{
    class NZazuTextField : NZazuField
    {
        public NZazuTextField(string key, string prompt, string description) : base(key, prompt, description)
        {
            Type = "string";
            Control = new TextBox();
        }
    }
}