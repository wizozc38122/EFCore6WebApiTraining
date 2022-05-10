using System.ComponentModel.DataAnnotations;


namespace EFCore6WebApiTraining.Parameters
{
    public class PostCreateParameter
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public int BlogId { get; set; }
    }
}
