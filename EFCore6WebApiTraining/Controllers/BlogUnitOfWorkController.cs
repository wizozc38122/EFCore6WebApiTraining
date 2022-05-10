using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
using EFCore6WebApiTraining.Repository.Entities;
using EFCore6WebApiTraining.Parameters;
using EFCore6WebApiTraining.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace EFCore6WebApiTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogUnitOfWorkController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BlogUnitOfWorkController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // C
        [HttpPost]
        public async Task<IActionResult> CreateAsync(BlogCreateParameter blog)
        {

            _unitOfWork.Blogs.Create(new Blog() { Url = blog.Url });

            var count = await _unitOfWork.SaveChangeAsync();

            return Ok(count > 0);
        }

        // R
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _unitOfWork.Blogs.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }



            return Ok(new BlogViewModel()
            {
                BlogId = result.BlogId,
                Url = result.Url,
                Posts = result.Posts?.Select(post => new PostViewModel()
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Content = post.Content,
                    BlogId = post.BlogId,
                }).ToList()
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _unitOfWork.Blogs.GetAllAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result.Select(blog => new BlogViewModel()
            {
                BlogId = blog.BlogId,
                Url = blog.Url,
                Posts = blog.Posts?.Select(post => new PostViewModel()
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Content = post.Content,
                    BlogId = post.BlogId,
                }).ToList(),
            }));
        }

        // U
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(Blog blog)
        {
            _unitOfWork.Blogs.Update(blog);

            var count = await _unitOfWork.SaveChangeAsync();

            return Ok(count > 0);
        }


        // D
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var blog = await _unitOfWork.Blogs.GetByIdAsync(id);

            if (blog == null) { return BadRequest(); }

            _unitOfWork.Blogs.Delete(blog);

            var count = await _unitOfWork.SaveChangeAsync();

            return Ok(count > 0);
        }
    }
}
