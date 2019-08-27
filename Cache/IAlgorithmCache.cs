namespace Cache
{
    public interface IAlgorithmCache<K, V>
    {
        V GetElement(K key);
        /*Returns the value to which the specified key is mapped 
        If the specified key does not exist then returns default(V)
        In addition performs the relevant cache algorithm*/

        V PutElement(K key, V value);
        /*Associates the specified value with the specified key in implemented cache 
        according to the current algorithm
        return the value of the element which had to be removed from the cache*/

        void RemoveElement(K key);
        /*Removes the element associated with this key from the cache if present.*/
    }
}

