USE [DEVTEMP]
GO

/****** Object:  Table [dbo].[TCC_ActiveTransaction]    Script Date: 04/27/2010 14:07:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TCC_ActiveTransaction]') AND type in (N'U'))
DROP TABLE [dbo].[TCC_ActiveTransaction]
GO
CREATE TABLE [dbo].[TCC_ActiveTransaction](
	[TxId] [uniqueidentifier] NOT NULL,	
	[ServiceAtComputer] [varchar](32) NOT NULL,
	[TxSchema] [varchar](32) NOT NULL,			-- 相当于Coordinator.TransName
	[BeginTime] [datetime] NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,	
	[TxState] [smallint] NOT NULL,
	[WorkState] [varchar](256) NOT NULL,		-- "2,2,2,1,1,0,0"
	[ContextKey] [varchar](64) NOT NULL,		-- Index: 
	[ContextData] [varbinary](max) NOT NULL,	-- 
	--[Error] [varchar](2048),			-- 当前错误
 CONSTRAINT [PK_TCC_ActiveTransaction] PRIMARY KEY CLUSTERED 
(
	[TxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO
