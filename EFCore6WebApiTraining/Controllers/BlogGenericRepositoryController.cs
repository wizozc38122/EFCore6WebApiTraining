using Microsoft.AspNetCore.Mvc;
using EFCore6WebApiTraining.Repository.Interfaces;
using EFCore6WebApiTraining.Repository.Entities;
using EFCore6WebApiTraining.Parameters;

namespace EFCore6WebApiTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogGenericRepositoryController : ControllerBase
    {
        private readonly IGenericRepository<Blog> _blogRepository;

        public BlogGenericRepositoryController(IGenericRepository<Blog> blogRepository) => _blogRepository = blogRepository;

        // C
        [HttpPost]
        public async Task<IActionResult> CreateAsync(BlogCreateParameter blog)
        {
            var result = await _blogRepository.CreateAsync(new Blog() { Url = blog.Url });

            return Ok(result);
        }

        // R
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _blogRepository.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _blogRepository.GetAllAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // U
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(Blog blog)
        {
            var result = await _blogRepository.UpdateAsync(blog);

            return Ok(result);
        }


        // D
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var result = await _blogRepository.DeleteByIdAsync(id);

            return Ok(result);
        }
    }
}
