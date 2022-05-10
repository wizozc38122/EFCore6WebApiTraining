using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
using EFCore6WebApiTraining.Repository.Entities;
using EFCore6WebApiTraining.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace EFCore6WebApiTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogGenericUnitOfWorkController : ControllerBase
    {
        private readonly IGenericUnitOfWork _unitOfWork;

        public BlogGenericUnitOfWorkController(IGenericUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // C
        [HttpPost]
        public async Task<IActionResult> CreateAsync(BlogCreateParameter blog)
        {
            _unitOfWork.GetRepository<Blog>().Create(new Blog() { Url = blog.Url });

            var count = await _unitOfWork.SaveChangeAsync();

            return Ok(count > 0);
        }

        // R
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _unitOfWork.GetRepository<Blog>().GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _unitOfWork.GetRepository<Blog>().GetAllAsync();

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
            _unitOfWork.GetRepository<Blog>().Update(blog);

            var count = await _unitOfWork.SaveChangeAsync();

            return Ok(count > 0);
        }


        // D
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var blogRepository = _unitOfWork.GetRepository<Blog>();

            var blog = await blogRepository.GetByIdAsync(id);

            if (blog == null) { return BadRequest(); }

            blogRepository.Delete(blog);

            var count = await _unitOfWork.SaveChangeAsync();

            return Ok(count > 0);
        }
    }
}
