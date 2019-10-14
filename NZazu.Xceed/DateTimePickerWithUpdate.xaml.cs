using System;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    /// <summary>
    ///     Interaction logic for DateTimePickerWithUpdate.xaml
    /// </summary>
    public partial class DateTimePickerWithUpdate
    {
        private readonly DateTimePicker _valuePicker;

        private bool _isInValueChanges;

        public DateTimePickerWithUpdate()
        {
            InitializeComponent();

            _valuePicker = new DateTimePicker();
            _valuePicker.ValueChanged += ValuePickerOnValueChanged;
            _valuePicker.HorizontalAlignment = HorizontalAlignment.Stretch;
            LayoutGrid.Children.Add(_valuePicker);
        }

        private void ValuePickerOnValueChanged(object sender,
            RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            if (_isInValueChanges) return;

            try
            {
                _isInValueChanges = true;
                Value = _valuePicker.Value;
            }
            finally
            {
                _isInValueChanges = false;
            }
        }

        internal void UpdateToToday_OnClick(object sender, RoutedEventArgs e)
        {
            Value = ActualDateTimeProvider.Now();
        }

        #region dependency properties

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(DateTime?), typeof(DateTimePickerWithUpdate),
            new PropertyMetadata(default(DateTime?), ValuePropertyChangedCallback));

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (DateTimePickerWithUpdate) d;
            ctrl._valuePicker.Value = (DateTime?) e.NewValue;
        }

        public DateTime? Value
        {
            get => (DateTime?) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ActualDateTimeProviderProperty = DependencyProperty.Register(
            "ActualDateTimeProvider", typeof(IActualDateTimeProvider), typeof(DateTimePickerWithUpdate),
            new PropertyMetadata(new NowDateTimeProvider()));

        // ReSharper disable once MemberCanBePrivate.Global
        public IActualDateTimeProvider ActualDateTimeProvider
        {
            get => (IActualDateTimeProvider) GetValue(ActualDateTimeProviderProperty);
            set => SetValue(ActualDateTimeProviderProperty, value);
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
        public bool UseUTCDateTimeFormat
        {
            get => ActualDateTimeProvider is UtcDateTimeProvider;
            set
            {
                if (value)
                    ActualDateTimeProvider = new UtcDateTimeProvider();
                else
                    ActualDateTimeProvider = new NowDateTimeProvider();
            }
        }

        #endregion

        #region pass-through properties

        public DateTimeFormat Format
        {
            get => _valuePicker.Format;
            set => _valuePicker.Format = value;
        }

        public string FormatString
        {
            get => _valuePicker.FormatString;
            set => _valuePicker.FormatString = value;
        }

        public object Watermark
        {
            get => _valuePicker.Watermark;
            set => _valuePicker.Watermark = value;
        }

        #endregion
    }
}