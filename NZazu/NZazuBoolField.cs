namespace NZazu
{
    class NZazuBoolField : NZazuField
    {
        public NZazuBoolField(string key, string prompt, string description) : base(key, prompt, description)
        {
        }

        public override string Type { get { return "bool"; } }
    }
}