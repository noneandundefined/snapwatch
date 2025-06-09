using System;
using System.Collections.Generic;

namespace snapwatch.Core.Models
{
    public class VideoMovieModel
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public uint Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("results")]
        public List<ResultsVideoMovieModel> Results { get; set; }
    }

    public class ResultsVideoMovieModel
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public string Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("iso_639_1")]
        public string Iso6391 { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("iso_3166_1")]
        public string Ico31661 { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("key")]
        public string Key { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("site")]
        public string Site { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("size")]
        public int Size { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        public string Type { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("official")]
        public bool Official { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("published_at")]
        public DateTime PublishedAt { get; set; }
    }
}
