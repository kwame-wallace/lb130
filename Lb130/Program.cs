using Lb130.Extension;
using Lb130.Helper;
using NDesk.Options;
using System;
using System.Drawing;

namespace Lb130
{
    class Program
    {
        static void Main(string[] args)
        {
            // https://particle.hackster.io/msmaha/direct-photon-to-tp-link-device-control-2fb6e8
            // LB130 Bulb control parameters are
            //{"smartlife.iot.smartbulb.lightingservice":{"transition_light_state":{"on_off":1,"mode":"normal","hue":120,"saturation":65,"color_temp":0,"brightness":100,"err_code":0}}}

            var host = string.Empty;
            Color? color = null;
            var onOff = string.Empty;
            var help = false;
            var transition = 150;
            var temp = -1;
            var brightness = -1;

            var p = new OptionSet
            {
                {
                    "h|host=", "ip address of the device",
                    h =>
                    {
                        host = h.ToLower();
                        var check = Uri.CheckHostName(host);
                        if (check == UriHostNameType.Unknown)
                        {
                            throw new OptionException($"the host type is \"{check}\".", "host");
                        }
                    }
                },
                {
                    "k|temperature=", @"temperature K (see http://en.wikipedia.org/wiki/Color_temperature)",
                    (int t) => temp = t
                },
                {
                    "c|color=", "the hexadecimal color",
                    col =>
                    {
                        try
                        {
                            color = ColorTranslator.FromHtml(col);
                        }
                        catch(Exception e)
                        {
                            throw new OptionException("invalid color: " + col, "host");
                        }
                    }
                },
                {
                    "b|brightness=", @"brightness (0 - 100)",
                    (int b) => brightness = b
                },
                 {
                    "t|transition=", "transition time in milliseconds (1000 = 1 second)",
                    (int t) => transition = t
                },
                {
                    "s|switch=",
                    "on - switch the bulb on.\n" +
                    "off - switch the bulb off",
                    (string swicth) =>
                    {
                        onOff = swicth.ToLower();
                        if (onOff != "on" && onOff != "off")
                        {
                            if (string.IsNullOrWhiteSpace(host))
                                throw new OptionException("the light bulb switch can be \"on\" or \"off\".", "host");
                        }
                    }
                },
                {
                    "?|help", "show this message and exit",
                    v => help = v != null
                },
            };

            try
            {
                p.Parse(args);

                if (help)
                {
                    ShowHelp(p);
                    return;
                }

                if (string.IsNullOrWhiteSpace(host))
                    throw new OptionException("ip address is required", "host");

            }
            catch (OptionException e)
            {
                ShowHelp(p);
                return;
            }


            if (Bulb.QuitWaitingForBulb(host)) return;

            var response = Bulb.Send(host, onOff, color, temp, brightness, transition);
            Console.WriteLine($"response: {response.ToJsonString()}");
        }

        private static void ShowHelp(OptionSet p)
        {
            Console.WriteLine();
            Console.WriteLine("usage: lb130 [OPTIONS]");
            Console.WriteLine();
            Console.WriteLine("options:");
            p.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }
    }
}
