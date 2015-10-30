USE [DEVTEMP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--- AUTO DROP ---
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[USP_TCC_UpdateTransaction]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[USP_TCC_UpdateTransaction]
GO

-- =============================================
-- Author:		<lichl>
-- Create date: <2010-04-29>
-- Description:	<更新TCC_ActiveTransaction内容>
-- =============================================



create PROCEDURE [dbo].[USP_TCC_UpdateTransaction]
	@TxId				uniqueidentifier,	
	@LastUpdateTime		datetime,	
	@TxState			smallint,
	@WorkState			varchar(256),
	@ContextData		varbinary(max)

AS
BEGIN
	update TCC_ActiveTransaction
	set LastUpdateTime = @LastUpdateTime,
	TxState = @TxState,
	WorkState = @WorkState,
	ContextData = @ContextData
	where TxId=@TxId
END