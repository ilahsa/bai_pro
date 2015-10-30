USE [DEVTEMP]
GO

/****** Object:  Table [dbo].[TCC_TransactionLog]    Script Date: 04/27/2010 14:14:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TCC_TransactionLog]') AND type in (N'U'))
DROP TABLE [dbo].[TCC_TransactionLog]
GO
CREATE TABLE [dbo].[TCC_TransactionLog](
	[TxId] [uniqueidentifier] NOT NULL,	
	[ServiceAtComputer] [varchar](32) NOT NULL,
	[TxSchema] [varchar](32) NOT NULL,			-- 相当于Coordinator.TransName
	[BeginTime] [datetime] NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,	
	[TxState] [smallint] NOT NULL,
	[WorkState] [varchar](256) NOT NULL,		-- "2,2,2,1,1,0,0"
	[ContextKey] [varchar](64) NOT NULL,		-- Index: 
	[ContextData] [varbinary](max) NOT NULL,	-- 
	[Error] [varchar](2048),			-- 当前错误
 CONSTRAINT [PK_TCC_TransactionLog] PRIMARY KEY CLUSTERED 
(
	[TxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO
/****** 对象:  Index [IX_TCC_TransactionLog_ContextKey]    脚本日期: 04/29/2010 10:34:40 ******/
CREATE NONCLUSTERED INDEX [IX_TCC_TransactionLog_ContextKey] ON [dbo].[TCC_TransactionLog] 
(
	[ContextKey] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TCC_TransactionLog_TxSchema] ON [dbo].[TCC_TransactionLog] 
(
	[TxSchema] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


