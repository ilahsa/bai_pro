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
	[Computer] [varchar](256) NOT NULL,
	[Service] [varchar](256) NOT NULL,
	[Repeat] [varchar](256) NOT NULL
)
 INDEX [IX_CMN_ServerTrace_Time] ON [dbo].[CMN_ServerTrace] 
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

CREATE TABLE `CMN_ServerTrace` (
	`Time`			datetime not null,
	`LoggerName`	varchar(256) not null,
	`Level`			int not null,
	`Message`		varchar(65536) not null,
	`Error`			varchar(65536) not null,
	`Thread`		varchar(256) not null,
	`Process`		varchar(256) not null,
	`From`			varchar(256) not null,
	`To`			varchar(256) not null,
	`Computer`		varchar(256) not null,
	`Service`		varchar(256) not null,
	`Repeat`		varchar(256) not null 
	
	KEY `Time` (`Time`)
	KEY `LoggerName` (`LoggerName`,`Time`),
) ENGINE=InnoDB DEFAULT CHARSET=utf8
