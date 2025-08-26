using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace GuestRegistration.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventView
{
    [EnumMember(Value = "future")]
    Future,
    
    [EnumMember(Value = "past")]
    Past
}