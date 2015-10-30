USE [TRACEDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CMN_ServerTrace](
	[Time] [datetime] NOT NULL,
	[Level] [int] NOT NULL,
	[LoggerName] [varchar](256) NOT NULL,
	[Message] [varchar](max) NOT NULL,
	[Error] [varchar](max) NOT NULL,
	[Thread] [varchar](256) NOT NULL,
	[Process] [varchar](256) NOT NULL,
	[From] [varchar](256) NOT NULL,
	[To] [varchar](256) NOT NULL,
	[Session] varchar(256) default '',
	[Computer] [varchar](256) NOT NULL,
	[Service] [varchar](256) NOT NULL,
	[Repeat] [varchar](256) NOT NULL
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_CMN_ServerTrace_Time] ON [dbo].[CMN_ServerTrace] 
(
	[Time] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_CMN_ServerTrace_LoggerName_Time] ON [dbo].[CMN_ServerTrace] 
(
	[LoggerName] ASC,
	[Time] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_CMN_ServerTrace_From_Time] ON [dbo].[CMN_ServerTrace] 
(
	[From] ASC,
	[Time] ASC
)
GO
