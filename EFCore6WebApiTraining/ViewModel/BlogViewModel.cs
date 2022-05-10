namespace EFCore6WebApiTraining.ViewModel
{
    public class BlogViewModel
    {
        public int BlogId { get; set; }

        public string? Url { get; set; }

        public List<PostViewModel>? Posts { get; set; }
    }
}
