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
            string[] tokenized = text.ToLower().Split(" \r\n@/.-:&*+=[]?!(){},''\">_<;%\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return tokenized;
        }
    }

    public class TFIDF
    {
        private readonly DocumentsTfCache m_tfCache;
        private string m_dirPath;

        public string DirPath
        {
            get { return m_dirPath; }
            set
            {
                if (value == "")
                {
                    throw new System.ArgumentException("Empty parameters");
                }
                m_dirPath = value;
            }
        }

        public TFIDF(string path)
        {
            DirPath = path;
            m_tfCache = new DocumentsTfCache(path, new RandomReplacementAlgoCacheImpl<string, Dictionary<string, double>>());
        }

        public static double CalculateTF(string dirPath, string fileName, string term)
        {
            if (dirPath == "" || fileName == "" || term == "")
            {
                throw new System.ArgumentException("Empty parameters");
            }

            string readText = File.ReadAllText(dirPath + fileName);
            string[] wordsInDocument = TextUtil.Tokenize(readText);
            int termFrequancy = 0;
            term = term.ToLower();
              
            foreach (string word in wordsInDocument)
            {
                if (term.Equals(word))
                {
                    termFrequancy++;
                }
            }

            return (double)termFrequancy / (double)wordsInDocument.Length;
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

            var documents = Directory.EnumerateFiles(dirPath);
            int totalDocumentsInDirectory = documents.Count();

            if (totalDocumentsInDirectory == 0)
            {
                return 0;
            }

            int numberOfDocumentsContainsTerm = 0;
            term = term.ToLower();

            foreach (var document in documents)
            {
                string readText = File.ReadAllText(document);
                string[] wordsInDocument = TextUtil.Tokenize(readText);
                
                if (wordsInDocument.Contains(term))
                {
                    numberOfDocumentsContainsTerm++;
                }
            }

            double denominator = numberOfDocumentsContainsTerm;

            if (numberOfDocumentsContainsTerm == 0)
            {
                denominator = 1;
            }

            return Math.Log((double)totalDocumentsInDirectory / denominator, 2);
        }

        public static double CalculateTFIDF(string dirpath, string filename, string term) => CalculateTF(dirpath, filename, term) * CalculateIDF(dirpath, term);

        public double CacheCalculateTF(string fileName, string term)
        {
            if (fileName == "" || term == "")
            {
                throw new System.ArgumentException("Empty parameters");
            }

            Dictionary<string, double> bagOfWords = m_tfCache.GetDocumentBagOfWordsTF(fileName);
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

            Dictionary<string, Dictionary<string, double>> bagOfBags = m_tfCache.GetAllBagsOfWordsInCourpus();
            int totalDocumentsInDirectory = bagOfBags.Count();

            if (totalDocumentsInDirectory == 0)
            {
                return 0;
            }

            int numberOfDocumentsContainsTerm = 0;
            term = term.ToLower();

            foreach (var bag in bagOfBags)
            {               
                if (bag.Value.ContainsKey(term))
                {
                    numberOfDocumentsContainsTerm++;
                }
            }

            double denominator = numberOfDocumentsContainsTerm;

            if (numberOfDocumentsContainsTerm == 0)
            {
                denominator = 1;
            }

            return Math.Log((double)totalDocumentsInDirectory / denominator, 2);
        }

        public double CacheCalculateTFIDF(string filename, string term) => CacheCalculateTF(filename, term) * CacheCalculateIDF(term);
    }
}
