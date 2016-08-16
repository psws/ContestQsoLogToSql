USE [ContestqsoData]
GO


/*DROP-CREATE THE PROC*/
--Check for Object and delete if exists
IF OBJECT_ID(N'[dbo].[CQD_sp_QsoInsertContacts]') IS NOT NULL DROP PROCEDURE [dbo].[CQD_sp_QsoInsertContacts]
/*DROP-CREATE THE TABLE TYPE*/
--Check for Object and delete if exists
IF TYPE_ID(N'[dbo].[udt_QsoContacts]') IS NOT NULL DROP TYPE [dbo].[udt_QsoContacts]
GO

CREATE TYPE [dbo].[udt_QsoContacts] AS TABLE(  
QsoNo smallint NOT NULL,
LogId int NOT NULL,    
StationName varchar(20) NULL,
Frequency decimal(18,2) NOT NULL,
CallsignId int NOT NULL,  
QsoDateTime datetime NOT NULL, 
RxRst smallint NOT NULL,
TxRst smallint NOT NULL,
QsoExchangeNumber smallint NULL,
QsoModeTypeEnum int NOT NULL,
QsoRadioTypeEnum int NOT NULL
);
Go

--Create the Proc
CREATE PROCEDURE [dbo].[CQD_sp_QsoInsertContacts](@InsertContacts udt_QsoContacts READONLY)
AS   
    SET NOCOUNT ON  
    INSERT INTO dbo.Qso  
           (QsoNo  
		   ,LogId
		   ,StationName
		   ,Frequency
		   ,CallsignId
		   ,QsoDateTime
		   ,RxRst
		   ,TxRst
		   ,QsoExchangeNumber
		   ,QsoModeTypeEnum
		   ,QsoRadioTypeEnum 
           ,QZoneMult  
           ,QCtyMult  
           ,QPrefixMult  
           ,QPts1  
           ,QPts2  
           ,QPts4  
           ,QPts8)  
        SELECT *, 0, 0, 0,  0,  0,  0,  0    
        FROM  @InsertContacts;  
        GO  

   
GRANT EXEC ON [dbo].[CQD_sp_QsoInsertContacts] TO jims9m8r
GO
GRANT EXEC ON TYPE ::[dbo].udt_QsoContacts TO jims9m8r;
GO 