using Caliburn.Micro;
using Newtonsoft.Json;

namespace NZazuFiddle
{
    public abstract class HaveJsonFor<T> : Screen, IHaveJsonFor<T>
    {
        private string _jsonError;

        public abstract T Model { get; set; }

        public string Json
        {
            get => JsonConvert.SerializeObject(Model, Formatting.Indented
                , new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore }
            );
            set
            {
                try
                {
                    Model = JsonConvert.DeserializeObject<T>(value);
                    JsonError = null;
                }
                catch (JsonException ex) { JsonError = ex.Message; }
            }
        }

        public bool HasJsonError => JsonError != null;

        public string JsonError
        {
            get => _jsonError;
            private set
            {
                if (Equals(value, _jsonError)) return;
                _jsonError = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(HasJsonError));
            }
        }
    }
}