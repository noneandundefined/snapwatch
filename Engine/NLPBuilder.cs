using System;
using System.Collections.Generic;
using System.Linq;

namespace snapwatch.Engine
{
    public class NLPBuilder
    {
        private readonly List<string> stopWords = new List<string>
        {
            "i", "me", "my", "myself", "we", "our", "ours", "us", "this", "these", "those",
            "he", "she", "it", "its", "itself", "them", "their", "by", "from", "into", "during",
            "after", "before", "above", "below", "but", "or", "as", "if", "then", "else", "so",
            "will", "would", "shall", "should", "can", "could", "may", "might", "must",
            "here", "there", "now", "just", "very", "too", "about", "again", "once", "been",
            "being", "both", "each", "few", "more", "most", "other", "some", "such", "what",
            "which", "why", "how"
        };

        private readonly char[] spltChars = new[] { ' ', ',', '.', '!', '?', ';', ':', '\t', '\n', '\r' };

        public List<string> Preprocess(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<string>();
            }

            List<string> output = input.ToLower()
                .Split(this.spltChars, StringSplitOptions.RemoveEmptyEntries)
                .SelectMany(word => word.Contains('-') ? word.Split('-') : new[] { word })
                .Where(word => word.Length > 2 && !this.stopWords.Contains(word) && !word.Any(char.IsDigit))
                .Select(word => this.Lemmatize(this.Stemming(word)))
                .Where(word => word.Length > 2)
                .ToList();

            return output;
        }

        private string Lemmatize(string input)
        {
            switch (input)
            {
                case "am": case "is": case "are": case "was": case "were": 
                    return "be";

                case "has": case "have": case "had": 
                    return "have";

                case "does": case "did": 
                    return "do";

                case "going": case "goes": case "went": 
                    return "go";

                case "made": case "making": 
                    return "make";

                case "saw": case "seen": case "seeing": 
                    return "see";

                case "came": case "coming": 
                    return "come";

                case "took": case "takes": case "taken": case "taking": 
                    return "take";

                case "better": case "best": 
                    return "good";

                case "worse": case "worst": 
                    return "bad";

                case "bigger": case "biggest": 
                    return "big";

                case "smaller": case "smallest": 
                    return "small";

                case "larger": case "largest": 
                    return "large";

                case "more": case "most": 
                    return "many";

                case "less": case "least": 
                    return "little";

                case "further": case "furthest": 
                    return "far";

                case "children": 
                    return "child";

                case "people": 
                    return "person";

                case "lives": 
                    return "life";

                case "wives": 
                    return "wife";

                case "fewer": case "fewest": 
                    return "few";

                default: 
                    return input;
            }
        }

        private string Stemming(string input)
        {
            string output = input;

            if (output.EndsWith("fulness")) return output.Substring(0, output.Length - 7);
            if (output.EndsWith("ousness")) return output.Substring(0, output.Length - 7);
            if (output.EndsWith("ization")) return output.Substring(0, output.Length - 7) + "ize";
            if (output.EndsWith("ational")) return output.Substring(0, output.Length - 7) + "ate";
            if (output.EndsWith("tional")) return output.Substring(0, output.Length - 6) + "tion";
            if (output.EndsWith("alize")) return output.Substring(0, output.Length - 5) + "al";
            if (output.EndsWith("icate")) return output.Substring(0, output.Length - 5) + "ic";
            if (output.EndsWith("ative")) return output.Substring(0, output.Length - 5);
            if (output.EndsWith("ement")) return output.Substring(0, output.Length - 5);
            if (output.EndsWith("ingly")) return output.Substring(0, output.Length - 5);
            if (output.EndsWith("fully")) return output.Substring(0, output.Length - 5);
            if (output.EndsWith("ably")) return output.Substring(0, output.Length - 4);
            if (output.EndsWith("ibly")) return output.Substring(0, output.Length - 4);

            if (output.EndsWith("ing"))
            {
                var stem = output.Substring(0, output.Length - 3);
                if (stem.Length > 0 && !"aeiou".Contains(stem[stem.Length - 1])) return stem + "e";

                return stem;
            }

            if (output.EndsWith("ies")) return output.Substring(0, output.Length - 3) + "y";
            if (output.EndsWith("ive")) return output.Substring(0, output.Length - 3);
            if (output.EndsWith("es")) return output.Substring(0, output.Length - 2);
            if (output.EndsWith("ly")) return output.Substring(0, output.Length - 2);

            if (output.EndsWith("ed"))
            {
                var stem = output.Substring(0, output.Length - 2);
                if (stem.Length > 0 && !"aeiou".Contains(stem[stem.Length - 1]))return stem + "e";

                return stem;
            }

            return output;
        }
    }
}
