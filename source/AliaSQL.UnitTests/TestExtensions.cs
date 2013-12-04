using NUnit.Framework;

namespace AliaSQL.UnitTests
{
	public static class TestExtensions
	{
		public static void ShouldEqual(this object current, object expected)
		{
			Assert.AreEqual(expected, current);
		}

		public static void ShouldBeTrue(this bool current)
		{
			Assert.IsTrue(current);
		}

		public static void ShouldBeFalse(this bool current)
		{
			Assert.IsFalse(current);
		}
	}
}