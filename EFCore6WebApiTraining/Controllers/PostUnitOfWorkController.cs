using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
using EFCore6WebApiTraining.Repository.Entities;
using EFCore6WebApiTraining.Parameters;
using EFCore6WebApiTraining.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace EFCore6WebApiTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostUnitOfWorkController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostUnitOfWorkController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        // C
        [HttpPost]
        public async Task<IActionResult> CreateAsync(PostCreateParameter post)
        {
            var blog = _unitOfWork.Blogs.GetByIdAsync(post.BlogId);

            if (blog == null) { return BadRequest(); }

            _unitOfWork.Posts.Create(new Post()
            {
                Title = post.Title,
                Content = post.Content,
                BlogId = post.BlogId,
            });

            var count = await _unitOfWork.SaveChangeAsync();

            return Ok(count > 0);
        }

        // R
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _unitOfWork.Posts.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok( new PostViewModel() { 
                PostId = result.PostId,
                Title = result.Title,
                Content= result.Content,
                BlogId= result.BlogId,
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _unitOfWork.Posts.GetAllAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result.Select(post => new PostViewModel()
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                BlogId = post.BlogId,
            }));

        }

        // U
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(PostUpdateParameter post)
        {
            var target = await _unitOfWork.Posts.GetByIdAsync(post.PostId);

            if (target == null) { return BadRequest(); }

            target.Title = post.Title;
            target.Content = post.Content;

            _unitOfWork.Posts.Update(target);

            var count = await _unitOfWork.SaveChangeAsync();

            return Ok(count > 0);
        }


        // D
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var postRepository = _unitOfWork.Posts;


            var post = await postRepository.GetByIdAsync(id);

            if (post == null) { return BadRequest(); }

            postRepository.Delete(post);

            var count = await _unitOfWork.SaveChangeAsync();

            return Ok(count > 0);
        }
    }
}
