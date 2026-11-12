using MoviesAdminApi.DTOs;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MoviesAdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesAdminApiController : ControllerBase
    {
        private readonly IDbConnection _db;

        // Constructor
        public MoviesAdminApiController(IDbConnection db)
        {
            _db = db;
        }

        // GET api/movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetAllMovies()
        {
            var sql = @"SELECT m.*, g.*, a.*
                FROM Movie m
                INNER JOIN Genre g ON m.GenreId = g.Id
                INNER JOIN AudienceRating a ON m.AudienceRatingId = a.Id";

            var movies = await _db.QueryAsync<Movie, Genre, AudienceRating, Movie>(
                sql,
                (movie, genre, audienceRating) =>
                {
                    movie.Genre = genre;
                    movie.AudienceRating = audienceRating;
                    return movie;
                },
                splitOn: "Id"
            );

            return Ok(movies);
        }
    }
}