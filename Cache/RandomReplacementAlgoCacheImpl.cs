using System;
using System.Collections;
using System.Collections.Generic;

namespace Cache
{
    public class RandomReplacementAlgoCacheImpl<K, V> : IAlgoCache<K, V>
    {
        private readonly Dictionary <K, V> m_cache;
        private readonly ArrayList m_keyIndex;
        private readonly int m_capacity;
        private int m_freeSpace;

        public RandomReplacementAlgoCacheImpl(int capacity=50) 
        {
            this.m_capacity = capacity;

            m_freeSpace = this.m_capacity;
            m_cache = new Dictionary<K, V>(this.m_capacity);
            m_keyIndex = new ArrayList();
        }

        public V GetElement(K key)
        {
            return m_cache[key];
        }

        public V PutElement(K key, V value)
        {
            if (m_keyIndex.Contains(key))
                return m_cache[key];

            if (m_freeSpace > 0)
            {
                m_freeSpace--;
                m_keyIndex.Add(key);
                return m_cache[key] = value;
            }
            Random rand = new Random();
            int rndIndex = 0 + rand.Next(m_keyIndex.Count);
            V v = m_cache[(K)m_keyIndex[rndIndex]];
            m_cache.Remove((K)m_keyIndex[rndIndex]);
            m_cache[key] = value;
            m_keyIndex[rndIndex] = key;
            return v;
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


