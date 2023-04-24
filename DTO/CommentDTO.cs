using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LearnJwtAuth.DTO
{
    public class CommentDTO
    {
        [MaxLength(255)]
        public string? BlogSecureId { get; set; }

        [DataType(DataType.Text)]
        public string? Content { get; set; }
    }
}