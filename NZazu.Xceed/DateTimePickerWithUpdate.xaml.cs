using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    /// <summary>
    /// Interaction logic for DateTimePickerWithUpdate.xaml
    /// </summary>
    public partial class DateTimePickerWithUpdate
    {
        private readonly DateTimePicker _valuePicker;

        public DateTimePickerWithUpdate()
        {
            InitializeComponent();

            _valuePicker = new DateTimePicker();
            _valuePicker.ValueChanged += ValuePickerOnValueChanged;
            _valuePicker.HorizontalAlignment = HorizontalAlignment.Stretch;
            LayoutGrid.Children.Add(_valuePicker);
        }

        private bool _isInValueChanges;
        private void ValuePickerOnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
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

        #region dependency properties

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(DateTime?), typeof(DateTimePickerWithUpdate), new PropertyMetadata(default(DateTime?), ValuePropertyChangedCallback));

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (DateTimePickerWithUpdate)d;
            ctrl._valuePicker.Value = (DateTime?)e.NewValue;
        }

        public DateTime? Value
        {
            get { return (DateTime?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ActualDateTimeProviderProperty = DependencyProperty.Register(
            "ActualDateTimeProvider", typeof(IActualDateTimeProvider), typeof(DateTimePickerWithUpdate), new PropertyMetadata(new NowDateTimeProvider()));

        public IActualDateTimeProvider ActualDateTimeProvider
        {
            get { return (IActualDateTimeProvider)GetValue(ActualDateTimeProviderProperty); }
            set { SetValue(ActualDateTimeProviderProperty, value); }
        }

        public bool UseUTCDateTimeFormat
        {
            get { return ActualDateTimeProvider is UtcDateTimeProvider; }
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
            get { return _valuePicker.Format; }
            set { _valuePicker.Format = value; }
        }

        public string FormatString
        {
            get { return _valuePicker.FormatString; }
            set { _valuePicker.FormatString = value; }
        }

        public object Watermark
        {
            get { return _valuePicker.Watermark; }
            set { _valuePicker.Watermark = value; }
        }

        #endregion

        internal void UpdateToToday_OnClick(object sender, RoutedEventArgs e)
        {
            Value = ActualDateTimeProvider.Now();
        }
    }

    public interface IActualDateTimeProvider
    {
        DateTime Now();
    }

    internal class NowDateTimeProvider : IActualDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }

    internal class UtcDateTimeProvider : IActualDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.UtcNow;
        }
    }

    [TestFixtureFor(typeof(DateTimePickerWithUpdate))]
    // ReSharper disable once InconsistentNaming
    internal class DateTimePickerWithUpdate_Should
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Be_Creatable()
        {
            var sut = new DateTimePickerWithUpdate();

            sut.Should().NotBeNull();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Change_Value()
        {
            var bd = new DateTime(1980, 2, 20, 14, 23, 12);
            var sut = new DateTimePickerWithUpdate();
            var field = sut.GetType().GetField("_valuePicker", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            // ReSharper disable once PossibleNullReferenceException
            var ctrl = (DateTimePicker)field.GetValue(sut);

            // lest test...
            sut.Value = bd;
            sut.Value.Should().BeCloseTo(bd);
            ctrl.Value.Should().BeCloseTo(bd);

            // now press the button
            sut.UpdateToToday_OnClick(this, null);

            sut.Value.Should().BeCloseTo(DateTime.Now, 2000);
            ctrl.Value.Should().BeCloseTo(DateTime.Now, 2000);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Use_Format_Settings()
        {
            var sut = new DateTimePickerWithUpdate()
            {
                Watermark = "the watermark",
                Format = DateTimeFormat.Custom,
                FormatString = "yyyy-MMM-dd"
            };
            sut.Watermark.Should().Be("the watermark");
            sut.Format.Should().Be(DateTimeFormat.Custom);
            sut.FormatString.Should().Be("yyyy-MMM-dd");

            var field = sut.GetType().GetField("_valuePicker", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            // ReSharper disable once PossibleNullReferenceException
            var ctrl = (DateTimePicker)field.GetValue(sut);

            ctrl.Watermark.Should().Be("the watermark");
            ctrl.Format.Should().Be(DateTimeFormat.Custom);
            ctrl.FormatString.Should().Be("yyyy-MMM-dd");
        }

    }
}
