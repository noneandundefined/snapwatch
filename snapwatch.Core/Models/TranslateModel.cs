using System.Collections.Generic;

namespace snapwatch.Core.Models
{
    public class TranslateModel
    {
        [System.Text.Json.Serialization.JsonPropertyName("source-language")]
        public string SourceLanguage { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("source-text")]
        public string SourceText { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("destination-language")]
        public string DestinationLanguage { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("destination-text")]
        public string DestinationText { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("pronunciation")]
        public Pronunciation Pronunciation { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("translations")]
        public Translations Translations { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("definitions")]
        public List<Definition> Definitions { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("see-also")]
        public object SeeAlso { get; set; }
    }

    public class Pronunciation
    {
        [System.Text.Json.Serialization.JsonPropertyName("source-text-phonetic")]
        public string SourceTextPhonetic { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("source-text-audio")]
        public string SourceTextAudio { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("destination-text-audio")]
        public string DestinationTextAudio { get; set; }
    }


    public class Translations
    {
        [System.Text.Json.Serialization.JsonPropertyName("all-translations")]
        public List<List<object>> AllTranslations { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("possible-translations")]
        public List<string> PossibleTranslations { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("possible-mistakes")]
        public object PossibleMistakes { get; set; }
    }

    public class Definition
    {
        [System.Text.Json.Serialization.JsonPropertyName("part-of-speech")]
        public string PartOfSpeech { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("definition")]
        public string Text { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("example")]
        public string Example { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("other-examples")]
        public object OtherExamples { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("synonyms")]
        public object Synonyms { get; set; }
    }
}
