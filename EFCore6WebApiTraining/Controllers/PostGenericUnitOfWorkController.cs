using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
using EFCore6WebApiTraining.Repository.Entities;
using EFCore6WebApiTraining.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace EFCore6WebApiTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostGenericUnitOfWorkController : ControllerBase
    {
        private readonly IGenericUnitOfWork _unitOfWork;

        public PostGenericUnitOfWorkController(IGenericUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        // C
        [HttpPost]
        public async Task<IActionResult> CreateAsync(PostCreateParameter post)
        {
            var blog = _unitOfWork.GetRepository<Blog>().GetByIdAsync(post.BlogId);

            if (blog == null) { return BadRequest(); }

            _unitOfWork.GetRepository<Post>().Create(new Post()
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
            var result = await _unitOfWork.GetRepository<Post>().GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _unitOfWork.GetRepository<Post>().GetAllAsync();

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
            var target = await _unitOfWork.GetRepository<Post>().GetByIdAsync(post.PostId);

            if (target == null) { return BadRequest(); }

            target.Title = post.Title;
            target.Content = post.Content;

            _unitOfWork.GetRepository<Post>().Update(target);

            var count = await _unitOfWork.SaveChangeAsync();

            return Ok(count > 0);
        }


        // D
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var postRepository = _unitOfWork.GetRepository<Post>();


            var post = await postRepository.GetByIdAsync(id);

            if(post == null) { return BadRequest(); }

            postRepository.Delete(post);

            var count = await _unitOfWork.SaveChangeAsync();

            return Ok(count > 0);
        }
    }
}
