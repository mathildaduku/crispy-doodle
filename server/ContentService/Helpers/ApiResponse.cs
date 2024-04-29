using Newtonsoft.Json;

namespace ContentService.Helpers
{
    public class ApiResponse<T>
    {
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ResponseStatus Status { get; set; } = ResponseStatus.Success;
        public string Message { get; set; } = string.Empty;
        public T? Result { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
