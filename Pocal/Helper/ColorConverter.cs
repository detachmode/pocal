using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocal.Helper
{
    public static class ColorConverter
    {

        private static void TestToRGB(HSB hsb)
        {
            Console.WriteLine("Converting:");
            Console.WriteLine("H: " + hsb.H);
            Console.WriteLine("S: " + hsb.S);
            Console.WriteLine("B: " + hsb.B);
            RGB rgb = ConvertToRGB(hsb);
            Console.WriteLine("Result:");
            Console.WriteLine("R: " + rgb.R);
            Console.WriteLine("G: " + rgb.G);
            Console.WriteLine("B: " + rgb.B);
            HSB hsb2 = ConvertToHSB(rgb);
            Console.WriteLine("Converting back:");
            Console.WriteLine("H: " + hsb2.H);
            Console.WriteLine("S: " + hsb2.S);
            Console.WriteLine("B: " + hsb2.B);
            Console.WriteLine("Done.\r\n");
        }

        private static void TestToHSB(RGB rgb)
        {
            Console.WriteLine("Converting:");
            Console.WriteLine("R: " + rgb.R);
            Console.WriteLine("G: " + rgb.G);
            Console.WriteLine("B: " + rgb.B);
            HSB hsb = ConvertToHSB(rgb);
            Console.WriteLine("Result:");
            Console.WriteLine("H: " + hsb.H);
            Console.WriteLine("S: " + hsb.S);
            Console.WriteLine("B: " + hsb.B);
            RGB rgb2 = ConvertToRGB(hsb);
            Console.WriteLine("Converting back:");
            Console.WriteLine("H: " + rgb2.R);
            Console.WriteLine("S: " + rgb2.G);
            Console.WriteLine("B: " + rgb2.B);
            Console.WriteLine("Done.\r\n");
        }

        public static RGB ConvertToRGB(HSB hsb)
        {
            double chroma = hsb.S * hsb.B;
            double hue2 = hsb.H / 60;
            double x = chroma * (1 - Math.Abs(hue2 % 2 - 1));
            double r1 = 0d;
            double g1 = 0d;
            double b1 = 0d;
            if (hue2 >= 0 && hue2 < 1)
            {
                r1 = chroma;
                g1 = x;
            }
            else if (hue2 >= 1 && hue2 < 2)
            {
                r1 = x;
                g1 = chroma;
            }
            else if (hue2 >= 2 && hue2 < 3)
            {
                g1 = chroma;
                b1 = x;
            }
            else if (hue2 >= 3 && hue2 < 4)
            {
                g1 = x;
                b1 = chroma;
            }
            else if (hue2 >= 4 && hue2 < 5)
            {
                r1 = x;
                b1 = chroma;
            }
            else if (hue2 >= 5 && hue2 <= 6)
            {
                r1 = chroma;
                b1 = x;
            }
            double m = hsb.B - chroma;
            return new RGB()
            {
                R = r1 + m,
                G = g1 + m,
                B = b1 + m
            };
        }

        public static HSB ConvertToHSB(RGB rgb)
        {
            double r = rgb.R;
            double g = rgb.G;
            double b = rgb.B;

            double max = Max(r, g, b);
            double min = Min(r, g, b);
            double chroma = max - min;
            double hue2 = 0d;
            if (chroma != 0)
            {
                if (max == r)
                {
                    hue2 = (g - b) / chroma;
                }
                else if (max == g)
                {
                    hue2 = (b - r) / chroma + 2;
                }
                else
                {
                    hue2 = (r - g) / chroma + 4;
                }
            }
            double hue = hue2 * 60;
            if (hue < 0)
            {
                hue += 360;
            }
            double brightness = max;
            double saturation = 0;
            if (chroma != 0)
            {
                saturation = chroma / brightness;
            }
            return new HSB()
            {
                H = hue,
                S = saturation,
                B = brightness
            };
        }

        private static double Max(double d1, double d2, double d3)
        {
            if (d1 > d2)
            {
                return Math.Max(d1, d3);
            }
            return Math.Max(d2, d3);
        }

        private static double Min(double d1, double d2, double d3)
        {
            if (d1 < d2)
            {
                return Math.Min(d1, d3);
            }
            return Math.Min(d2, d3);
        }
    }

    public struct RGB
    {
        public double R;
        public double G;
        public double B;
    }

    public struct HSB
    {
        public double H;
        public double S;
        public double B;
    }
}
