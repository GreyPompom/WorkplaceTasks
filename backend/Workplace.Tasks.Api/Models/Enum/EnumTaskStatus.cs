

using System.Text.Json.Serialization;

namespace Workplace.Tasks.Api.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))] //aceita string sem precisar 0, 1, 2
    public enum EnumTaskStatus
    {
        Pending,
        InProgress,
        Done
    }

}
