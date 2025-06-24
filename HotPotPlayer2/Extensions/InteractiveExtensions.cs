using Avalonia;
using Avalonia.Animation;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Extensions
{
    public class InteractiveExtensions : AvaloniaObject
    {
        static InteractiveExtensions()
        {
            IsVisibleProperty.Changed.AddClassHandler<Interactive>(HandleIsVisibleChanged);
        }

        public static bool? GetIsVisible(AvaloniaObject obj)
        {
            return obj.GetValue(IsVisibleProperty);
        }

        public static void SetIsVisible(AvaloniaObject obj, bool? value)
        {
            obj.SetValue(IsVisibleProperty, value);
        }

        public static readonly AttachedProperty<bool?> IsVisibleProperty =
            AvaloniaProperty.RegisterAttached<InteractiveExtensions,Interactive,bool?>("IsVisible", default, false, BindingMode.OneWay);

        public static string? GetShowTransform(AvaloniaObject obj)
        {
            return obj.GetValue(ShowTransformProperty);
        }

        public static void SetShowTransform(AvaloniaObject obj, string? value)
        {
            obj.SetValue(ShowTransformProperty, value);
        }

        public static readonly AttachedProperty<string?> ShowTransformProperty =
            AvaloniaProperty.RegisterAttached<InteractiveExtensions, Interactive, string?>("ShowTransform", default, false, BindingMode.OneWay);

        public static string? GetHideTransform(AvaloniaObject obj)
        {
            return obj.GetValue(HideTransformProperty);
        }

        public static void SetHideTransform(AvaloniaObject obj, string? value)
        {
            obj.SetValue(HideTransformProperty, value);
        }

        public static readonly AttachedProperty<string?> HideTransformProperty =
            AvaloniaProperty.RegisterAttached<InteractiveExtensions, Interactive, string?>("HideTransform", default, false, BindingMode.OneWay);

        private static async void HandleIsVisibleChanged(Interactive interactive, AvaloniaPropertyChangedEventArgs args)
        {
            var n = args.NewValue;
            if (n == null || args.OldValue == null) { return; };
            var b = (bool)n;
            var parentIsVisible = interactive.Parent!.GetValue(IsVisibleProperty);
            if (b)
            {
                // Visible
                interactive.IsVisible = true;
                var showTrans = GetShowTransform(interactive);
                if (!string.IsNullOrEmpty(showTrans))
                {
                    interactive.RenderTransform = TransformOperations.Parse(showTrans);
                }
            }
            else
            {
                var hideTrans = GetHideTransform(interactive);
                var transitions = interactive.Transitions;
                if (transitions != null && transitions.Count > 0)
                {
                    var dur = (transitions[0] as TransitionBase)?.Duration;

                    if (dur != null && dur.Value.Ticks > 0)
                    {
                        if (!string.IsNullOrEmpty(hideTrans))
                        {
                            interactive.RenderTransform = TransformOperations.Parse(hideTrans);
                        }
                        if(parentIsVisible == null)
                        {
                            await Task.Delay(dur.Value);
                            interactive.IsVisible = false;
                        }
                    }
                }
            }
        }
    }
}
