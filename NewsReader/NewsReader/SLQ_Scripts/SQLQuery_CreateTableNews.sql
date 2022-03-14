USE [DB_NEWS]
GO

/****** Object:  Table [dbo].[News]    Script Date: 12.03.2022 12:17:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[News](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Publication_Date] [datetime2](0) NULL,
	[Guid] [nvarchar](500) NULL,
	[SourseUrl] [nvarchar](500) NULL,
	[SourceName] [nvarchar](500) NULL,
	[News_title] [nvarchar](500) NULL,
	[News_description] [nvarchar](max) NULL,
 CONSTRAINT [PK_News] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


