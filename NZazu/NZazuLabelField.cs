using System.Windows.Controls;

namespace NZazu
{
    class NZazuLabelField : NZazuField
    {
        public NZazuLabelField(string key, string prompt, string description) : base(key, prompt, description)
        {
            Type = "label";
            Control = new Label();
        }
    }
}