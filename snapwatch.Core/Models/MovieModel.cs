using System.Collections.Generic;

namespace snapwatch.Core.Models
{
    public class MovieModel
    {
        [System.Text.Json.Serialization.JsonPropertyName("adult")]
        public bool Adult { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("genre_ids")]
        public List<ushort> GenreIds { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public uint Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("original_language")]
        public string OriginalLanguage { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("original_title")]
        public string OriginalTitle { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("overview")]
        public string Overview { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("popularity")]
        public float Popularity { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("title")]
        public string Title { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("video")]
        public bool Video { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("vote_count")]
        public ushort VoteCount { get; set; }
    }

    public class MoviesModel
    {
        [System.Text.Json.Serialization.JsonPropertyName("page")]
        public ushort Page { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("results")]
        public List<MovieModel> Results { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("total_pages")]
        public uint TotalPages { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("total_results")]
        public uint TotalResults { get; set; }
    }
}
