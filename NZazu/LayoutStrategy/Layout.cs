using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace NZazu.LayoutStrategy
{
    public abstract class Layout : INZazuWpfLayoutStrategy
    {
        private readonly ControlTemplate _errorTemplate;

        protected Layout(ControlTemplate errorTemplate = null)
        {
            _errorTemplate = errorTemplate ?? GetDefaultErrorTemplate();
        }

        public abstract void DoLayout(
            ContentControl contentControl,
            IEnumerable<INZazuWpfField> fields,
            IResolveLayout resolveLayout = null);

        protected void SetErrorTemplate(Control valueElement)
        {
            Validation.SetErrorTemplate(valueElement, _errorTemplate);
        }

        protected void ProcessGroupField(
            IResolveLayout resolveLayout,
            Control control,
            INZazuWpfField field)
        {
            var contentControl = control as ContentControl;
            if (!(field is INZazuWpfFieldContainer groupField) || contentControl == null) return;
            var layout = SafeResolve(resolveLayout, groupField.Layout);
            layout.DoLayout(contentControl, groupField.Fields, resolveLayout);
        }

        private static ControlTemplate GetDefaultErrorTemplate()
        {
            if (Application.Current == null || Application.Current.Dispatcher.HasShutdownStarted)
            {
                Trace.TraceWarning("No application");
                return null;
            }

            var uri = new Uri("pack://application:,,,/NZazu;component/Themes/Generic.xaml", UriKind.Absolute);
            var resources = new ResourceDictionary {Source = uri};
            return (ControlTemplate) resources["NZazuErrorTemplate"];
        }

        private INZazuWpfLayoutStrategy SafeResolve(IResolveLayout resolveLayout, string name)
        {
            if (resolveLayout == null) return this;
            var layout = resolveLayout.Resolve(name);
            return layout ?? this;
        }

        protected static void StyleValue(Control control)
        {
            control.VerticalAlignment = VerticalAlignment.Center;
            control.Margin = new Thickness(3);
            control.Padding = new Thickness(3);
        }

        protected static void StyleLabel(Control control)
        {
            StyleValue(control);
            control.HorizontalAlignment = HorizontalAlignment.Left;
        }
    }
}