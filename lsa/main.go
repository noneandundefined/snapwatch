package main

import (
	"encoding/json"
	"fmt"
	"math"
	"os"
	"regexp"
	"sort"
	"strings"
	"sync"
	"time"
	"unicode"

	"gonum.org/v1/gonum/mat"
)

type Movies struct {
	Page         uint16  `json:"page"`
	Results      []Movie `json:"results"`
	TotalPages   uint32  `json:"total_pages"`
	TotalResults uint32  `json:"total_results"`
}

type Movie struct {
	Adult            bool     `json:"adult"`
	BackdropPath     *string  `json:"backdrop_path"`
	GenreIds         []uint16 `json:"genre_ids"`
	Id               uint32   `json:"id"`
	OriginalLanguage string   `json:"original_language"`
	OriginalTitle    string   `json:"original_title"`
	Overview         string   `json:"overview"`
	Popularity       float32  `json:"popularity"`
	PosterPath       string   `json:"poster_path"`
	ReleaseDate      string   `json:"release_date"`
	Title            string   `json:"title"`
	Video            bool     `json:"video"`
	VoteAverage      float64  `json:"vote_average"`
	VoteCount        uint16   `json:"vote_count"`
}

var avgOverview uint8 = 39
var top int = 25
var vocabularyIndexMap map[string]int
var vocabulary []string
var tokenizedDocs [][]string
var idfCache map[string]float64

func TF(df int, N int) float32 {
	return float32(df) / float32(N)
}

func IDF(N int, df int) float64 {
	return math.Log(float64(N) / (1 + float64(df)))
}

func TFIDF(tf float32, idf float64) float64 {
	return float64(tf) * idf
}

func Preprocess(input string) []string {
	if input == "" {
		return []string{}
	}

	re := regexp.MustCompile(`[()<>""{}\[\]]`)
	input = re.ReplaceAllString(input, " ")

	words := strings.FieldsFunc(strings.ToLower(input), func(r rune) bool {
		return unicode.IsSpace(r) || strings.ContainsRune(",.!?;:\t\n\r", r)
	})

	var output []string
	for _, word := range words {
		for _, part := range strings.Split(word, "-") {
			if len(part) <= 2 || isStopWord(part) || hasDigit(part) {
				continue
			}
			stemmed := stemming(part)
			lemmatized := lemmatize(stemmed)
			if len(lemmatized) > 2 {
				output = append(output, lemmatized)
			}
		}
	}

	return output
}

// nolint
func lemmatize(word string) string {
	switch word {
	case "am", "is", "are", "was", "were":
		return "be"
	case "has", "have", "had":
		return "have"
	case "does", "did":
		return "do"
	case "going", "goes", "went":
		return "go"
	case "made", "making":
		return "make"
	case "saw", "seen", "seeing":
		return "see"
	case "came", "coming":
		return "come"
	case "took", "takes", "taken", "taking":
		return "take"
	case "better", "best":
		return "good"
	case "worse", "worst":
		return "bad"
	case "bigger", "biggest":
		return "big"
	case "smaller", "smallest":
		return "small"
	case "larger", "largest":
		return "large"
	case "more", "most":
		return "many"
	case "less", "least":
		return "little"
	case "further", "furthest":
		return "far"
	case "children":
		return "child"
	case "people":
		return "person"
	case "lives":
		return "life"
	case "wives":
		return "wife"
	case "fewer", "fewest":
		return "few"
	default:
		return word
	}
}

