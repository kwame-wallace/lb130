using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Lb130.DTO;
using Newtonsoft.Json;

namespace Lb130.Helper
{
    public class Bulb
    {
        public static BulbState Send(string ip, string newState, Color? color, int temp, int brightness, int transition, int port = 9999)
        {
            var tempChange = string.Empty;
            if (temp > -1)
            {
                tempChange = $"\"color_temp\":{temp},";

                //if (color != null)
                //{
                //    Console.WriteLine($"Temperature {temp}K overrides color {color.Value} (the color is ignored)");
                //}

                //color = ImageUtils.GetRgbFromK(temp);
            }

            var colorchange = string.Empty;
            if (color != null)
            {
                var c = color.Value;
                var hue = Math.Round(c.GetHue(), 2);
                var saturation = Math.Round(c.GetSaturation() * 100, 2);
                var lightness = Math.Round(c.GetBrightness() * 100, 2);
                colorchange =
                    $"\"hue\":{hue},\"saturation\":{saturation},\"color_temp\":0,\"brightness\":{lightness},";
            }

            var bightnessChange = string.Empty;
            if (brightness > -1)
            {
                bightnessChange = $"\"brightness\":{brightness},";
            }

            var onOffChange = string.Empty;
            if (!string.IsNullOrWhiteSpace(newState))
            {
                onOffChange = $"\"on_off\":{(newState == "on" ? 1 : 0)},";
            }

            string jsonPayload =
                $"{{\"smartlife.iot.smartbulb.lightingservice\":{{\"transition_light_state\":{{{onOffChange}\"mode\":\"normal\",{colorchange}{tempChange}{bightnessChange}\"transition_period\":{transition},\"err_code\":0}}}}}}";

            Console.WriteLine($"jsonPayload: {jsonPayload}");
            Console.WriteLine();

            var response = SendToSmartDevice(ip, jsonPayload, SocketType.Dgram, ProtocolType.Udp, port);
            var jsonObject = JsonConvert.DeserializeObject<BulbState>(response);
            return jsonObject;
        }


        private static string SendToSmartDevice(string ip, string jsonPayload, SocketType socketType, ProtocolType protocolType, int port)
        {
            using (var sender = new Socket(AddressFamily.InterNetwork, socketType, protocolType))
            {
                IPEndPoint tpEndPoint;
                try
                {
                    tpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to connect exception: {e.Message}");
                    return string.Empty;
                }

                sender.Connect(tpEndPoint);
                sender.Send(Encrypt(jsonPayload, protocolType == ProtocolType.Tcp));
                var buffer = new byte[2048];
                sender.ReceiveTimeout = 5000;

                var bytesLen = sender.Receive(buffer);
                if (bytesLen > 0)
                {
                    return Decrypt(buffer.Take(bytesLen).ToArray(), protocolType == ProtocolType.Tcp);
                }
                else
                {
                    throw new Exception("No answer...something went wrong");
                }
            }
        }

        // https://github.com/iqmeta/tplink-smartplug

        private static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
        private static byte[] Encrypt(string payload, bool hasHeader = true)
        {
            byte key = 0xAB;
            var cipherBytes = new byte[payload.Length];
            var header = hasHeader ? BitConverter.GetBytes(ReverseBytes((UInt32)payload.Length)) : new byte[] { };
            for (var i = 0; i < payload.Length; i++)
            {
                cipherBytes[i] = Convert.ToByte(payload[i] ^ key);
                key = cipherBytes[i];
            }
            return header.Concat(cipherBytes).ToArray();
        }
        private static string Decrypt(byte[] cipher, bool hasHeader = true)
        {
            byte key = 0xAB;
            if (hasHeader)
                cipher = cipher.Skip(4).ToArray();
            var result = new byte[cipher.Length];

            for (var i = 0; i < cipher.Length; i++)
            {
                var nextKey = cipher[i];
                result[i] = (byte)(cipher[i] ^ key);
                key = nextKey;
            }
            return Encoding.UTF7.GetString(result);
        }

        public static bool WaitForBulb(string host)
        {
            var pinger = new Ping();
            try
            {
                var linebreak = false;
                var status = IPStatus.Unknown;

                for (var i = 0; i < 120; i++)
                {
                    var reply = pinger.Send(host);

                    if (reply != null)
                        status = reply.Status;

                    if (status == IPStatus.Success)
                        break;

                    Thread.Sleep(1000);

                    if (i > 2)
                    {
                        Console.Write($"lb130: Connecting... Waiting for bulb ({i} seconds)       \r");
                        linebreak = true;
                    }
                }

                if (linebreak)
                {
                    Console.WriteLine();
                }

                if (status != IPStatus.Success)
                {
                    Console.WriteLine("lb130: Unable to connect to Bulb in a reasonable amount of time");
                    return true;
                }
            }
            catch (PingException e)
            {
                Console.WriteLine("lb130: Unable to connect to Bulb");
                Console.WriteLine(e.Message);
                return true;
            }
            return false;
        }
    }

}
