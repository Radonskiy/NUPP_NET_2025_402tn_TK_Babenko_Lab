using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PcPartsStore.Infrastructure;
using PcPartsStore.Infrastructure.Models;

namespace PcPartsStore.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly PcPartsStoreContext _db;

        public ProductsController(PcPartsStoreContext db)
        {
            _db = db;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductEntity>>> GetAll()
        {
            var items = await _db.Set<ProductEntity>()
                .AsNoTracking()
                .ToListAsync();

            return Ok(items);
        }

        // GET: api/products/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductEntity>> GetById(int id)
        {
            var item = await _db.Set<ProductEntity>().FindAsync(id);
            if (item == null) return NotFound();

            return Ok(item);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<ProductEntity>> Create([FromBody] ProductEntity entity)
        {
            _db.Set<ProductEntity>().Add(entity);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        // PUT: api/products/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductEntity entity)
        {
            if (id != entity.Id) return BadRequest("id в URL і entity.Id мають збігатися");

            _db.Entry(entity).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _db.Set<ProductEntity>().AnyAsync(x => x.Id == id);
                if (!exists) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Set<ProductEntity>().FindAsync(id);
            if (item == null) return NotFound();

            _db.Set<ProductEntity>().Remove(item);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
