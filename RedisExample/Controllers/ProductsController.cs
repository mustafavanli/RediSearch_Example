using Microsoft.AspNetCore.Http;
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
    public class ProductsController : ControllerBase
    {
        ProductService _productService;
        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(Product product)
        {
            product.Id = Guid.NewGuid().ToString("N"); 
            var newProduct = await _productService.Add(product);
            return Ok(newProduct);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _productService.GetAll());
        }
        [HttpPost("Update")]
        public async Task<IActionResult> Update(Product product)
        {
            return Ok(await _productService.Update(product));
        }
        [HttpGet("GetByCategoryId")]
        public async Task<IActionResult> GetCategoryId(int categoryId)
        {
            return Ok(await _productService.GetByCategoryId(categoryId));
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await _productService.Get(id));
        }

        [HttpGet("Range")]
        public async Task<IActionResult> Range(double min,double max)
        {
            return Ok(await _productService.Range(min,max));
        }

        [HttpGet("NameQuery")]
        public async Task<IActionResult> NameQuery(string name)
        {
            return Ok(await _productService.NameQuery(name));
        }
    }
}
