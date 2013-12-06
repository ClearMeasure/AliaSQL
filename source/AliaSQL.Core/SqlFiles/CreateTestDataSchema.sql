if not exists (select 1 from dbo.sysobjects where name = 'usd_AppliedDatabaseTestDataScript' and type = 'U') begin

	create table [dbo].[usd_AppliedDatabaseTestDataScript](
		[ScriptFile] [nvarchar](255) NOT NULL,
		[DateApplied] [datetime] NOT NULL
	constraint [PK_usd_AppliedDatabaseTestDataScript] primary key clustered 
	(
		[ScriptFile] asc
	)
	)

	create index [IX_usd_DateApplied] ON [dbo].[usd_AppliedDatabaseTestDataScript] 
	(
		[DateApplied] asc
	)

end