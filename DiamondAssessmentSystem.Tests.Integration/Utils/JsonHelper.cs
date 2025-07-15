using System.Text;
using System.Text.Json;

namespace DiamondAssessmentSystem.Tests.Integration.Utils
{
    public static class JsonHelper
    {
        public static StringContent ToJsonContent<T>(T obj)
        {
            var json = JsonSerializer.Serialize(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
