using System.ComponentModel.DataAnnotations;

namespace EFCore6WebApiTraining.Parameters
{
    public class PostUpdateParameter
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Content { get; set; }
    }
}
