using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyBeeDOS.Drivers
{
    struct KeyValuePairWithSameType<T>
    {
        public T Key { get; set; }
        public T Value { get; set; }
    }

    public class DictionaryWithSameType<T>
    {
        List<KeyValuePairWithSameType<T>> pairs = new List<KeyValuePairWithSameType<T>>();

        public DictionaryWithSameType() { }

        public void Add(T key, T value)
        {
            KeyValuePairWithSameType<T> pair = new KeyValuePairWithSameType<T>();
            pair.Key = key;
            pair.Value = value;
            foreach(var p in pairs)
            {
                if (p.Key.Equals(pair.Key))
                {
                    pairs.Remove(p);
                    break;
                }
            }
            pairs.Add(pair);
        }

        public bool Remove(T key)
        {
            foreach(var p in pairs)
            {
                if (p.Key.Equals(key))
                {
                    pairs.Remove(p);
                    return true;
                }
            }
            return false;
        }

        public void Clear() { pairs.Clear(); }

        public bool Contains(T key)
        {
            foreach(var p in pairs)
            {
                if(p.Key.Equals(key)) return true;
            }
            return false;
        }

        public T Get(T key)
        {
            T value = default(T);
            foreach(var p in pairs)
            {
                if (p.Key.Equals(key))
                {
                    value = p.Value;
                    break;
                }
            }
            return value;
        }
    }
}
