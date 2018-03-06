using System;
using System.ComponentModel;
using System.Windows.Media;
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

        public Brush StatusBrush { get => StateToBrush(Status); }

        public override string ToString()
        {
            return Name;
        }

        protected virtual Brush StateToBrush(ETemplateStatus status)
        {
            switch(status)
            {
                case ETemplateStatus.Modified:
                    return Brushes.DarkBlue;
                case ETemplateStatus.New:
                    return Brushes.DarkGreen;
                default:
                    return Brushes.Black;
            }
        }

    }
}