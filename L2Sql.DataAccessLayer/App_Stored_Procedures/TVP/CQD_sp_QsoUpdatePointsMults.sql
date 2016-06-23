USE [ContestqsoData]
GO


/*DROP-CREATE THE PROC*/
--Check for Object and delete if exists
IF OBJECT_ID(N'[dbo].[CQD_sp_QsoUpdatePointsMults]') IS NOT NULL DROP PROCEDURE [dbo].[CQD_sp_QsoUpdatePointsMults]
/*DROP-CREATE THE TABLE TYPE*/
--Check for Object and delete if exists
IF TYPE_ID(N'[dbo].[udt_QsoPointsMults]') IS NOT NULL DROP TYPE [dbo].[udt_QsoPointsMults]
GO

CREATE TYPE [dbo].[udt_QsoPointsMults] AS TABLE(  
QsoNo smallint NOT NULL,
LogId int NOT NULL,    
QZoneMult bit NOT NULL,
QCtyMult bit NOT NULL,
QPrefixMult bit NOT NULL,
QPts1 bit NOT NULL,
QPts2 bit NOT NULL,
QPts4 bit NOT NULL,
QPts8 bit NOT NULL
);
Go

--Create the Proc
CREATE PROCEDURE [dbo].[CQD_sp_QsoUpdatePointsMults](@UpdatedQsoPointsMult udt_QsoPointsMults READONLY)
AS UPDATE dbo.Qso
   SET 
   Qso.QZoneMult = ec.QZoneMult,
   Qso.QCtyMult = ec.QCtyMult,
   Qso.QPrefixMult = ec.QPrefixMult,
   Qso.QPts1 = ec.QPts1,
   Qso.QPts2 = ec.QPts2,
   Qso.QPts4 = ec.QPts4,
   Qso.QPts8 = ec.QPts8
  FROM Qso INNER JOIN @UpdatedQsoPointsMult AS ec
   ON Qso.QsoNo = ec.QsoNo AND Qso.LogId = ec.LogId;
GO
   
GRANT EXEC ON [dbo].[CQD_sp_QsoUpdatePointsMults] TO jims9m8r
GO
GRANT EXEC ON TYPE ::[dbo].[udt_QsoPointsMults] TO jims9m8r;
GO 