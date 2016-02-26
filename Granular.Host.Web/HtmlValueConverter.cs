﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Granular.Extensions;

namespace Granular.Host
{
    public interface IHtmlValueConverter
    {
        string ToPixelString(double value);
        string ToPercentString(double value);
        string ToDegreesString(double value);
        string ToImplicitValueString(double value);
        string ToPixelString(Point point);
        string ToPercentString(Point point);
        string ToImplicitValueString(Point point);
        string ToPixelString(Size size);
        string ToColorString(Color color);
        string ToPixelString(Thickness thickness);
        string ToImplicitValueString(Thickness thickness);
        string ToUrlString(string url);
        string ToLinearGradientString(LinearGradientBrush brush);
        string ToRadialGradientString(RadialGradientBrush brush);
        string ToColorStopsString(IEnumerable<GradientStop> gradientStops);
        string ToColorString(SolidColorBrush brush);
        string ToImageString(LinearGradientBrush brush);
        string ToImageString(RadialGradientBrush brush);
        string ToImageString(ImageBrush brush);
        string ToFontStyleString(FontStyle fontStyle);
        string ToFontStretchString(FontStretch fontStretch);
        string ToFontWeightString(FontWeight fontWeight);
        string ToTextAlignmentString(TextAlignment textAlignment);
        string ToOverflowString(ScrollBarVisibility scrollBarVisibility);
        string ToHtmlContentString(string value);
        string ToWrapString(TextWrapping textWrapping);
        string ToWhiteSpaceString(TextWrapping textWrapping);
        string ToFontFamilyNamesString(FontFamily fontFamily);
        string ToBooleanString(bool value);
        string ToMimeTypeString(RenderImageType renderImageType);
        string ToCursorString(Cursor cursor);
        string ToTransformString(Matrix matrix);

        MouseButton ConvertBackMouseButton(short buttonIndex);
        Key ConvertBackKey(int keyCode, int location);
    }

    public static class HtmlValueConverterExtensions
    {
        public static string ToImageString(this IHtmlValueConverter converter, Brush brush)
        {
            if (brush is LinearGradientBrush)
            {
                return converter.ToImageString((LinearGradientBrush)brush);
            }

            if (brush is RadialGradientBrush)
            {
                return converter.ToImageString((RadialGradientBrush)brush);
            }

            if (brush is ImageBrush)
            {
                return converter.ToImageString((ImageBrush)brush);
            }

            throw new Granular.Exception("Unexpected brush type \"{0}\"", brush.GetType());
        }
    }

    public class HtmlValueConverter : IHtmlValueConverter
    {
        public static readonly HtmlValueConverter Default = new HtmlValueConverter();

        private HtmlValueConverter()
        {
            //
        }

        public string ToPixelString(double value)
        {
            if (value.IsNaN() || !Double.IsFinite(value))
            {
                throw new Granular.Exception("Can't convert {0} to pixel string", value);
            }

            return String.Format("{0}px", Math.Round(value, 2));
        }

        public string ToPercentString(double value)
        {
            if (value.IsNaN())
            {
                throw new Granular.Exception("Can't convert Double.NaN to percent string");
            }

            return String.Format("{0}%", Math.Round(value * 100, 2));
        }

        public string ToDegreesString(double value)
        {
            if (value.IsNaN())
            {
                throw new Granular.Exception("Can't convert Double.NaN to degrees string");
            }

            return String.Format("{0}deg", Math.Round(value, 2));
        }

        public string ToImplicitValueString(double value)
        {
            return String.Format("{0}", Math.Round(value, 2));
        }

        public string ToPixelString(Point point)
        {
            return String.Format("{0} {1}", ToPixelString(point.X), ToPixelString(point.Y));
        }

        public string ToPercentString(Point point)
        {
            return String.Format("{0} {1}", ToPercentString(point.X), ToPercentString(point.Y));
        }

        public string ToImplicitValueString(Point point)
        {
            return String.Format("{0} {1}", ToImplicitValueString(point.X), ToImplicitValueString(point.Y));
        }

        public string ToPixelString(Size size)
        {
            return String.Format("{0} {1}", ToPixelString(size.Width), ToPixelString(size.Height));
        }

        public string ToColorString(Color color)
        {
            return color.A == 255 ? String.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B) :
                String.Format("rgba({0}, {1}, {2}, {3})", color.R, color.G, color.B, Math.Round((double)color.A / 255, 2));
        }

        public string ToPixelString(Thickness thickness)
        {
            return thickness.IsUniform ? ToPixelString(thickness.Top) :
                String.Format("{0} {1} {2} {3}", ToPixelString(thickness.Top), ToPixelString(thickness.Right), ToPixelString(thickness.Bottom), ToPixelString(thickness.Left));
        }

