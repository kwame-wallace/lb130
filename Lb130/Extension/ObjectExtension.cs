﻿using Newtonsoft.Json;
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
