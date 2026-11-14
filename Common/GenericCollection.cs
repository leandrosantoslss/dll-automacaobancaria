using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AutomacaoBancaria.Generics
{
    /// <summary>
    /// Classe GenericCollection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public abstract class GenericCollection<T> : IEnumerable<T> where T : class
	{
		#region Fields

        /// <summary>
        /// The list
        /// </summary>
		protected List<T> List;

		#endregion Fields

		#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCollection{T}" /> class.
        /// </summary>
		protected internal GenericCollection()
		{
			List = new List<T>();
		}

		#endregion Constructor

		#region Properties

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
		public int Count
		{
			get
			{
				return List.Count;
			}
		}

        /// <summary>
        /// Gets or sets the <see cref="T" /> with the specified index.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// </exception>
        [IndexerName("GetItem")]
		public T this[int idx]
		{
			get
			{
				if (idx >= Count || idx < 0)
					throw new IndexOutOfRangeException();

				return List[idx];
			}
			set
			{
				if (idx >= Count || idx < 0)
					throw new IndexOutOfRangeException();

				List[idx] = value;
			}
		}

		#endregion Properties

		#region Methods

        /// <summary>
        /// Clears this instance.
        /// </summary>
		public virtual void Clear()
		{
			List.Clear();
		}

		#endregion Methods

		#region IEnumerable<T>

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return List.GetEnumerator();
		}

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion IEnumerable<T>
	}
}
