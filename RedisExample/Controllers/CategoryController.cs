using Microsoft.AspNetCore.Mvc;
using RedisExample.Models;
using RedisExample.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController:ControllerBase
    {
        CategoryService _categoryService;
        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }
         
        [HttpPost("Add")]
        public async Task<IActionResult> Add(Category category)
        {
            category.Id = Guid.NewGuid().ToString("N");
            return Ok(await _categoryService.Add(category));
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(Category category)
        {
            return Ok(await _categoryService.Update(category));

        }
        
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _categoryService.GetAll());
        }
    }
}
