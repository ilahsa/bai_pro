USE [DEVTEMP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--- AUTO DROP ---
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[USP_TCC_GetTransactionLog]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[USP_TCC_GetTransactionLog]
GO

-- =============================================
-- Author:		<lichl>
-- Create date: <2010-04-29>
-- Description:	<获取TCC_TransactionLog表中内容--失败的Transaction>
-- =============================================



create PROCEDURE [dbo].[USP_TCC_GetTransactionLog]
	@TxSchema	varchar(32), 
	@TxState	smallint
AS
BEGIN
	select * from TCC_TransactionLog 
	where TxSchema=@TxSchema 
	and	TxState<>@TxState
	
END