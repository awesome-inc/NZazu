namespace NZazu
{
    class NZazuTextField : NZazuField
    {
        public NZazuTextField(string key, string prompt, string description) : base(key, prompt, description)
        {
        }

        public override string Type { get { return "string"; } }
    }
}