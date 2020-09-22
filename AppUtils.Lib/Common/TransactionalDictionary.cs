using System.Collections;
using System.Collections.Generic;

/// <summary>

/// ''' Dizionario transazionale

/// ''' </summary>

/// ''' <typeparam name="TKey"></typeparam>

/// ''' <typeparam name="TValue"></typeparam>
public class TransactionalDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private bool mIsInTransaction;
    private Dictionary<TKey, TValue> mCommittedDictionary = new Dictionary<TKey, TValue>();
    private Dictionary<TKey, TValue> mTransDictionary = new Dictionary<TKey, TValue>();

    public int Count
    {
        get
        {
            return this.ActiveDictionary.Count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            return this.ActiveDictionary[key];
        }
        set
        {
            this.ActiveDictionary[key] = value;
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            return this.ActiveDictionary.Keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            return this.ActiveDictionary.Values;
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        this.ActiveDictionary.Add(item.Key, item.Value);
    }

    public void Add(TKey key, TValue value)
    {
        this.ActiveDictionary.Add(key, value);
    }

    public void Clear()
    {
        this.ActiveDictionary.Clear();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (KeyValuePair<TKey, TValue> item in this)
        {
            array[arrayIndex] = item;
            arrayIndex += 1;
        }
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return this.ActiveDictionary.ContainsKey(item.Key);
    }

    public bool ContainsKey(TKey key)
    {
        return this.ActiveDictionary.ContainsKey(key);
    }

 

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return this.ActiveDictionary.Remove(item.Key);
    }

    public bool Remove(TKey key)
    {
        return this.ActiveDictionary.Remove(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return this.ActiveDictionary.TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return this.ActiveDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }


    /// <summary>
    ///     ''' Dizionario interno correntemente attivo (transazionale o consolidato)
    ///     ''' </summary>
    ///     ''' <returns></returns>
    protected Dictionary<TKey, TValue> ActiveDictionary
    {
        get
        {
            if (this.mIsInTransaction)
                return this.mTransDictionary;
            else
                return this.mCommittedDictionary;
        }
    }

    /// <summary>
    ///     ''' Avvia transazione
    ///     ''' </summary>
    public void BeginTransaction()
    {
        this.mIsInTransaction = true;

        this.mTransDictionary = new Dictionary<TKey, TValue>(this.mCommittedDictionary);
    }

    /// <summary>
    ///     ''' Conferma le variazioni effettuate in transazione
    ///     ''' </summary>
    public void CommitTransaction()
    {
        this.mIsInTransaction = false;
        this.mCommittedDictionary = this.mTransDictionary;
        this.mTransDictionary = null;
    }

    /// <summary>
    ///     ''' Elimina variazioni effettuate in transazione
    ///     ''' </summary>
    public void RollbackTransaction()
    {
        this.mIsInTransaction = false;
        this.mTransDictionary.Clear();
        this.mTransDictionary = null;
    }

    /// <summary>
    ///     ''' Indica se siamo in transazione
    ///     ''' </summary>
    ///     ''' <returns></returns>
    public bool IsInTransaction
    {
        get
        {
            return this.mIsInTransaction;
        }
    }
}
