USE [DEVTEMP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--- AUTO DROP ---
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[USP_TCC_DeleteActiveTransaction]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[USP_TCC_DeleteActiveTransaction]
GO

-- =============================================
-- Author:		<lichl>
-- Create date: <2010-04-29>
-- Description:	<删除ActiveTransaction表中内容>
-- =============================================



create PROCEDURE [dbo].[USP_TCC_DeleteActiveTransaction]
	@TxId				uniqueidentifier	

AS
BEGIN
	delete from TCC_ActiveTransaction where TxId=@TxId
END
