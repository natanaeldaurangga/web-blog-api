using System.Runtime.Serialization;

namespace LearnJwtAuth.DTO.Enum
{
    public enum TrashFilter
    {
        [EnumMember(Value = "WithoutTrashed")]
        WithoutTrashed,

        [EnumMember(Value = "WithTrashed")]
        WithTrashed,

        [EnumMember(Value = "OnlyTrashed")]
        OnlyTrashed
    }
}