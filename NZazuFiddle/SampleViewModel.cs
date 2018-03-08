using System;
using System.ComponentModel;
using System.Windows.Media;
using Caliburn.Micro;
using FontAwesome.Sharp;
using NZazu.Contracts;
using NZazuFiddle.TemplateManagement.Contracts;

namespace NZazuFiddle
{
    public class SampleViewModel : Screen, ISample, IHandle<FormData>, IHandle<FormDefinition>
    {
        private readonly IEventAggregator _fiddleRelatedEvents;

        // ToDo [bornemeier]: implement statechange as state-pattern
        private ETemplateStatus _status;
        private Brush _statusBrush;
        private IconChar _statusIcon;

        public ETemplateStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                StatusBrush = StateToBrush(_status);
                StatusIcon = StateToIcon(_status);
                NotifyOfPropertyChange();
            }
        }
        public Brush StatusBrush
        {
            get => _statusBrush;
            set
            {
                _statusBrush = value;
                NotifyOfPropertyChange();
            }
        }
        public IconChar StatusIcon
        {
            get => _statusIcon;
            set {
                _statusIcon = value;
                NotifyOfPropertyChange();
            }
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public IFiddle Fiddle { get; set; }
        public SampleViewModel(
                string id,
                string name,
                IFiddle fiddle,
                IEventAggregator fiddleRelatedEvents,
                ETemplateStatus status = ETemplateStatus.Initial
        )
        {
            Id = id;
            Name = name;
            Fiddle = fiddle;
            _fiddleRelatedEvents = fiddleRelatedEvents;

            _status = status;
            _statusBrush = StateToBrush(_status);
            _statusIcon = StateToIcon(_status);

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
                case ETemplateStatus.Imported:
                    return Brushes.BlueViolet;
                default:
                    return Brushes.Black;
            }
        }

        protected virtual IconChar StateToIcon(ETemplateStatus status)
        {
            switch (status)
            {
                case ETemplateStatus.Modified:
                    return IconChar.Asterisk;
                case ETemplateStatus.New:
                    return IconChar.Asterisk;
                case ETemplateStatus.Imported:
                    return IconChar.Asterisk;
                default:
                    return IconChar.Database;
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