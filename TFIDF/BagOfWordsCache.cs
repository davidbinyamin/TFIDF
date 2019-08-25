using System.Collections.Generic;
using System.IO;
using Cache;

namespace InformationRetrieval
{
    class CorpusCache
    {
        private IAlgoCache<string, Dictionary<string, double>> m_corpusCache;
        readonly string m_corpusPath;

        public CorpusCache(string corpusPath, IAlgoCache<string, Dictionary<string, double>> algorithmCache)
        {
            this.AlgorithmCache = algorithmCache;

            if (Directory.Exists(corpusPath))
            {
                this.m_corpusPath = corpusPath;
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        public IAlgoCache<string, Dictionary<string, double>> AlgorithmCache { get => m_corpusCache; set => m_corpusCache = value; }

        public Dictionary<string, double> GetFileBagOfWordsTF(string fileName)
        {
            Dictionary<string, double> fileBagOfWords;
            try
            {
                fileBagOfWords = AlgorithmCache.GetElement(fileName);
            }
            catch (KeyNotFoundException)
            {
                string text = File.ReadAllText(m_corpusPath + fileName);
                string[] wordsInFile = TextUtil.Tokenize(text);
                fileBagOfWords = new Dictionary<string, double>();
                double dfFragment = 1.0 / wordsInFile.Length;

                foreach (string word in wordsInFile)
                {
                    if (fileBagOfWords.ContainsKey(word))
                    {
                        fileBagOfWords[word] += dfFragment;
                    }
                    else
                    {
                        fileBagOfWords[word] = dfFragment;
                    }
                }

                AlgorithmCache.PutElement(fileName,fileBagOfWords);
            }

            return fileBagOfWords;
        }

        public Dictionary<string, Dictionary<string, double>> GetAllBagsOfWordsInCourpus()
        {
          
            var files = Directory.EnumerateFiles(m_corpusPath);
            Dictionary<string, Dictionary<string, double>> bagOfBags = new Dictionary<string, Dictionary<string, double>>();
            
            foreach (var file in files)
            {
                string key = file.Substring(m_corpusPath.Length).ToLower();
                bagOfBags[key] = GetFileBagOfWordsTF(key);
            }

            return bagOfBags;
        }
    }
}
