using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace snapwatch.Core.Models
{
    public class MovieModel
    {
        [JsonPropertyName("adult")]
        public bool Adult { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonPropertyName("genre_ids")]
        public List<ushort> GenreIds { get; set; }

        [JsonPropertyName("id")]
        public uint Id { get; set; }

        [JsonPropertyName("original_language")]
        public string OriginalLanguage { get; set; }

        [JsonPropertyName("original_title")]
        public string OriginalTitle { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("popularity")]
        public float Popularity { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("video")]
        public bool Video { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public ushort VoteCount { get; set; }
    }

    public class MoviesModel
    {
        [JsonPropertyName("page")]
        public ushort Page { get; set; }

        [JsonPropertyName("results")]
        public List<MovieModel> Results { get; set; }

        [JsonPropertyName("total_pages")]
        public uint TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public uint TotalResults { get; set; }
    }
}
