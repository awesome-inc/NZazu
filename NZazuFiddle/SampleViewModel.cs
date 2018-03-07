using System;
using System.ComponentModel;
using System.Windows.Media;
using Caliburn.Micro;
using NZazu.Contracts;
using NZazuFiddle.TemplateManagement.Contracts;

namespace NZazuFiddle
{
    public class SampleViewModel : Screen, ISample, IHandle<FormData>, IHandle<FormDefinition>
    {
        private readonly IEventAggregator _fiddleRelatedEvents;
        private readonly IEventAggregator _globalEvents;

        private ETemplateStatus _status;
        private Brush _statusBrush;

        public ETemplateStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                StatusBrush = StateToBrush(_status);
            }
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public IFiddle Fiddle { get; set; }

        public Brush StatusBrush {
            get => _statusBrush;
            set
            {
                _statusBrush = value;
                NotifyOfPropertyChange();
            }
        }

        public SampleViewModel(
                string id,
                string name,
                IFiddle fiddle,
                IEventAggregator fiddleRelatedEvents,
                IEventAggregator globalEvents
        )
        {
            Id = id;
            Name = name;
            Fiddle = fiddle;
            _fiddleRelatedEvents = fiddleRelatedEvents;
            _globalEvents = globalEvents;

            _status = ETemplateStatus.Initial;
            _statusBrush = StateToBrush(_status);

            _fiddleRelatedEvents.Subscribe(this);
        }

        public override string ToString()
        {
            return Name;
        }

        protected virtual Brush StateToBrush(ETemplateStatus status)
        {
            switch(status)
            {
                case ETemplateStatus.Modified:
                    return Brushes.DarkRed;
                case ETemplateStatus.New:
                    return Brushes.DarkGreen;
                default:
                    return Brushes.Black;
            }
        }

        public void Handle(FormData message)
        {
            Status = ETemplateStatus.Modified;
        }

        public void Handle(FormDefinition message)
        {
            Status = ETemplateStatus.Modified;
        }
    }
}