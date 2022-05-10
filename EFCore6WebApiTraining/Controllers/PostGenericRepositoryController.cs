using Microsoft.AspNetCore.Mvc;
using EFCore6WebApiTraining.Repository.Interfaces;
using EFCore6WebApiTraining.Repository.Entities;
using EFCore6WebApiTraining.Parameters;


namespace EFCore6WebApiTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostGenericRepositoryController : ControllerBase
    {
        private readonly IGenericRepository<Blog> _blogRepository;
        private readonly IGenericRepository<Post> _postRepository;

        public PostGenericRepositoryController(IGenericRepository<Blog> blogRepository, IGenericRepository<Post> postRepository)
        {
            _blogRepository = blogRepository;
            _postRepository = postRepository;
        }

        // C
        [HttpPost]
        public async Task<IActionResult> CreateAsync(PostCreateParameter post)
        {
            var blog = _blogRepository.GetByIdAsync(post.BlogId);

            if (blog == null) { return BadRequest(); }

            var result = await _postRepository.CreateAsync(new Post()
            {
                Title = post.Title,
                Content = post.Content,
                BlogId = post.BlogId,
            });

            return Ok(result);
        }

        // R
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _postRepository.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _postRepository.GetAllAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // U
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(PostUpdateParameter post)
        {
            var target = await _postRepository.GetByIdAsync(post.PostId);

            if(target == null) { return BadRequest(); }

            target.Title = post.Title;
            target.Content = post.Content;

            var result = await _postRepository.UpdateAsync(target);

            return Ok(result);
        }


        // D
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var result = await _postRepository.DeleteByIdAsync(id);

            return Ok(result);
        }
    }
}
