using AliaSQL.Core;

namespace AliaSQL.Core.Services.Impl
{
	namespace AliaSQL.Core.Services.Impl
	{
		public class DatabaseAction : Enumeration
		{
			public static readonly DatabaseAction Create = new DatabaseAction(1, "Create");
			public static readonly DatabaseAction Update = new DatabaseAction(2, "Update");
			public static readonly DatabaseAction Drop = new DatabaseAction(3, "Drop");
            public static readonly DatabaseAction Seed = new DatabaseAction(4, "Seed");
            public static readonly DatabaseAction Baseline = new DatabaseAction(5, "Baseline");

			public DatabaseAction()
			{
			}

			public DatabaseAction(int value, string displayName) : base(value, displayName)
			{
			}
		}
	}	
}