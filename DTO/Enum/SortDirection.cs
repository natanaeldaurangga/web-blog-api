using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace LearnJwtAuth.DTO.Enum
{
    public enum SortDirection
    {
        [EnumMember(Value = "ASC")]
        [Display(Name = "Ascending")]
        ASC,

        // TODO: keyword search udh beres, tinggal sorting pake enum
        [EnumMember(Value = "DESC")]
        [Display(Name = "Descending")]
        DESC
    }
}