// nolint
func stemming(word string) string {
	switch {
	case strings.HasSuffix(word, "fulness"):
		return word[:len(word)-7]
	case strings.HasSuffix(word, "ousness"):
		return word[:len(word)-7]
	case strings.HasSuffix(word, "ization"):
		return word[:len(word)-7] + "ize"
	case strings.HasSuffix(word, "ational"):
		return word[:len(word)-7] + "ate"
	case strings.HasSuffix(word, "tional"):
		return word[:len(word)-6] + "tion"
	case strings.HasSuffix(word, "alize"):
		return word[:len(word)-5] + "al"
	case strings.HasSuffix(word, "icate"):
		return word[:len(word)-5] + "ic"
	case strings.HasSuffix(word, "ative"):
		return word[:len(word)-5]
	case strings.HasSuffix(word, "ement"):
		return word[:len(word)-5]
	case strings.HasSuffix(word, "ingly"):
		return word[:len(word)-5]
	case strings.HasSuffix(word, "fully"):
		return word[:len(word)-5]
	case strings.HasSuffix(word, "ably"):
		return word[:len(word)-4]
	case strings.HasSuffix(word, "ibly"):
		return word[:len(word)-4]
	case strings.HasSuffix(word, "ing"):
		stem := word[:len(word)-3]
		if len(stem) > 0 && !isVowel(rune(stem[len(stem)-1])) {
			return stem + "e"
		}
		return stem
	case strings.HasSuffix(word, "ies"):
		return word[:len(word)-3] + "y"
	case strings.HasSuffix(word, "ive"):
		return word[:len(word)-3]
	case strings.HasSuffix(word, "es"):
		return word[:len(word)-2]
	case strings.HasSuffix(word, "ly"):
		return word[:len(word)-2]
	case strings.HasSuffix(word, "ed"):
		stem := word[:len(word)-2]
		if len(stem) > 0 && !isVowel(rune(stem[len(stem)-1])) {
			return stem + "e"
		}
		return stem
	default:
		return word
	}
}

func isStopWord(word string) bool {
	words := []string{
		"i", "me", "my", "myself", "we", "our", "ours", "us", "this", "these", "those",
		"he", "she", "it", "its", "itself", "them", "their", "by", "from", "into", "during",
		"after", "before", "above", "below", "but", "or", "as", "if", "then", "else", "so",
		"will", "would", "shall", "should", "can", "could", "may", "might", "must",
		"here", "there", "now", "just", "very", "too", "about", "again", "once", "been",
		"being", "both", "each", "few", "more", "most", "other", "some", "such", "what",
		"which", "why", "how", "the",
	}

	stopWords := make(map[string]struct{})
	for _, w := range words {
		stopWords[w] = struct{}{}
	}

	_, exists := stopWords[word]
	return exists
}

func isVowel(r rune) bool {
	return strings.ContainsRune("aeiou", r)
}

func hasDigit(s string) bool {
	for _, r := range s {
		if unicode.IsDigit(r) {
			return true
		}
	}
	return false
}

func AddVocabulary(documents []string) {
	if vocabulary != nil {
		return
	}

	tokenizedDocs = make([][]string, len(documents))
	wCount := make(map[string]int)

	for i, doc := range documents {
		tokens := Preprocess(doc)
		tokenizedDocs[i] = tokens

		for _, token := range tokens {
			wCount[token]++
		}
	}

	type wc struct {
		word  string
		count int
	}

	wcList := make([]wc, 0, len(wCount))
	for w, c := range wCount {
		wcList = append(wcList, wc{w, c})
	}

	sort.Slice(wcList, func(i, j int) bool {
		return wcList[i].count > wcList[j].count
	})

	limit := int(avgOverview) * len(documents)
	if limit > len(wcList) {
		limit = len(wcList)
	}

	vocabulary = make([]string, limit)
	vocabularyIndexMap = make(map[string]int, limit)

	for i := 0; i < limit; i++ {
		vocabulary[i] = wcList[i].word
		vocabularyIndexMap[wcList[i].word] = i
	}
}

func CalcIDF() {
	N := len(tokenizedDocs)
	idfCache = make(map[string]float64, len(vocabulary))
	dfCount := make(map[string]int)

	for _, doc := range tokenizedDocs {
		seen := make(map[string]bool)
		for _, token := range doc {
			if !seen[token] {
				dfCount[token]++
				seen[token] = true
			}
		}
	}

	for word := range vocabularyIndexMap {
		df := dfCount[word]

		if df == 0 {
			df = 1
		}

		idfCache[word] = IDF(N, df)
	}
}

