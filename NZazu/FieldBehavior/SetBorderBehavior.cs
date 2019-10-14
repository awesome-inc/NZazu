using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NZazu.FieldBehavior
{
    public class SetBorderBehavior : INZazuWpfFieldBehavior
    {
        internal Control Control { get; private set; }

        public double Thickness { get; set; } = 3;

        public string ForegroundColor { get; set; } =
            $"{SystemColors.HotTrackBrush.Color.R},{SystemColors.HotTrackBrush.Color.G},{SystemColors.HotTrackBrush.Color.B}";

        public void AttachTo(INZazuWpfField field, INZazuWpfView view)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            var valueControl = field.ValueControl;

            Control = valueControl;
            Control.BorderThickness = new Thickness(Thickness);
            var rgb = ForegroundColor.Split(',').Select(byte.Parse).ToArray();
            Control.BorderBrush = new SolidColorBrush(Color.FromRgb(rgb[0], rgb[1], rgb[2]));
        }

        public void Detach()
        {
            if (Control == null) return;
            Control = null;
        }
    }
}