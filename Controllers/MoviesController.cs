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
                splitOn: "Id" // Specify what column to split on (the second tables Id column in this case) 
            );

            return Ok(movies);
        }

        // GET api/movies/{id}
        [HttpGet("{id:int}")] // Flagging to the get reques
        public async Task<ActionResult<Movie>> GetMovieById(int id)
        {
            var sql = @"SELECT m.*, g.*, a.*, r.*, u.*
                FROM Movie m
                INNER JOIN Genre g ON m.GenreId = g.Id
                INNER JOIN AudienceRating a ON m.AudienceRatingId = a.Id
                LEFT OUTER JOIN CriticReview r ON m.Id = r.MovieId
                AND r.IsPublished = 1
                LEFT OUTER JOIN UserSnapshot u ON u.UserId = r.CreatedBy
                WHERE m.Id = @Id";

            // Create a null Movie object that we populate from the query results
            Movie? movie = null;

           await _db.QueryAsync<Movie, Genre, AudienceRating, Review, User, Movie>( // Table 1, table 2, table 3, return type
                sql,
                                (m, g, a, r, u) =>
                                {
                                    // Populate the movie object with the first row of data
                                    if (movie == null)
                                    {
                                        movie = m;
                                        movie.AudienceRating = a;
                                        movie.Genre = g;
                                        movie.Reviews = new List<Review>();
                                    }

                                    // Add the review to the movie's Reviews list if it exists
                                    if (r != null)
                                    {
                                        r.User = u;
                                        movie.Reviews.Add(r);
                                    }

                                    return movie;
                                },
                new { Id = id }, // Dapper takes id and plugs it in, replace @Id in the sql 
                splitOn: "Id,Id,Id,UserId" // Specify what column to split on (the second tables Id column in this case) 
            );

            if (movie == null)
                return NotFound();

            return Ok(movie);
        }
    }
}