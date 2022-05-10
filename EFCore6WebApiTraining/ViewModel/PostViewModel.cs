namespace EFCore6WebApiTraining.ViewModel
{
    public class PostViewModel
    {
        public int PostId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }

        public int BlogId { get; set; }
    }
}
