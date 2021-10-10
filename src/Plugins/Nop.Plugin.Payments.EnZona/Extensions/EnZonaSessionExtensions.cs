using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.EnZona.Extensions
{
    public static class EnZonaSessionExtensions
    {
        private const string TOKEN_KEY = "EnZonaApiToken";

        public static void SetEnZonaApiToken<T>(this ISession session, T value, string key = null)
        {
            key = string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key) ? TOKEN_KEY : key;
            session.SetString(key, JsonConvert.SerializeObject(value));
        }


        public static T GetEnZonaApiToken<T>(this ISession session, string key = null)
        {
            key = string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key) ? TOKEN_KEY : key;
            var value = session.GetString(key);
            if (value == null)
                return default;

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
