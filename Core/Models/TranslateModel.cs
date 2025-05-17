using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace snapwatch.Core.Models
{
    public class TranslateModel
    {
        [JsonPropertyName("source-language")]
        public string SourceLanguage { get; set; }

        [JsonPropertyName("source-text")]
        public string SourceText { get; set; }

        [JsonPropertyName("destination-language")]
        public string DestinationLanguage { get; set; }

        [JsonPropertyName("destination-text")]
        public string DestinationText { get; set; }

        [JsonPropertyName("pronunciation")]
        public Pronunciation Pronunciation { get; set; }

        [JsonPropertyName("translations")]
        public Translations Translations { get; set; }

        [JsonPropertyName("definitions")]
        public List<Definition> Definitions { get; set; }

        [JsonPropertyName("see-also")]
        public object SeeAlso { get; set; }
    }

    public class Pronunciation
    {
        [JsonPropertyName("source-text-phonetic")]
        public string SourceTextPhonetic { get; set; }

        [JsonPropertyName("source-text-audio")]
        public string SourceTextAudio { get; set; }

        [JsonPropertyName("destination-text-audio")]
        public string DestinationTextAudio { get; set; }
    }


    public class Translations
    {
        [JsonPropertyName("all-translations")]
        public List<List<object>> AllTranslations { get; set; }

        [JsonPropertyName("possible-translations")]
        public List<string> PossibleTranslations { get; set; }

        [JsonPropertyName("possible-mistakes")]
        public object PossibleMistakes { get; set; }
    }

    public class Definition
    {
        [JsonPropertyName("part-of-speech")]
        public string PartOfSpeech { get; set; }

        [JsonPropertyName("definition")]
        public string Text { get; set; }

        [JsonPropertyName("example")]
        public string Example { get; set; }

        [JsonPropertyName("other-examples")]
        public object OtherExamples { get; set; }

        [JsonPropertyName("synonyms")]
        public object Synonyms { get; set; }
    }
}
