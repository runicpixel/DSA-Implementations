using System;
using System.Collections.Generic;
using System.Text;

namespace Vector
{
    public class Vector<T> where T : IComparable<T>
    {

        // This constant determines the default number of elements in a newly created vector.
        // It is also used to extended the capacity of the existing vector
        private const int DEFAULT_CAPACITY = 10;

        // This array represents the internal data structure wrapped by the vector class.
        // In fact, all the elements are to be stored in this private  array. 
        // You will just write extra functionality (methods) to make the work with the array more convenient for the user.
        private T[] data;

        // This property represents the number of elements in the vector
        public int Count { get; private set; } = 0;

        // This property represents the maximum number of elements (capacity) in the vector
        public int Capacity
        {
            get { return data.Length; }
        }

        // This is an overloaded constructor
        public Vector(int capacity)
        {
            data = new T[capacity];
        }

        // This is the implementation of the default constructor
        public Vector() : this(DEFAULT_CAPACITY) { }

        // An Indexer is a special type of property that allows a class or structure to be accessed the same way as array for its internal collection. 
        // For example, introducing the following indexer you may address an element of the vector as vector[i] or vector[0] or ...
        public T this[int index]
        {
            get
            {
                if (index >= Count || index < 0) throw new IndexOutOfRangeException();
                return data[index];
            }
            set
            {
                if (index >= Count || index < 0) throw new IndexOutOfRangeException();
                data[index] = value;
            }
        }

        // This private method allows extension of the existing capacity of the vector by another 'extraCapacity' elements.
        // The new capacity is equal to the existing one plus 'extraCapacity'.
        // It copies the elements of 'data' (the existing array) to 'newData' (the new array), and then makes data pointing to 'newData'.
        private void ExtendData(int extraCapacity)
        {
            T[] newData = new T[Capacity + extraCapacity];
            for (int i = 0; i < Count; i++) newData[i] = data[i];
            data = newData;
        }

        // This method adds a new element to the existing array.
        // If the internal array is out of capacity, its capacity is first extended to fit the new element.
        public void Add(T element)
        {
            if (Count == Capacity) ExtendData(DEFAULT_CAPACITY);
            data[Count++] = element;
        }

        // This method searches for the specified object and returns the zero‐based index of the first occurrence within the entire data structure.
        // This method performs a linear search; therefore, this method is an O(n) runtime complexity operation.
        // If occurrence is not found, then the method returns –1.
        // Note that Equals is the proper method to compare two objects for equality, you must not use operator '=' for this purpose.
        public int IndexOf(T element)
        {
            for (var i = 0; i < Count; i++)
            {
                if (data[i].Equals(element)) return i;
            }
            return -1;
        }

        public ISorter Sorter { set; get; } = new DefaultSorter();

        internal class DefaultSorter : ISorter
        {
            public void Sort<K>(K[] sequence, IComparer<K> comparer) where K : IComparable<K>
            {
                if (comparer == null) comparer = Comparer<K>.Default;
                Array.Sort(sequence, comparer);
            }
        }

        public void Sort()
        {
            if (Sorter == null) Sorter = new DefaultSorter();
            Array.Resize(ref data, Count);
            Sorter.Sort(data, null);
        }

        public void Sort(IComparer<T> comparer)
        {
            if (Sorter == null) Sorter = new DefaultSorter();
            Array.Resize(ref data, Count);
            if (comparer == null) Sorter.Sort(data, null);
            else Sorter.Sort(data, comparer);
        }

    }

    public class RandomizedQuickSort : ISorter
    {
        private static Random random = new Random();

        public void Sort<K>(K[] sequence, IComparer<K> comparer) where K : IComparable<K>
        {
            if (comparer == null) comparer = Comparer<K>.Default;
            RandomizedQuickSortInternal(sequence, comparer, 0, sequence.Length - 1);
        }

        private void Swap<K>(K[] sequence, int i, int j)
        {
            K temp = sequence[i];
            sequence[i] = sequence[j];
            sequence[j] = temp;
        }

        private int Partition<K>(K[] sequence, IComparer<K> comparer, int low, int high)
        {
            int pivotIndex = random.Next(low, high + 1);
            Swap(sequence, pivotIndex, high);
            K pivot = sequence[high];
            int i = low - 1;

            for (int j = low; j <= high - 1; j++)
            {
                if (comparer.Compare(sequence[j], pivot) <= 0)  
                {
                    i++;
                    Swap(sequence, i, j);
                }
            }

            Swap(sequence, i + 1, high);
            return i + 1;
        }