//export AnalyzeByDocs
func AnalyzeByDocs(documents []Movie, inputText string) (*mat.Dense, []Movie) {
	documentsTake := documents
	if len(documents) > 0 {
		documentsTake = documents[:len(documents)/5]
	}

	dOverview := make([]string, len(documentsTake)+1)
	for i, movie := range documentsTake {
		dOverview[i] = movie.Overview
	}
	dOverview[len(dOverview)-1] = inputText

	AddVocabulary(dOverview)
	CalcIDF()

	nDocs := len(dOverview)
	nTerms := len(vocabulary)

	data := make([]float64, nDocs*nTerms)

	var wg sync.WaitGroup
	sem := make(chan struct{}, 100)

	for i := 0; i < nDocs; i++ {
		wg.Add(1)
		sem <- struct{}{}

		go func(i int) {
			defer wg.Done()
			defer func() { <-sem }()

			tokens := tokenizedDocs[i]

			termFreq := make(map[string]int)
			for _, token := range tokens {
				termFreq[token]++
			}

			total := len(tokens)
			for token, count := range termFreq {
				idf, okIDF := idfCache[token]
				idx, okIDX := vocabularyIndexMap[token]

				if okIDF && okIDX && idx < nTerms && i < nDocs {
					tf := TF(count, total)
					tfidf := TFIDF(tf, idf)

					data[i*nTerms+idx] = tfidf
				}
			}
		}(i)
	}
	wg.Wait()

	matrix := mat.NewDense(nDocs, nTerms, data)

	var svd mat.SVD
	ok := svd.Factorize(matrix, mat.SVDThin)
	if !ok {
		return nil, nil
	}

	U := mat.NewDense(nDocs, nDocs, nil)
	svd.UTo(U)

	return U, documentsTake
}

func CosineSimilarity(a, b []float64) float64 {
	var dot, normA, normB float64
	for i := range a {
		dot += a[i] * b[i]
		normA += a[i] * a[i]
		normB += b[i] * b[i]
	}

	if normA == 0 || normB == 0 {
		return 0
	}

	return dot / (math.Sqrt(normA) * math.Sqrt(normB))
}

func main() {
	startTime := time.Now()

	movieFile, err := os.Open("./movies.json")
	if err != nil {
		fmt.Println(err.Error())
		return
	}
	defer movieFile.Close()

	var moviesJson []Movies

	decoder := json.NewDecoder(movieFile)
	if err := decoder.Decode(&moviesJson); err != nil {
		fmt.Println("we didn't find any movies.")
		return
	}

	var movieList []Movie

	for _, movies := range moviesJson {
		for _, movie := range movies.Results {
			movieList = append(movieList, movie)
		}
	}

	matrix, docs := AnalyzeByDocs(movieList, "An experienced thief specializing in the theft of secrets from the subconscious during sleep. His unique skills made him a valuable player in the dangerous world of espionage, but also condemned the life on the run and loss of the family. Cobb receives a task that seems impossible - not to steal the idea, but to introduce it into the consciousness of the goal.")
	if matrix == nil {
		return
	}

	rows, _ := matrix.Dims()
	inputVec := matrix.RawRowView(rows - 1)

	type docSim struct {
		index      int
		similarity float64
	}

	sims := make([]docSim, 0, rows-1)
	for i := 0; i < rows-1; i++ {
		rowVec := matrix.RawRowView(i)
		sim := CosineSimilarity(rowVec, inputVec)
		sims = append(sims, docSim{i, sim})
	}

	sort.Slice(sims, func(i, j int) bool {
		return sims[i].similarity > sims[j].similarity
	})

	if len(sims) < top {
		top = len(sims)
	}

	for i := 0; i < top; i++ {
		idx := sims[i].index
		sim := sims[i].similarity
		movie := docs[idx]

		fmt.Printf("[%e] %s\n", sim, movie.Title)
	}

	elapsed := time.Since(startTime)
	fmt.Printf("Время выполнения %s\n", elapsed)
}
