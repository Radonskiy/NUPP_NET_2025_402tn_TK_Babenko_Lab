using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PcPartsStore.Infrastructure;
using PcPartsStore.Infrastructure.Models;

namespace PcPartsStore.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly PcPartsStoreContext _db;

        public BrandsController(PcPartsStoreContext db)
        {
            _db = db;
        }

        // GET: api/brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandEntity>>> GetAll()
        {
            var items = await _db.Set<BrandEntity>()
                .AsNoTracking()
                .ToListAsync();

            return Ok(items);
        }

        // GET: api/brands/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BrandEntity>> GetById(int id)
        {
            var item = await _db.Set<BrandEntity>().FindAsync(id);
            if (item == null) return NotFound();

            return Ok(item);
        }

        // POST: api/brands
        [HttpPost]
        public async Task<ActionResult<BrandEntity>> Create([FromBody] BrandEntity entity)
        {
            _db.Set<BrandEntity>().Add(entity);
            await _db.SaveChangesAsync();

            // entity.Id має заповнитись після SaveChanges (SQLite AUTOINCREMENT)
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        // PUT: api/brands/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BrandEntity entity)
        {
            if (id != entity.Id) return BadRequest("id в URL і entity.Id мають збігатися");

            // Attach + Modified
            _db.Entry(entity).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _db.Set<BrandEntity>().AnyAsync(x => x.Id == id);
                if (!exists) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/brands/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Set<BrandEntity>().FindAsync(id);
            if (item == null) return NotFound();

            _db.Set<BrandEntity>().Remove(item);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
