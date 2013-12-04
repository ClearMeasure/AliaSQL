PRINT N'Creating [dbo].[TestTable]...';


GO
CREATE TABLE [dbo].[TestTable](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [FullName] [nvarchar](50) NULL,
        [value1] [int] NOT NULL,
        [value2] [int] NOT NULL,
 CONSTRAINT [PK_TestTable] PRIMARY KEY CLUSTERED 
(
        [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO