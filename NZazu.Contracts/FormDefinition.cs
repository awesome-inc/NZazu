using Newtonsoft.Json;

namespace NZazu.Contracts
{
    public class FormDefinition
    {
        public FieldDefinition[] Fields { get; set; }
        public string FocusOn { get; set; }
        public string Layout { get; set; }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            FormDefinition formDef = (FormDefinition)obj;
            
            // Convert to Json to make content comparison more easy
            var formDefAsJson = JsonConvert.SerializeObject(formDef, Formatting.Indented
                , new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            var thisFormAsJson = toJson();
            if (formDefAsJson.Equals(thisFormAsJson)) return true;

            // otherwise return false
            return false;
        }

        public override int GetHashCode()
        {
            var hash = toJson().GetHashCode();
            return hash;
        }

        private string toJson()
        {
            var thisFormAsJson = JsonConvert.SerializeObject(this, Formatting.Indented
                , new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            return thisFormAsJson;
        }
    }
}
