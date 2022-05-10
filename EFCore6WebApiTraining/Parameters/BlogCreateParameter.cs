using System.ComponentModel.DataAnnotations;

namespace EFCore6WebApiTraining.Parameters
{
    public class BlogCreateParameter
    {
        [Required]
        public string? Url { get; set; }
    }
}
