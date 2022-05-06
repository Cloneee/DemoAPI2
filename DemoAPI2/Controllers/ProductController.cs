using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DemoAPI2.Models;
using Microsoft.AspNetCore.Authorization;

namespace DemoAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _context;
        public ProductController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get()
        {
            return Ok(await _context.Products.ToListAsync());
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Not found");
            }
            return Ok(product);
        }
        [HttpPost]
        public async Task<ActionResult<List<Product>>> AddProduct(ProductDTO product)
        {
            var newProduct = new Product(product.Name, product.Description, product.Price);
            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();
            return Ok(newProduct);
        }
        [HttpPut]
        public async Task<ActionResult<List<Product>>> UpdateProduct(Product request)
        {
            var dbProduct = await _context.Products.FindAsync(request.Id);
            if (dbProduct == null)
            {
                return NotFound("Not found");
            }
            dbProduct.Name = request.Name;
            dbProduct.Description = request.Description;
            dbProduct.Price = request.Price;
            await _context.SaveChangesAsync();
            return Ok(dbProduct);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Not found");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }
    }
}
