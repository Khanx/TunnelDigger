using Newtonsoft.Json.Linq;
using Pipliz;
using UnityEngine.Assertions;

namespace TunnelDigger
{
    public static class Extender
    {
        public static T GetAsOrDefaultOrError<T>(this JObject token, string propertyName, T defaultValue)
        {
            Assert.IsFalse(typeof(T).IsSubclassOf(typeof(JToken)));
            JValue result = default(JValue);
            try
            {
                if (token?.TryGetValue(propertyName, out result) ?? false)
                {
                    return result.Value<T>();
                }
            }
            catch (System.Exception)    { return defaultValue;}

            return defaultValue;
        }
    }
}
