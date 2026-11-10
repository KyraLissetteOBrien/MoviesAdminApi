using MoviesAdminApi.DTOs;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MoviesAdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesAdminApi : ControllerBase
    {
        private readonly IDbConnection _db;

        // Constructor
        public MoviesAdminApi(IDbConnection db)
        {
            _db = db;
        }

        // GET api/movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetAll()
        {
            var sql = "SELECT * FROM Movie";

            var movies = await _db.QueryAsync<Movie>(sql);

            return Ok(movies);
        }
    }
}