using System;
using System.Collections;
using System.Collections.Generic;

namespace Cache
{
    public class RandomReplacementAlgorithmCache<K, V> : IAlgorithmCache<K, V>
    {
        private static readonly int m_defaultCapacity = 50;
        private readonly Dictionary <K, V> m_cache;
        private readonly ArrayList m_keyIndex;
        private int m_freeSpace;
        private readonly Random m_random;

        public RandomReplacementAlgorithmCache() : this(m_defaultCapacity) { }

        public RandomReplacementAlgorithmCache(int capacity) 
        {
            m_freeSpace = capacity;
            m_cache = new Dictionary<K, V>(capacity);
            m_keyIndex = new ArrayList();
            m_random = new Random();
        }

        public V GetElement(K key)
        {
            if (m_cache.ContainsKey(key))
            {
                return m_cache[key];
            }
            else
            {
                return default;
            }
        }

        public V PutElement(K key, V value)
        {
            if (m_keyIndex.Contains(key))
            {
                return m_cache[key];
            }

            if (m_freeSpace > 0)
            {
                m_freeSpace--;
                m_keyIndex.Add(key);

                return m_cache[key] = value;
            }

            int rndIndex = m_random.Next(m_keyIndex.Count);
            K keyToRemove = (K)m_keyIndex[rndIndex];
            V removedValue = m_cache[keyToRemove];
            m_cache.Remove(keyToRemove);
            m_cache[key] = value;
            m_keyIndex[rndIndex] = key;

            return removedValue;
        }

        public void RemoveElement(K key)
        {
            if (m_cache.ContainsKey(key))
            {
                m_cache.Remove(key);
                m_keyIndex.Remove(key);
                m_freeSpace++;
            }
        }
    }
}


