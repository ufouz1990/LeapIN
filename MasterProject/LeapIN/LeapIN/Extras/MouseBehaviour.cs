using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LeapIN.Extras
{
    public static class MouseBehaviour
    {
        public static readonly DependencyProperty MouseOverProperty
            = DependencyProperty.RegisterAttached(
            "MouseOver",
            typeof(bool),
            typeof(MouseBehaviour),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                MouseOverBindingPropertyChanged));

        public static bool GetMouseOver(DependencyObject obj)
        {
            return (bool)obj.GetValue(MouseOverProperty);
        }

        public static void SetMouseOver(DependencyObject obj, bool value)
        {
            obj.SetValue(MouseOverProperty, value);
        }

        private static void MouseOverBindingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;

            if (element != null)
            {

            }

            //lock (_wiredUpElementsLock)
            //{
            //    if (_wiredUpElements.Contains(element))
            //    {
            //        return;
            //    }

            //    _wiredUpElements.Add(element);
            //    element.MouseEnter += (sender, args) =>
            //    {
            //        var f = (FrameworkElement)sender;
            //        SetMouseOver(f, f.IsMouseOver);
            //    };
            //}
        }

    }
}
