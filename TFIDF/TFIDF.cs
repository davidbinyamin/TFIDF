using Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace InformationRetrieval
{
    public static class TextUtil
    {
        public static string[] Tokenize(string text)
        {
            // Tokenize and get rid of any punctuation
            string[] tokenized = text.Split(" \r\n@/.-:&*+=[]?!(){},''\">_<;%\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            tokenized = tokenized.Select(s => s.ToLowerInvariant()).ToArray();

            return tokenized;
        }
    }
    public class TFIDF
    {
        private readonly CorpusCache m_corpusCache;
        private string m_corpusPath;

        public string CorpusPath
        {
            get { return m_corpusPath; }
            set
            {
                if (value == "")
                {
                    throw new System.ArgumentException("Empty parameters");
                }
                m_corpusPath = value;
            }
        }

        public TFIDF(string path)
        {
            CorpusPath = path;
            m_corpusCache = new CorpusCache(path, new RandomReplacementAlgoCacheImpl<string, Dictionary<string, double>>());
        }

        public static double CalculateTF(string dirPath, string fileName, string term)
        {
            if (dirPath == "" || fileName == "" || term == "")
            {
                throw new System.ArgumentException("Empty parameters");
            }

            string readText = File.ReadAllText(dirPath + fileName);
            string[] wordsInFile = TextUtil.Tokenize(readText);
            int termFrequancy = 0;
            term = term.ToLower();
              
            foreach (string word in wordsInFile)
            {
                if (term.Equals(word))
                {
                    termFrequancy++;
                }
            }

            return (double)termFrequancy / (double)wordsInFile.Length;
        }

        public static double CalculateIDF(string dirPath, string term)
        {
            if (dirPath == "")
            {
                throw new System.ArgumentException("Empty parameters");
            }

            if (!Directory.Exists(dirPath))
            {
                throw new System.ArgumentException("Invalid dir path");
            }

            var files = Directory.EnumerateFiles(dirPath);
            int totalFilesInCorpus = files.Count();

            if (totalFilesInCorpus == 0)
            {
                return 0;
            }

            int numberOfFilesContainsTerm = 0;
            term = term.ToLower();

            foreach (var file in files)
            {
                string readText = File.ReadAllText(file);
                string[] wordsInFile = TextUtil.Tokenize(readText);
                
                if (wordsInFile.Contains(term))
                {
                    numberOfFilesContainsTerm++;
                }
            }

            double denominator = numberOfFilesContainsTerm;

            if (numberOfFilesContainsTerm == 0)
            {
                denominator = 1;
            }

            return Math.Log((double)totalFilesInCorpus / denominator, 2);
        }

        public static double CalculateTFIDF(string dirpath, string filename, string term) => CalculateTF(dirpath, filename, term) * CalculateIDF(dirpath, term);

        public double CacheCalculateTF(string fileName, string term)
        {
            if (fileName == "" || term == "")
            {
                throw new System.ArgumentException("Empty parameters");
            }

            Dictionary<string, double> bagOfWords = m_corpusCache.GetFileBagOfWordsTF(fileName);
            double termTF = 0;
            term = term.ToLower();

            if (bagOfWords.ContainsKey(term))
            {
                termTF = bagOfWords[term];
            }

            return termTF;
        }

        public double CacheCalculateIDF(string term)
        {
            if (term == "")
            {
                throw new System.ArgumentException("Empty parameters");
            }

            Dictionary<string, Dictionary<string, double>> bagOfBags = m_corpusCache.GetAllBagsOfWordsInCourpus();
            int totalFilesInCorpus = bagOfBags.Count();

            if (totalFilesInCorpus == 0)
            {
                return 0;
            }

            int numberOfFilesContainsTerm = 0;
            term = term.ToLower();

            foreach (var bag in bagOfBags)
            {               
                if (bag.Value.ContainsKey(term))
                {
                    numberOfFilesContainsTerm++;
                }
            }

            double denominator = numberOfFilesContainsTerm;

            if (numberOfFilesContainsTerm == 0)
            {
                denominator = 1;
            }

            return Math.Log((double)totalFilesInCorpus / denominator, 2);
        }

        public double CacheCalculateTFIDF(string filename, string term) => CacheCalculateTF(filename, term) * CacheCalculateIDF(term);
    }
}
