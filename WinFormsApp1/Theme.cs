using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace WinFormsApp1 {
    public static class Theme {

        public static bool IsDark { get; private set; } = false;

        public static void Toggle() => IsDark = !IsDark;

        public static Color Background => IsDark ? Color.FromArgb(28, 28, 42) : Color.FromArgb(245, 245, 252);
        public static Color Foreground => IsDark ? Color.FromArgb(130, 130, 160) : Color.FromArgb(70, 70, 100);
        public static Color BackgroundFocused => IsDark ? Color.FromArgb(18, 18, 28) : Color.FromArgb(228, 228, 240);
        public static Color ColAccent => IsDark ? Color.FromArgb(30, 150, 70) : Color.FromArgb(20, 130, 55);
        public static Color ColText => IsDark ? Color.FromArgb(255, 255, 255) : Color.FromArgb(20, 20, 40);
        public static Color ColSubtext => IsDark ? Color.FromArgb(130, 130, 160) : Color.FromArgb(110, 110, 140);
        public static Color ColDanger => IsDark ? Color.FromArgb(150, 20, 20) : Color.FromArgb(190, 30, 30);
        public static Color ColLogOut => IsDark ? Color.FromArgb(100, 220, 150) : Color.FromArgb(20, 140, 70);
        public static Color ColLogIn => IsDark ? Color.FromArgb(50, 120, 205) : Color.FromArgb(40, 100, 185);
        public static Color ColWarning => IsDark ? Color.FromArgb(205, 150, 50) : Color.FromArgb(180, 120, 20);

        public static void LoadColors(Control parent) {
            return;
            List<Control> headerList = GetAllControlsWithTag(parent, "Header");
            List<Control> footerList = GetAllControlsWithTag(parent, "Footer");
            List<Control> focusList = GetAllControlsWithTag(parent, "Focus");
            List<Control> mainList = GetAllControlsWithTag(parent, "Main");

            ApplyColors(headerList, Theme.BackgroundFocused, Theme.Foreground);
            ApplyColors(footerList, Theme.BackgroundFocused, Theme.Foreground);
            ApplyColors(focusList, Theme.BackgroundFocused, Theme.Foreground);
            ApplyColors(mainList, Theme.Background, Theme.Foreground);
        }


        //helper function to apply the colors to elements
        private static void ApplyColors(List<Control> control, Color back, Color fore) {
            foreach (Control c in control) {
                c.BackColor = back;
                c.ForeColor = fore;
            }
        }

        // recursively searches for given tag in the parents scope and returns the list of elements
        private static List<Control> GetAllControlsWithTag(Control parent, string tag) {
            List<Control> list = new List<Control>();

            foreach (Control c in parent.Controls) {
                c.Tag?.ToString().Split(",").ToList().ForEach(c1 => {
                    if (c1 == tag) {
                        list.Add(c);
                    }
                });
                if (c.HasChildren) {
                    list.AddRange(GetAllControlsWithTag(c, tag));
                }
            }
            return list;
        }
    }
}
/*
Header
Footer
Focus
Main
 
 
 */