        public string ToImplicitValueString(Thickness thickness)
        {
            return String.Format("{0} {1} {2} {3}", ToImplicitValueString(thickness.Top), ToImplicitValueString(thickness.Right), ToImplicitValueString(thickness.Bottom), ToImplicitValueString(thickness.Left));
        }

        public string ToUrlString(string url)
        {
            return String.Format("url({0})", url);
        }

        public string ToLinearGradientString(LinearGradientBrush brush)
        {
            IEnumerable<GradientStop> gradientStops = brush.SpreadMethod == GradientSpreadMethod.Reflect ? GetReflectedGradientStops(brush.GradientStops) : brush.GradientStops;
            string repeatingPrefix = brush.SpreadMethod == GradientSpreadMethod.Repeat ? "repeating-" : String.Empty;

            return String.Format("{0}linear-gradient({1}, {2})", repeatingPrefix, ToDegreesString(brush.Angle + 90), ToColorStopsString(gradientStops));
        }

        public string ToRadialGradientString(RadialGradientBrush brush)
        {
            IEnumerable<GradientStop> gradientStops = brush.SpreadMethod == GradientSpreadMethod.Reflect ? GetReflectedGradientStops(brush.GradientStops) : brush.GradientStops;
            string repeatingPrefix = brush.SpreadMethod == GradientSpreadMethod.Repeat ? "repeating-" : String.Empty;

            return String.Format("{0}radial-gradient(ellipse {1} {2} at {3}, {4})", repeatingPrefix, ToPercentString(brush.RadiusX), ToPercentString(brush.RadiusY), ToPercentString(brush.GradientOrigin), ToColorStopsString(gradientStops));
        }

        public string ToColorStopsString(IEnumerable<GradientStop> gradientStops)
        {
            return gradientStops.Select(gradientStop => String.Format("{0} {1}", ToColorString(gradientStop.Color), ToPercentString(gradientStop.Offset))).DefaultIfEmpty(String.Empty).Aggregate((s1, s2) => String.Format("{0}, {1}", s1, s2));
        }

        public string ToColorString(SolidColorBrush brush)
        {
            return ToColorString(brush.Color.ApplyOpacity(brush.Opacity));
        }

        public string ToImageString(LinearGradientBrush brush)
        {
            return ToLinearGradientString(brush);
        }

        public string ToImageString(RadialGradientBrush brush)
        {
            return ToRadialGradientString(brush);
        }

        public string ToImageString(ImageBrush brush)
        {
            return ToUrlString(brush.ImageSource);
        }

        private static IEnumerable<GradientStop> GetReflectedGradientStops(IEnumerable<GradientStop> gradientStops)
        {
            return gradientStops.Select(gradientStop => new GradientStop(gradientStop.Color, gradientStop.Offset / 2)).
                Concat(gradientStops.Select(gradientStop => new GradientStop(gradientStop.Color, 1.0 - gradientStop.Offset / 2)).Reverse());
        }

        public string ToFontStyleString(FontStyle fontStyle)
        {
            switch (fontStyle)
            {
                case FontStyle.Normal: return "normal";
                case FontStyle.Italic: return "italic";
                case FontStyle.Oblique: return "oblique";
            }

            throw new Granular.Exception("Unexpected FontStyle \"{0}\"", fontStyle);
        }

        public string ToFontStretchString(FontStretch fontStretch)
        {
            switch (fontStretch)
            {
                case FontStretch.UltraCondensed: return "ultra-condensed";
                case FontStretch.ExtraCondensed: return "extra-condensed";
                case FontStretch.Condensed: return "condensed";
                case FontStretch.SemiCondensed: return "semi-condensed";
                case FontStretch.Medium:
                case FontStretch.Normal: return "normal";
                case FontStretch.SemiExpanded: return "semi-expanded";
                case FontStretch.Expanded: return "expanded";
                case FontStretch.ExtraExpanded: return "extra-expanded";
                case FontStretch.UltraExpanded: return "ultra-expanded";
            }

            throw new Granular.Exception("Unexpected FontStretch \"{0}\"", fontStretch);
        }

        public string ToFontWeightString(FontWeight fontWeight)
        {
            switch (fontWeight)
            {
                case FontWeight.Thin: return "100";
                case FontWeight.ExtraLight:
                case FontWeight.UltraLight: return "200";
                case FontWeight.Light: return "300";
                case FontWeight.Normal:
                case FontWeight.Regular: return "400";
                case FontWeight.Medium: return "500";
                case FontWeight.DemiBold:
                case FontWeight.SemiBold: return "600";
                case FontWeight.Bold: return "700";
                case FontWeight.ExtraBold:
                case FontWeight.UltraBold: return "800";
                case FontWeight.Black:
                case FontWeight.Heavy: return "900";
                case FontWeight.ExtraBlack:
                case FontWeight.UltraBlack: return "950";
            }

            throw new Granular.Exception("Unexpected FontWeight \"{0}\"", fontWeight);
        }

