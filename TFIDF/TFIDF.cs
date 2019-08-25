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

            string[] termParts = term.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            double termTf = 0;

            foreach (string item in termParts)
            {
                string termPart = item.ToLower();
                double termPartCount = 0;
                foreach (string word in wordsInFile)
                {
                    if (termPart.Equals(word))
                    {
                        termPartCount++;
                    }
                }

                termTf += termPartCount / wordsInFile.Length;
            }

            return termTf;
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

            int totalDocuments = files.Count();
            int numberOfFilesContainsTerm = 0;

            string[] termParts = term.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var file in files)
            {
                string readText = File.ReadAllText(file);
                string[] wordsInFile = TextUtil.Tokenize(readText);

                bool termInFile = true;
                foreach (string word in termParts)
                {
                    if (!wordsInFile.Contains(word.ToLower()))
                    {
                        termInFile = false;
                        break;
                    }
                }

                if (termInFile)
                {
                    numberOfFilesContainsTerm++;
                }
            }

            if (numberOfFilesContainsTerm == 0)
            {
                return 0;
            }
            else
            {
                return Math.Log((double)totalDocuments / (double)numberOfFilesContainsTerm, 2);
            }
        }

        public static double CalculateTFIDF(string dirpath, string filename, string term) => Math.Round(CalculateTF(dirpath, filename, term) * CalculateIDF(dirpath, term), 5);

        public double CacheCalculateTF(string fileName, string term)
        {
            if (fileName == "" || term == "")
            {
                throw new System.ArgumentException("Empty parameters");
            }

            Dictionary<string, double> bagOfWords = m_corpusCache.GetFileBagOfWordsTF(fileName);

            string[] termParts = term.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            double termTf = 0;

            foreach (string item in termParts)
            {
                if (bagOfWords.ContainsKey(item.ToLower()))
                {
                    termTf += bagOfWords[item];
                }
            }

            return termTf;
        }

        public double CacheCalculateIDF(string term)
        {
            if (term == "")
            {
                throw new System.ArgumentException("Empty parameters");
            }

            Dictionary<string, Dictionary<string, double>> bagOfBags = m_corpusCache.GetAllBagsOfWordsInCourpus();

            int totalFilesInCorpus = bagOfBags.Count();
            int numberOfFilesContainsTerm = 0;

            string[] termParts = term.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var bag in bagOfBags)
            {
                bool termInFile = true;
                foreach (string word in termParts)
                {
                    if (!bag.Value.ContainsKey(word.ToLower()))
                    {
                        termInFile = false;
                        break;
                    }
                }

                if (termInFile)
                {
                    numberOfFilesContainsTerm++;
                }
            }

            if (numberOfFilesContainsTerm == 0)
            {
                return 0;
            }
            else
            {
                return Math.Log((double)totalFilesInCorpus / (double)numberOfFilesContainsTerm, 2);
            }
        }

        public double CacheCalculateTFIDF(string filename, string term) => Math.Round(CacheCalculateTF(filename, term) * CacheCalculateIDF(term), 5);
    }
}
