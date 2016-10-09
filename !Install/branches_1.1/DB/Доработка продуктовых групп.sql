use ITTrade


declare @process bit
set @process=0
if(@process<>0)
begin

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT

BEGIN TRANSACTION
ALTER TABLE dbo.ProductGroups ADD
	DateCreation datetime2(7) NOT NULL CONSTRAINT DF_ProductGroups_DateCreation DEFAULT getdate()
ALTER TABLE dbo.ProductGroups SET (LOCK_ESCALATION = TABLE)
COMMIT




BEGIN TRANSACTION

update ITTrade.dbo.ProductGroups set DateCreation=DATEADD(day, -1 * ProductGroupID, DateCreation) 

select pg.ProductGroupID, pg.DateCreation, pg.Name from ITTrade.dbo.ProductGroups pg

--rollback
commit


end
