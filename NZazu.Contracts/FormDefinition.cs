namespace NZazu.Contracts
{
    public class FormDefinition
    {
        public FieldDefinition[] Fields { get; set; }
        public string Layout { get; set; }
        public CheckDefinition[] Checks { get; set; }
    }
}