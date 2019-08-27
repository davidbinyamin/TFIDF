using Cache;
using System.Collections.Generic;
using System.IO;

namespace InformationRetrieval
{
    class DocumentsTfCache
    {
        private IAlgoCache<string, Dictionary<string, double>> m_algorithmCache;
        readonly string m_dirPath;

        public DocumentsTfCache(string dirPath, IAlgoCache<string, Dictionary<string, double>> algorithmCache)
        {
            this.AlgorithmCache = algorithmCache;

            if (Directory.Exists(dirPath))
            {
                this.m_dirPath = dirPath;
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        public IAlgoCache<string, Dictionary<string, double>> AlgorithmCache { get => m_algorithmCache; set => m_algorithmCache = value; }

        public Dictionary<string, double> GetDocumentBagOfWordsTF(string fileName)
        {
            Dictionary<string, double> bagOfWords;
            try
            {
                bagOfWords = AlgorithmCache.GetElement(fileName);
            }
            catch (KeyNotFoundException)
            {
                string text = File.ReadAllText(m_dirPath + fileName);
                string[] wordsInDocument = TextUtil.Tokenize(text);
                bagOfWords = new Dictionary<string, double>();
                double dfFragment = 1.0 / wordsInDocument.Length;

                foreach (string word in wordsInDocument)
                {
                    if (bagOfWords.ContainsKey(word))
                    {
                        bagOfWords[word] += dfFragment;
                    }
                    else
                    {
                        bagOfWords[word] = dfFragment;
                    }
                }

                AlgorithmCache.PutElement(fileName, bagOfWords);
            }

            return bagOfWords;
        }

        public Dictionary<string, Dictionary<string, double>> GetAllBagsOfWordsInCourpus()
        {

            var documents = Directory.EnumerateFiles(m_dirPath);
            Dictionary<string, Dictionary<string, double>> bagOfBags = new Dictionary<string, Dictionary<string, double>>();

            foreach (var document in documents)
            {
                string key = document.Substring(m_dirPath.Length).ToLower();
                bagOfBags[key] = GetDocumentBagOfWordsTF(key);
            }

            return bagOfBags;
        }
    }
}
