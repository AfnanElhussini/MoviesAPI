using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetActionAsync()
        {
            var geners = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
            return Ok(geners);

        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateGenreDto dto)
        {
            var genre = new Genre {Name = dto.Name};
          await  _context.Genres.AddAsync(genre);
            _context.SaveChanges();
            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateGenreDto dto)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
            {
                return NotFound($"No genre was found with ID: {id}");
            }

            genre.Name = dto.Name;
            _context.SaveChanges();

            return Ok(genre);
        }
        [HttpDelete]

        public async Task<IActionResult> DeleteAsync (int id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id ==id);
            if (genre == null)
                return NotFound($"No Genre was Found With ID: {id}");
            _context.Genres.Remove(genre);
            _context.SaveChanges();
            return Ok(genre);
        }

    }
}
