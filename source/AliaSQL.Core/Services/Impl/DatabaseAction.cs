﻿using AliaSQL.Core;

namespace AliaSQL.Core.Services.Impl
{
	namespace AliaSQL.Core.Services.Impl
	{
		public class DatabaseAction : Enumeration
		{
			public static readonly DatabaseAction Create = new DatabaseAction(1, "Create");
			public static readonly DatabaseAction Update = new DatabaseAction(2, "Update");
			public static readonly DatabaseAction Drop = new DatabaseAction(3, "Drop");
            public static readonly DatabaseAction TestData = new DatabaseAction(4, "TestData");
            public static readonly DatabaseAction Baseline = new DatabaseAction(5, "Baseline");
            public static readonly DatabaseAction UpdateCustom = new DatabaseAction(6, "UpdateCustom");

            public DatabaseAction()
			{
			}

			public DatabaseAction(int value, string displayName) : base(value, displayName)
			{
			}
		}
	}	
}