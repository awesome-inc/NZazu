using Caliburn.Micro;
using NZazuFiddle.TemplateManagement.Contracts;

namespace NZazuFiddle
{
    public class SampleViewModel : Screen, ISample
    {
        public ETemplateStatus Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public IFiddle Fiddle { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}