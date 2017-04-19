using Lb130.Helper;
using Newtonsoft.Json;
using System.Drawing;

namespace Lb130.DTO
{
    public class TransitionLightState
    {

        [JsonProperty("on_off")]
        public int OnOff { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("hue")]
        public int Hue { get; set; }

        [JsonProperty("saturation")]
        public int Saturation { get; set; }

        [JsonProperty("color_temp")]
        public int ColorTemp { get; set; }

        [JsonProperty("brightness")]
        public int Brightness { get; set; }

        [JsonProperty("err_code")]
        public int ErrCode { get; set; }

        public string HtmlColor
        {
            get
            {
                var color = ImageUtils.FromHSLA(Hue/100.0, Saturation/100.0, Brightness/100.0, 1);
                var html = ColorTranslator.ToHtml(color);
                return html;
            }
        }
    }

    public class SmartlifeIotSmartbulbLightingservice
    {

        [JsonProperty("transition_light_state")]
        public TransitionLightState TransitionLightState { get; set; }
    }

    public class BulbState
    {

        [JsonProperty("smartlife.iot.smartbulb.lightingservice")]
        public SmartlifeIotSmartbulbLightingservice SmartlifeIotSmartbulbLightingservice { get; set; }
    }





}
