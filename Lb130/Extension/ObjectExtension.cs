using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lb130.Extension
{
    static class ObjectExtension
    {
        public static string ToJsonString(this object obj)
        {
            var jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonConverter[] {new StringEnumConverter()});
            return jsonString;
        }
    }
}