        private void RandomizedQuickSortInternal<K>(K[] sequence, IComparer<K> comparer, int low, int high) where K : IComparable<K>
        {
            if (low < high)
            {
                int pivotIndex = Partition(sequence, comparer, low, high);
                RandomizedQuickSortInternal(sequence, comparer, low, pivotIndex - 1);
                RandomizedQuickSortInternal(sequence, comparer, pivotIndex + 1, high);
            }
        }
    }

    public class MergeSortTopDown : ISorter
    {
        public void Sort<K>(K[] sequence, IComparer<K> comparer) where K : IComparable<K>
        {
            if (comparer == null) comparer = Comparer<K>.Default;
            MergeSortTopDownInternal(sequence, comparer, 0, sequence.Length - 1);
        }

        private void MergeSortTopDownInternal<K>(K[] sequence, IComparer<K> comparer, int low, int high) where K : IComparable<K>
        {
            if (low < high)
            {
                int mid = (low + high) / 2;
                MergeSortTopDownInternal(sequence, comparer, low, mid);
                MergeSortTopDownInternal(sequence, comparer, mid + 1, high);
                Merge(sequence, comparer, low, mid, high);
            }
        }

        private void Merge<K>(K[] sequence, IComparer<K> comparer, int low, int mid, int high) where K : IComparable<K>
        {
            int leftSize = mid - low + 1;
            int rightSize = high - mid;

            K[] left = new K[leftSize];
            K[] right = new K[rightSize];

            for (int i = 0; i < leftSize; i++)
                left[i] = sequence[low + i];
            for (int j = 0; j < rightSize; j++)
                right[j] = sequence[mid + 1 + j];

            int leftIndex = 0;
            int rightIndex = 0;
            int mergeIndex = low;

            while (leftIndex < leftSize && rightIndex < rightSize)
            {
                if (comparer.Compare(left[leftIndex], right[rightIndex]) <= 0)
                {
                    sequence[mergeIndex] = left[leftIndex];
                    leftIndex++;
                }
                else
                {
                    sequence[mergeIndex] = right[rightIndex];
                    rightIndex++;
                }
                mergeIndex++;
            }

            while (leftIndex < leftSize)
            {
                sequence[mergeIndex] = left[leftIndex];
                leftIndex++;
                mergeIndex++;
            }

            while (rightIndex < rightSize)
            {
                sequence[mergeIndex] = right[rightIndex];
                rightIndex++;
                mergeIndex++;
            }
        }
    }

    public class MergeSortBottomUp : ISorter
    {
        public void Sort<K>(K[] sequence, IComparer<K> comparer) where K : IComparable<K>
        {
            if (comparer == null) comparer = Comparer<K>.Default;
            MergeSortBottomUpInternal(sequence, comparer);
        }

        private void MergeSortBottomUpInternal<K>(K[] sequence, IComparer<K> comparer) where K : IComparable<K>
        {
            int n = sequence.Length;
            K[] aux = new K[n];

            for (int sz = 1; sz < n; sz *= 2)
            {
                for (int lo = 0; lo < n - sz; lo += 2 * sz)
                {
                    Merge(sequence, aux, comparer, lo, lo + sz - 1, Math.Min(lo + 2 * sz - 1, n - 1));
                }
            }
        }

        private void Merge<K>(K[] sequence, K[] aux, IComparer<K> comparer, int low, int mid, int high) where K : IComparable<K>
        {
            int leftIndex = low;
            int rightIndex = mid + 1;

            for (int k = low; k <= high; k++)
            {
                aux[k] = sequence[k];
            }

            for (int k = low; k <= high; k++)
            {
                if (comparer.Compare(aux[leftIndex], aux[rightIndex]) <= 0)
                {
                    sequence[k] = aux[leftIndex++];
                }
                else if (leftIndex > mid)
                {
                    sequence[k] = aux[rightIndex++];
                }
                else if (rightIndex > high)
                {
                    sequence[k] = aux[leftIndex++];
                }
                else
                {
                    sequence[k] = aux[rightIndex++];
                }
            }
        }
    }
}