        public string ToTextAlignmentString(TextAlignment textAlignment)
        {
            switch (textAlignment)
            {
                case TextAlignment.Left: return "left";
                case TextAlignment.Right: return "right";
                case TextAlignment.Center: return "center";
                case TextAlignment.Justify: return "justify";
            }

            throw new Granular.Exception("Unexpected TextAlignment \"{0}\"", textAlignment);
        }

        public string ToOverflowString(ScrollBarVisibility scrollBarVisibility)
        {
            switch (scrollBarVisibility)
            {
                case ScrollBarVisibility.Disabled: return "hidden";
                case ScrollBarVisibility.Auto: return "auto";
                case ScrollBarVisibility.Hidden: return "hidden";
                case ScrollBarVisibility.Visible: return "scroll";
            }

            throw new Granular.Exception("Unexpected ScrollBarVisibility \"{0}\"", scrollBarVisibility);
        }

        public string ToHtmlContentString(string value)
        {
            return value.Replace(Environment.NewLine, "<br/>");
        }

        public string ToWrapString(TextWrapping textWrapping)
        {
            switch (textWrapping)
            {
                case TextWrapping.Wrap: return "soft";
                case TextWrapping.NoWrap: return "off";
            }

            throw new Granular.Exception("Unexpected TextWrapping \"{0}\"", textWrapping);
        }

        public string ToWhiteSpaceString(TextWrapping textWrapping)
        {
            switch (textWrapping)
            {
                case TextWrapping.Wrap: return "pre-wrap";
                case TextWrapping.NoWrap: return "pre";
            }

            throw new Granular.Exception("Unexpected TextWrapping \"{0}\"", textWrapping);
        }

        public string ToFontFamilyNamesString(FontFamily fontFamily)
        {
            return fontFamily.FamilyNames.Select(familyName => String.Format("\"{0}\"", familyName)).Aggregate((s1, s2) => String.Format("{0}, {1}", s1, s2));
        }

        public string ToBooleanString(bool value)
        {
            return value ? "true" : "false";
        }

        public string ToMimeTypeString(RenderImageType renderImageType)
        {
            switch (renderImageType)
            {
                case RenderImageType.Unknown: return String.Empty;
                case RenderImageType.Gif: return "image/gif";
                case RenderImageType.Jpeg: return "image/jpeg";
                case RenderImageType.Png: return "image/png";
                case RenderImageType.Svg: return "image/svg+xml";
            }

            throw new Granular.Exception("Unexpected RenderImageType \"{0}\"", renderImageType);
        }

        public string ToCursorString(Cursor cursor)
        {
            if (cursor == null)
            {
                return "default";
            }

            if (cursor.ImageSource != null)
            {
                string urlString = ToUrlString(((RenderImageSource)cursor.ImageSource.RenderImageSource).Url);

                return !Point.IsNullOrEmpty(cursor.Hotspot) ?
                    String.Format("{0} {1}, default", urlString, ToImplicitValueString(cursor.Hotspot)) :
                    String.Format("{0}, default", urlString);
            }

            switch (cursor.CursorType)
            {
                case CursorType.None: return "none";
                case CursorType.No: return "not-allowed";
                case CursorType.Arrow: return "default";
                case CursorType.AppStarting: return "progress";
                case CursorType.Cross: return "crosshair";
                case CursorType.Help: return "help";
                case CursorType.IBeam: return "text";
                case CursorType.SizeAll: return "move";
                case CursorType.SizeNESW: return "nesw-resize";
                case CursorType.SizeNS: return "ns-resize";
                case CursorType.SizeNWSE: return "nwse-resize";
                case CursorType.SizeWE: return "ew-resize";
                case CursorType.Wait: return "wait";
                case CursorType.Hand: return "pointer";

                case CursorType.UpArrow:
                case CursorType.Pen:
                case CursorType.ScrollNS:
                case CursorType.ScrollWE:
                case CursorType.ScrollAll:
                case CursorType.ScrollN:
                case CursorType.ScrollS:
                case CursorType.ScrollW:
                case CursorType.ScrollE:
                case CursorType.ScrollNW:
                case CursorType.ScrollNE:
                case CursorType.ScrollSW:
                case CursorType.ScrollSE:
                case CursorType.ArrowCD:
                    return "default";
            }

            throw new Granular.Exception("Unexpected CursorType \"{0}\"", cursor.CursorType);
        }

