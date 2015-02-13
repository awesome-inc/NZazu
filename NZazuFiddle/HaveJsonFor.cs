using System;
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
            get { return JsonConvert.SerializeObject(Model, Formatting.Indented); }
            set
            {
                try
                {
                    Model = JsonConvert.DeserializeObject<T>(value);
                    JsonError = null;
                }
                catch (JsonReaderException ex)
                {
                    JsonError = String.Format("Error in line {0}, position {1}. Path: {2}",
                        ex.LineNumber, ex.LinePosition, ex.Path);
                }
            }
        }

        public bool HasJsonError { get { return JsonError != null; } }

        public string JsonError
        {
            get { return _jsonError; }
            private set
            {
                if (Equals(value, _jsonError)) return;
                _jsonError = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange("HasJsonError");
            }
        }
    }
}