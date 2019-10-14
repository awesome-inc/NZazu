using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace NZazu.Extensions
{
    public static class FocusExtensions
    {
        // cf.: https://wpf.codeplex.com/workitem/13476
        public static Task DelayedFocus(this UIElement uiElement)
        {
            if (uiElement == null) throw new ArgumentNullException(nameof(uiElement));

            return uiElement.Dispatcher.BeginInvoke(SetFocusAction(uiElement), DispatcherPriority.Render).Task;
        }

        private static Action SetFocusAction(UIElement uiElement)
        {
            return () => SetFocus(uiElement);
        }

        public static void SetFocus(this UIElement uiElement)
        {
            uiElement.Focusable = true;
            FocusManager.SetFocusedElement(uiElement, uiElement);
            uiElement.Focus();
            Keyboard.Focus(uiElement);
        }

        public static void RemoveFocus(this UIElement uiElement)
        {
            var frameWorkElement = uiElement as FrameworkElement;
            if (frameWorkElement == null) return;

            // Move to a parent that can take focus
            var parent = frameWorkElement.Parent as FrameworkElement;
            while (parent != null && !parent.Focusable) parent = parent.Parent as FrameworkElement;

            var scope = FocusManager.GetFocusScope(uiElement);
            FocusManager.SetFocusedElement(scope, parent);
        }
    }
}