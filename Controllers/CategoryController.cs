using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.Data;
using LearnJwtAuth.DTO;
using LearnJwtAuth.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnJwtAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext? _context;

        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ILogger<CategoryController> logger, AppDbContext context, ILogger<CategoryController> logger2)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context!.Categories.ToListAsync());
        }

        [HttpPost, Authorize(Policy = "ADMIN")]
        public async Task<IActionResult> Add(CategoryDTO dto)
        {
            var category = new Category()
            {
                Name = dto.Name
            };

            _context!.Categories.Add(category);
            await _context!.SaveChangesAsync();
            return Ok(category);
        }

        [HttpDelete("{id}"), Authorize(Policy = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context!.Categories.FindAsync(id);
            if (category == null) return BadRequest();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }
    }
}