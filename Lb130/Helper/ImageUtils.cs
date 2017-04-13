using System;
using System.Drawing;

namespace Lb130.Helper
{
    //see https://gist.github.com/stasikos/06b02d18f570fc1eaa9f
    public class ImageUtils
    {

        public static Color GetRgbFromK(double temperature)
        {
            //  Used this: https://gist.github.com/paulkaplan/5184275 at the beginning
            //  based on http://stackoverflow.com/questions/7229895/display-temperature-as-a-color-with-c
            //  this answer: http://stackoverflow.com/a/24856307
            //  (so, just interpretation of pseudocode in Java)
            var x = (temperature / 1000);
            if ((x > 40))
            {
                x = 40;
            }

            double red;
            double green;
            double blue;
            //  R
            if ((temperature < 6527))
            {
                red = 1;
            }
            else
            {
                var redpoly = new [] {
                    4.93596077,
                    -1.29917429,
                    0.164810386,
                    -0.0116449912,
                    0.000486540872,
                    -1.19453511E-05,
                    1.59255189E-07,
                    -8.89357601E-10};
                red = Poly(redpoly, x);
            }

            //  G
            if ((temperature < 850))
            {
                green = 0;
            }
            else if ((temperature <= 6600))
            {
                var greenpoly = new [] {
                    -0.49593172,
                    1.08442658,
                    -0.917444217,
                    0.494501179,
                    -0.148487675,
                    0.0249910386,
                    -0.0022152853,
                    8.06118266E-05};
                green = Poly(greenpoly, x);
            }
            else
            {
                var greenpoly = new [] {
                    3.06119745,
                    -0.676337896,
                    0.0828276286,
                    -0.00572828699,
                    0.00023593113,
                    -5.73391101E-06,
                    7.58711054E-08,
                    -4.21266737E-10};
                green = Poly(greenpoly, x);
            }

            //  B
            if ((temperature < 1900))
            {
                blue = 0;
            }
            else if ((temperature < 6600))
            {
                var bluepoly = new [] {
                    0.493997706,
                    -0.859349314,
                    0.545514949,
                    -0.181694167,
                    0.0416704799,
                    -0.00601602324,
                    0.000480731598,
                    -1.61366693E-05};
                blue = Poly(bluepoly, x);
            }
            else
            {
                blue = 1;
            }

            red = Clamp(red, 0, 1)*100;
            blue = Clamp(blue, 0, 1)*100;
            green = Clamp(green, 0, 1)*100;
            return Color.FromArgb((int)red, (int)green, (int)blue);
        }

        public static double Poly(double[] coefficients, double x)
        {
            var result = coefficients[0];
            var xn = x;
            for (var i = 1; (i < coefficients.Length); i++)
            {
                result = (result
                            + (xn * coefficients[i]));
                xn = (xn * x);
            }

            return result;
        }

        public static double Clamp(double x, double min, double max)
        {
            return x < min ? min : ((x > max) ? max : x);
        }

        // http://stackoverflow.com/questions/2942/how-to-use-hsl-in-asp-net/2504318#2504318
        // Given H,S,L,A in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static Color FromHSLA(double h, double s, double l, double a)
        {
            double v;
            double r, g, b;

            if (a > 1.0)
                a = 1.0;

            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + s)) : (l + s - l * s);

            if (v > 0)
            {
                var m = l + l - v;
                var sv = (v - m) / v;
                h *= 6.0;
                var sextant = (int)h;
                var fract = h - sextant;
                var vsf = v * sv * fract;
                var mid1 = m + vsf;
                var mid2 = v - vsf;

                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;

                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;

                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;

                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;

                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;

                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }


            var red = Convert.ToByte(r * 255.0f);
            var green = Convert.ToByte(g * 255.0f);
            var blue = Convert.ToByte(b * 255.0f);
            var alpha = Convert.ToByte(a * 255.0f);
            var color = Color.FromArgb(alpha, red, green, blue);

            return color;
        }
    }
}
