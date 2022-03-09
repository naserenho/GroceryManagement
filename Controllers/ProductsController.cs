#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroceryManagement.Data;
using GroceryManagement.Models;

namespace GroceryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("Count")]
        public async Task<ActionResult<IEnumerable<CountsDTO>>> GetProductsCounts()
        {
            return await _context.Products.GroupBy(x => x.Status).Select(x => new CountsDTO
            {
                Status = x.Key,
                Count = x.Count(),
            }).ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductDTO productdto)
        {
            if (_context.Products.Any(x => x.Id == id))
            {
                var product = _context.Products.Where(x => x.Id == id).FirstOrDefault();
                if (!string.IsNullOrEmpty(productdto.Barcode))
                    product.Barcode = productdto.Barcode;
                if (!string.IsNullOrEmpty(productdto.Name))
                    product.Name = productdto.Name;
                if (!string.IsNullOrEmpty(productdto.Description))
                    product.Description = productdto.Description;
                if (!string.IsNullOrEmpty(productdto.Status))
                    product.Status = StatusVal(productdto.Status);
                if (productdto.Weight != null)
                    product.Weight = productdto.Weight;
                _context.Entry(product).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();

                    return Ok(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }
            else
                return BadRequest("Product ID is invalid");
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductDTO productdto)
        {
            int? CategoryID = null;
            if (!string.IsNullOrEmpty(productdto.CategoryCode))
            {
                if (_context.Categories.Any(x => x.Code == productdto.CategoryCode))
                {
                    CategoryID = _context.Categories.Where(x => x.Code == productdto.CategoryCode).Select(x => x.Id).FirstOrDefault();
                }
                else
                    return BadRequest("Category Code is invalid");
            }
            Product product = DTOToProduct(productdto);
            product.CategoryID = CategoryID;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        private Product DTOToProduct(ProductDTO productdto)
        {
            return new Product
            {
                Barcode = productdto.Barcode,
                Name = productdto.Name,
                Description = productdto.Description,
                Status = StatusVal(productdto.Status),
                Weight = productdto.Weight
            };
        }

        private static string StatusVal(string Status)
        {
            if (!string.IsNullOrEmpty(Status) && (Status == "sold" || Status == "damaged"))
                return Status;
            return "inStock";
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("sell")]
        public async Task<ActionResult<Product>> SellProduct(string Barcode)
        {
            if (!string.IsNullOrEmpty(Barcode))
            {
                if (_context.Products.Any(x => x.Barcode == Barcode))
                {
                    var product = _context.Products.Where(x => x.Barcode == Barcode && x.Status == "inStock").FirstOrDefault();
                    if (product != null)
                    {
                        product.Status = "sold";
                        await _context.SaveChangesAsync();

                        return Ok(product);
                    }
                    else
                        return BadRequest($"Product is not in-stock. Either sold or damaged.");

                }
            }
            return BadRequest("Barcode is invalid");
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public class CountsDTO
        {
            public CountsDTO()
            {
            }

            public string Status { get; set; }
            public int Count { get; set; }
        }

        public class ProductDTO
        {
            public ProductDTO() { }
            public string Name { get; set; } = string.Empty;
            public string Barcode { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public double? Weight { get; set; }
            public string Status { get; set; } = string.Empty;
            public string CategoryCode { get; set; } = string.Empty;
        }
    }
}
