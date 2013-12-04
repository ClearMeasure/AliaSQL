using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace AliaSQL.UnitTests
{
	public class EnumerableAssert
	{
		public static void Contains<T>(IEnumerable<T> enumerable, T actual)
		{
			CollectionAssert.Contains(new List<T>(enumerable), actual);
		}

		public static void DoesNotContain<T>(IEnumerable<T> enumerable, T actual)
		{
			CollectionAssert.DoesNotContain(new List<T>(enumerable), actual);
		}

		public static void AreEquivalent<T>(IEnumerable<T> enumerable, IEnumerable<T> actual)
		{
			CollectionAssert.AreEquivalent(new List<T>(enumerable), new List<T>(actual));
		}

		public static void IsNotEmpty<T>(IEnumerable<T> enumerable)
		{
			CollectionAssert.IsNotEmpty(new List<T>(enumerable));
		}

		public static void That<T>(IEnumerable<T> enumerable, Constraint constraint)
		{
			Assert.That(new List<T>(enumerable), constraint);
		}
	}
}