        public string ToTransformString(Matrix matrix)
        {
            return String.Format("matrix({0}, {1}, {2}, {3}, {4}, {5}",
                Math.Round(matrix.M11, 2),
                Math.Round(matrix.M12, 2),
                Math.Round(matrix.M21, 2),
                Math.Round(matrix.M22, 2),
                Math.Round(matrix.OffsetX, 2),
                Math.Round(matrix.OffsetY, 2));
        }

        public MouseButton ConvertBackMouseButton(short buttonIndex)
        {
            switch (buttonIndex)
            {
                case 0: return MouseButton.Left;
                case 1: return MouseButton.Middle;
                case 2: return MouseButton.Right;
            }

            throw new Granular.Exception("Unexpected button index \"{0}\"", buttonIndex);
        }

        public Key ConvertBackKey(int keyCode, int location)
        {
            switch (keyCode)
            {
                case 8: return Key.Back;
                case 9: return Key.Tab;
                case 13: return Key.Enter;
                case 16: return location == 1 ? Key.LeftShift : Key.RightShift;
                case 17: return location == 1 ? Key.LeftCtrl : Key.RightCtrl;
                case 18: return location == 1 ? Key.LeftAlt : Key.RightAlt;
                case 19: return Key.Pause;
                case 20: return Key.CapsLock;
                case 27: return Key.Escape;
                case 32: return Key.Space;
                case 33: return Key.PageUp;
                case 34: return Key.PageDown;
                case 35: return Key.End;
                case 36: return Key.Home;
                case 37: return Key.Left;
                case 38: return Key.Up;
                case 39: return Key.Right;
                case 40: return Key.Down;
                case 45: return Key.Insert;
                case 46: return Key.Delete;
                case 48: return Key.D0;
                case 49: return Key.D1;
                case 50: return Key.D2;
                case 51: return Key.D3;
                case 52: return Key.D4;
                case 53: return Key.D5;
                case 54: return Key.D6;
                case 55: return Key.D7;
                case 56: return Key.D8;
                case 57: return Key.D9;
                case 59: return Key.OemSemicolon;
                case 65: return Key.A;
                case 66: return Key.B;
                case 67: return Key.C;
                case 68: return Key.D;
                case 69: return Key.E;
                case 70: return Key.F;
                case 71: return Key.G;
                case 72: return Key.H;
                case 73: return Key.I;
                case 74: return Key.J;
                case 75: return Key.K;
                case 76: return Key.L;
                case 77: return Key.M;
                case 78: return Key.N;
                case 79: return Key.O;
                case 80: return Key.P;
                case 81: return Key.Q;
                case 82: return Key.R;
                case 83: return Key.S;
                case 84: return Key.T;
                case 85: return Key.U;
                case 86: return Key.V;
                case 87: return Key.W;
                case 88: return Key.X;
                case 89: return Key.Y;
                case 90: return Key.Z;
                case 91: return location == 1 ? Key.LWin : Key.RWin;
                case 93: return Key.Apps;
                case 96: return Key.NumPad0;
                case 97: return Key.NumPad1;
                case 98: return Key.NumPad2;
                case 99: return Key.NumPad3;
                case 100: return Key.NumPad4;
                case 101: return Key.NumPad5;
                case 102: return Key.NumPad6;
                case 103: return Key.NumPad7;
                case 104: return Key.NumPad8;
                case 105: return Key.NumPad9;
                case 106: return Key.Multiply;
                case 107: return Key.Add;
                case 109: return Key.Subtract;
                case 110: return Key.Decimal;
                case 111: return Key.Divide;
                case 112: return Key.F1;
                case 113: return Key.F2;
                case 114: return Key.F3;
                case 115: return Key.F4;
                case 116: return Key.F5;
                case 117: return Key.F6;
                case 118: return Key.F7;
                case 119: return Key.F8;
                case 120: return Key.F9;
                case 121: return Key.F10;
                case 122: return Key.F11;
                case 123: return Key.F12;
                case 144: return Key.NumLock;
                case 145: return Key.Scroll;
                case 173: return Key.OemMinus;
                case 188: return Key.OemComma;
                case 190: return Key.OemPeriod;
                case 191: return Key.OemQuestion;
                case 192: return Key.OemTilde;
                case 219: return Key.OemOpenBrackets;
                case 220: return Key.OemPipe;
                case 221: return Key.OemCloseBrackets;
                case 222: return Key.OemQuotes;
            }

            return Key.None;
        }
    }
}
