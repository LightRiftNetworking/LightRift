/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace DarkRift.DataStructures
{
    /// <summary>
    ///     A dictionary with limited spaces, once all spaces are filled the dictionary will remove the oldest elements to add new element.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <remarks>
    ///     A number of standard dictionary methods are not implemented as they are not needed in DarkRift.
    /// </remarks>
    internal class CircularDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     The backing array behind the dictionary.
        /// </summary>
        private readonly KeyValuePair<TKey, TValue>[] backingArray;

        /// <summary>
        ///     The element we will next insert into.
        /// </summary>
        private int ptr;

        public TValue this[TKey key]
        {
            get
            {
                lock (backingArray)
                {
                    foreach (var entry in backingArray)
                    {
                        if (!entry.Key.Equals(key))
                        {
                            continue;
                        }

                        return entry.Value;
                    }
                }

                throw new KeyNotFoundException();
            }

            set
            {
                lock (backingArray)
                {
                    for (var i = 0; i < backingArray.Length; i++)
                    {
                        if (!backingArray[i].Key.Equals(key))
                        {
                            continue;
                        }

                        backingArray[i] = new KeyValuePair<TKey, TValue>(key, value);
                    }
                }

                throw new KeyNotFoundException();
            }
        }

        public ICollection<TKey> Keys => throw new NotImplementedException();

        public ICollection<TValue> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => false;

        public CircularDictionary(int size)
        {
            backingArray = new KeyValuePair<TKey, TValue>[size];
        }

        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (backingArray)
            {
                backingArray[ptr] = item;
                ptr = (ptr + 1) % backingArray.Length;
            }
        }

        public void Clear()
        {
            lock (backingArray)
            {
                for (var i = backingArray.Length - 1; i >= 0; i--)
                {
                    backingArray[i] = default;
                }
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (backingArray)
            {
                foreach (var entry in backingArray)
                {
                    if (entry.Equals(item))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ContainsKey(TKey key)
        {
            lock (backingArray)
            {
                foreach (var entry in backingArray)
                {
                    if (entry.Key.Equals(key))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (backingArray)
            {
                foreach (var entry in backingArray)
                {
                    if (!entry.Key.Equals(key))
                    {
                        continue;
                    }

                    value = entry.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
