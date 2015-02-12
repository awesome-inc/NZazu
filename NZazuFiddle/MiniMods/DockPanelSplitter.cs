using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

// ReSharper disable CheckNamespace
namespace MiniMod.WPF
{
    // cf.: adapted from http://www.codeproject.com/Articles/34377/DockPanel-Splitter-Control-for-WPF
    public class DockPanelSplitter : Control
    {
        public static readonly DependencyProperty ProportionalResizeProperty =
            DependencyProperty.Register("ProportionalResize", typeof(bool), typeof(DockPanelSplitter),
            new UIPropertyMetadata(false));

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(DockPanelSplitter),
            new UIPropertyMetadata(4.0, ThicknessChanged));

        private FrameworkElement _element;
        private double _height;
        private double _width;
        private double _previousParentHeight;
        private double _previousParentWidth;
        private Point _startDragPoint;

        static DockPanelSplitter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockPanelSplitter),
                new FrameworkPropertyMetadata(typeof(DockPanelSplitter)));

            // override the Background property to enable Mouse capturing by default
            BackgroundProperty.OverrideMetadata(typeof(DockPanelSplitter),
                new FrameworkPropertyMetadata(Brushes.Transparent));

            // override the Dock property to get notifications when Dock is changed
            DockPanel.DockProperty.OverrideMetadata(typeof(DockPanelSplitter),
                new FrameworkPropertyMetadata(Dock.Left, DockChanged));
        }

        public DockPanelSplitter()
        {
            Loaded += DockPanelSplitterLoaded;
            Unloaded += DockPanelSplitterUnloaded;

            UpdateHeightOrWidth();
        }

        public bool ProportionalResize
        {
            get { return (bool)GetValue(ProportionalResizeProperty); }
            set { SetValue(ProportionalResizeProperty, value); }
        }

        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public bool IsHorizontal
        {
            get
            {
                var dock = DockPanel.GetDock(this);
                return dock == Dock.Top || dock == Dock.Bottom;
            }
        }

        private void DockPanelSplitterLoaded(object sender, RoutedEventArgs e)
        {
            var dp = (Panel)Parent;

            // Subscribe to the parent's size changed event
            dp.SizeChanged += ParentSizeChanged;

            // Store the current size of the parent DockPanel
            _previousParentWidth = dp.ActualWidth;
            _previousParentHeight = dp.ActualHeight;

            // Find the target element
            UpdateTargetElement();
        }

        private void DockPanelSplitterUnloaded(object sender, RoutedEventArgs e)
        {
            var dp = (Panel)Parent;
            dp.SizeChanged -= ParentSizeChanged;
        }

        private static void DockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockPanelSplitter)d).UpdateHeightOrWidth();
        }

        private static void ThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockPanelSplitter)d).UpdateHeightOrWidth();
        }

        private void UpdateHeightOrWidth()
        {
            if (IsHorizontal)
            {
                Height = Thickness;
                Width = double.NaN;
            }
            else
            {
                Width = Thickness;
                Height = double.NaN;
            }
        }

        private void UpdateTargetElement()
        {
            var dp = (Panel)Parent;

            var i = dp.Children.IndexOf(this);

            // The splitter cannot be the first child of the parent DockPanel
            // The splitter works on the 'older' sibling 
            if (i > 0 && dp.Children.Count > 0)
            {
                _element = dp.Children[i - 1] as FrameworkElement;
            }
        }

        private void SetTargetWidth(double newWidth)
        {
            newWidth = Math.Max(_element.MinWidth, Math.Min(_element.MaxWidth, newWidth));
            if (newWidth < _element.MinWidth)
                newWidth = _element.MinWidth;
            if (newWidth > _element.MaxWidth)
                newWidth = _element.MaxWidth;

            // todo - constrain the width of the element to the available client area
            var dp = (Panel)Parent;
            var dock = DockPanel.GetDock(this);
            var t = (MatrixTransform)_element.TransformToAncestor(dp);
            if (dock == Dock.Left && newWidth > dp.ActualWidth - t.Matrix.OffsetX - Thickness)
                newWidth = dp.ActualWidth - t.Matrix.OffsetX - Thickness;

            newWidth = Math.Max(_element.MinWidth, Math.Min(_element.MaxWidth, newWidth));
            _element.Width = newWidth;
        }

        private void SetTargetHeight(double newHeight)
        {
            newHeight = Math.Max(_element.MinHeight, Math.Min(_element.MaxHeight, newHeight));

            // todo - constrain the height of the element to the available client area
            var dp = (Panel)Parent;
            var dock = DockPanel.GetDock(this);
            var t = (MatrixTransform)_element.TransformToAncestor(dp);
            
            if (dock == Dock.Top && newHeight > dp.ActualHeight - t.Matrix.OffsetY - Thickness)
                newHeight = dp.ActualHeight - t.Matrix.OffsetY - Thickness;

            newHeight = Math.Max(_element.MinHeight, Math.Min(_element.MaxHeight, newHeight));
            _element.Height = newHeight;
        }

        private void ParentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!ProportionalResize) return;

            var dp = (DockPanel)Parent;

            var sx = dp.ActualWidth / _previousParentWidth;
            var sy = dp.ActualHeight / _previousParentHeight;

            if (!double.IsInfinity(sx))
                SetTargetWidth(_element.Width * sx);
            if (!double.IsInfinity(sy))
                SetTargetHeight(_element.Height * sy);

            _previousParentWidth = dp.ActualWidth;
            _previousParentHeight = dp.ActualHeight;
        }

        private double AdjustWidth(double dx, Dock dock)
        {
            if (dock == Dock.Right)
                dx = -dx;

            _width += dx;
            SetTargetWidth(_width);

            return dx;
        }

        private double AdjustHeight(double dy, Dock dock)
        {
            if (dock == Dock.Bottom)
                dy = -dy;

            _height += dy;
            SetTargetHeight(_height);

            return dy;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (!IsEnabled) return;
            Cursor = IsHorizontal ? Cursors.SizeNS : Cursors.SizeWE;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!IsEnabled) return;

            if (!IsMouseCaptured)
            {
                _startDragPoint = e.GetPosition(Parent as IInputElement);
                UpdateTargetElement();
                if (_element != null)
                {
                    _width = _element.ActualWidth;
                    _height = _element.ActualHeight;
                    CaptureMouse();
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                var ptCurrent = e.GetPosition(Parent as IInputElement);
                var delta = new Point(ptCurrent.X - _startDragPoint.X, ptCurrent.Y - _startDragPoint.Y);
                var dock = DockPanel.GetDock(this);

                if (IsHorizontal)
                    delta.Y = AdjustHeight(delta.Y, dock);
                else
                    delta.X = AdjustWidth(delta.X, dock);

                var isBottomOrRight = (dock == Dock.Right || dock == Dock.Bottom);

                // When docked to the bottom or right, the position has changed after adjusting the size
                _startDragPoint = isBottomOrRight 
                    ? e.GetPosition(Parent as IInputElement) 
                    : new Point(_startDragPoint.X + delta.X, _startDragPoint.Y + delta.Y);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();
            base.OnMouseUp(e);
        }
    }
}
