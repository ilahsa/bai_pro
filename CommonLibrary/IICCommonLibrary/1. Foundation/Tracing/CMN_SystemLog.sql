USE [TRACEDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

drop table CMN_SystemLog
CREATE TABLE [dbo].[CMN_SystemLog](
	[Time] datetime NOT NULL,
	[Level] int NOT NULL,
	[EventId] varchar(256) not null,
	[Message] varchar(max) NOT NULL,
	[Service] varchar(256) not null,
	[Computer] varchar(256) NOT NULL,
	Repeat int not null,
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_CMN_SystemLog_Time] ON [dbo].[CMN_SystemLog] 
(
	[Time] ASC
)
GO