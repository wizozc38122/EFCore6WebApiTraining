using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EFCore6WebApiTraining.Repository.Entities
{
    public class Blog
    {
        public int BlogId { get; set; }
        
        public string? Url { get; set; }

        public List<Post>? Posts { get; } = new();
    }
}
