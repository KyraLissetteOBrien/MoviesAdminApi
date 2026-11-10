
namespace MoviesAdminApi.DTOs
{
    public class Movie
    {
        public int Id { get; set; }
        public int GenreId { get; set; }
        public int AudienceRatingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Runtime { get; set; }
        public string Synopsis { get; set; } = string.Empty;
        public DateTime? ReleaseDate { get; set; }
        public string FunFact { get; set; } = string.Empty;
        public string Directors { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}