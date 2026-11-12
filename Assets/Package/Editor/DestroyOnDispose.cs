using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OneLastClick.ProjectSettings.Editor
{
    /// <summary>
    /// Owns a set of Unity objects and destroys them on dispose.
    /// Constrained to <see cref="UnityEngine.Object"/> so ownership is unambiguous: every element is destroyed.
    /// </summary>
    public sealed class DestroyOnDispose<T> : IDisposable, IReadOnlyList<T> where T : Object
    {
        /// <summary>Raw array view. Do not keep this past the owner's lifetime.</summary>
        public T[] Items => _items ?? Array.Empty<T>();
        
        public bool IsDisposed => _items == null;
        public int Count => _items?.Length ?? 0;
        public int Length => Count;

        public T this[int index] => Items[index];

        private T[] _items;

        public DestroyOnDispose(T[] items)
        {
            _items = items ?? Array.Empty<T>();
        }

        public static implicit operator T[](DestroyOnDispose<T> source) => source?.Items ?? Array.Empty<T>();

        public static implicit operator DestroyOnDispose<T>(T[] source) => new DestroyOnDispose<T>(source);

        public void Dispose()
        {
            T[] local = _items;
            _items = null;
            
            if (local == null)
            {
                return;
            }

            for (int i = 0; i < local.Length; i++)
            {
                Object unityObject = local[i];
                
                if (unityObject == null)
                {
                    continue;
                }

                if (Application.isPlaying == false)
                {
                    Object.DestroyImmediate(unityObject);
                }
                else
                {
                    Object.Destroy(unityObject);
                }
            }

            Array.Clear(local, 0, local.Length); 
        }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Items).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}