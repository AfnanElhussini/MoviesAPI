using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        // How Posters BE

        private new List<string> _allowedExtenstions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048566;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _context.Movies.
                OrderByDescending(x => x.Rate).

                Include(m => m.Genre)
                .Select(m => new MovieDetailsDto
                {
                    Id = m.Id,
                    GenreId = m.GenreId,
                    GenreName = m.Genre.Name,
                    Poster = m.Poster,
                    Storyline = m.Storyline,
                    Title = m.Title,
                    Year = m.Year,

                }).ToListAsync();


            return Ok(movies);
        }
        [HttpGet(template: "{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);
            if (movie == null)
                return NotFound();
            var dto = new MovieDetailsDto { Id = movie.Id,
                GenreId = movie.GenreId,
                GenreName = movie.Genre.Name,
                Poster = movie.Poster,
                Rate = movie.Rate,
                Storyline = movie.Storyline,
                Title = movie.Title,
                Year = movie.Year,
            };
            return Ok(dto); }


        [HttpGet("GetByGenreIdAsync")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _context.Movies
                .Where(m => m.GenreId == genreId)
                .OrderByDescending(x => x.Rate)
                .Include(m => m.Genre)
                .Select(m => new MovieDetailsDto
                {
                    Id = m.Id,
                    GenreId = m.GenreId,
                    GenreName = m.Genre.Name,
                    Poster = m.Poster,
                    Rate = m.Rate,
                    Storyline = m.Storyline,
                    Title = m.Title,
                    Year = m.Year,

                }).ToListAsync();
            return Ok(movies);




        }



        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            // Before Dealing with file Check  File Size and Extensions
            if (dto.Poster == null)
                return BadRequest("Poster is Requierd");

            if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only .png and .jpg are allowed");

            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("Max allowed Size is 1 MB!");

            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid Genre ID");

            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);

            // save movie to database
            var movie = new Movie
            {

                GenreId = dto.GenreId,
                Title = dto.Title,
                Poster = dataStream.ToArray(),
                Rate = dto.Rate,
                Storyline = dto.Storyline,
                Year = dto.Year,

            };

            await _context.AddAsync(movie);
            _context.SaveChanges();
            return Ok(movie);

        }

        [HttpPut(template:"{id}")]
        public async Task<IActionResult> UpdateAsync([FromBody] int id, [FromForm] MovieDto dto)
            //Select Movie From DB
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"No Movie was Found with ID{id}");

            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid Genre ID");

            if(dto.Poster != null)
            {
                if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png and .jpg are allowed");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Max allowed Size is 1 MB!");


                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);

                movie.Poster = dataStream.ToArray();
            }

            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.Year = dto.Year;
            movie.Storyline = dto.Storyline;
            movie.Rate = dto.Rate;

            _context.SaveChanges();
            return Ok(movie);

        }

        [HttpDelete(template:"{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"No Movie with Found With ID{id}");
            _context.Remove(movie);
            _context.SaveChanges();

        return Ok(movie);
            
        }

    }


}
