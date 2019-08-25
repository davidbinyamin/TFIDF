namespace Cache
{
    public interface IAlgoCache<K, V>
    {
        V GetElement(K key);
        /*Returns the value to which the specified key is mapped 
        If the specified key does not exist then a KeyNotFoundException will be thrown. 
        In addition performs the relevant cache algorithm*/

        V PutElement(K key, V value);
        /*Associates the specified value with the specified key in implemented cache 
        according to the current algorithm
        return the value of the element which need to be replaced*/

        void RemoveElement(K key);
        /*Removes the mapping for the specified key from this map if present.*/
    }
}

