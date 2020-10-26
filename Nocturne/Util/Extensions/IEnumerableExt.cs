﻿using System.Collections.Generic;
using System.Collections;

//ripped from https://github.com/prime31/Nez

namespace Nocturne.IEnumerableExtensions
{
	public static class IEnumerableExt
	{
		/// <summary>
		/// Jon Skeet's excellent reimplementation of LINQ Count.
		/// </summary>
		/// <typeparam name="TSource">The source type.</typeparam>
		/// <param name="source">The source IEnumerable.</param>
		/// <returns>The number of items in the source.</returns>
		public static int Count<TSource>( this IEnumerable<TSource> source )
		{

			// Optimization for ICollection<T> 
			var genericCollection = source as ICollection<TSource>;
			if ( genericCollection != null )
				return genericCollection.Count;

			// Optimization for ICollection 
			var nonGenericCollection = source as ICollection;
			if ( nonGenericCollection != null )
				return nonGenericCollection.Count;

			// Do it the slow way - and make sure we overflow appropriately 
			checked
			{
				int count = 0;
				using ( var iterator = source.GetEnumerator() )
				{
					while ( iterator.MoveNext() )
						count++;
				}

				return count;
			}
		}
	}
}