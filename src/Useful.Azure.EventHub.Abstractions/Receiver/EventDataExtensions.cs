using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace Useful.Azure.EventHub.Abstractions.Receiver
{
    internal static class EventDataExtensions
    {
        public static IEnumerable<T> ConvertEventData<T>(this IEnumerable<EventData> source)
        {
            return source.Where(eventData => eventData?.Body.Array != null)
                         .Select(eventData => eventData.ConvertEventData<T>());
        }

        public static T ConvertEventData<T>(this EventData source)
        {
            if (source?.Body.Array == null)
            {
                return default;
            }

            var bytes = Encoding.Default.GetString(source.Body.Array);
            return JsonConvert.DeserializeObject<T>(bytes);
        }
    }
}