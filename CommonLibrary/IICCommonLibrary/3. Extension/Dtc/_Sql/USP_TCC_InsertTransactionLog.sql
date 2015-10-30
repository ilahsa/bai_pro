USE [DEVTEMP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--- AUTO DROP ---
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[USP_TCC_InsertTransactionLog]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[USP_TCC_InsertTransactionLog]
GO

-- =============================================
-- Author:		<lichl>
-- Create date: <2010-04-29>
-- Description:	<插入TCC_InsertTransactionLog内容>
-- =============================================



create PROCEDURE [dbo].[USP_TCC_InsertTransactionLog]
	@TxId				uniqueidentifier,	
	@ServiceAtComputer	varchar(32),
	@TxSchema			varchar(32),
	@BeginTime			datetime,
	@LastUpdateTime		datetime,	
	@TxState			smallint,
	@WorkState			varchar(256),
	@ContextKey			varchar(64),
	@ContextData		varbinary(max),
	@Error				varchar(2048)
AS
BEGIN
	insert into TCC_TransactionLog(TxId,ServiceAtComputer,TxSchema,BeginTime,LastUpdateTime,TxState,WorkState,ContextKey,ContextData,Error)
	values(@TxId,@ServiceAtComputer,@TxSchema,@BeginTime,@LastUpdateTime,@TxState,@WorkState,@ContextKey,@ContextData,@Error)
END
