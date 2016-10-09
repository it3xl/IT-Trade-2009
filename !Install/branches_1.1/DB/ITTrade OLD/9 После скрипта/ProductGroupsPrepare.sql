


-- !!! перед запуском убедиться, что Products содержит колонку ProductGroupID






USE [ITTrade]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Products_ProductGroups]') AND parent_object_id = OBJECT_ID(N'[dbo].[Products]'))
ALTER TABLE [dbo].[Products] DROP CONSTRAINT [FK_Products_ProductGroups]
GO










IF COLUMNPROPERTY(OBJECT_ID(N'[dbo].Products','U'), N'ProductGroupID', 'AllowsNull') IS NULL
Begin
	PRINT 'Нет колонки';
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
	ALTER TABLE dbo.Products ADD
		ProductGroupID int NULL
	ALTER TABLE dbo.Products SET (LOCK_ESCALATION = DISABLE)
	COMMIT
end
else
print 'Есть колонка';
























/****** Object:  Table [dbo].[ProductGroups]    Script Date: 06/14/2009 03:15:59 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductGroups]') AND type in (N'U'))
DROP TABLE [dbo].[ProductGroups]
GO

USE [ITTrade]
GO

/****** Object:  Table [dbo].[ProductGroups]    Script Date: 06/14/2009 03:15:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ProductGroups](
	[ProductGroupID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_ProductGroups] PRIMARY KEY CLUSTERED 
(
	[ProductGroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Index [IX_ProductGroups_UnicName]    Script Date: 06/14/2009 03:18:25 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ProductGroups]') AND name = N'IX_ProductGroups_UnicName')
DROP INDEX [IX_ProductGroups_UnicName] ON [dbo].[ProductGroups] WITH ( ONLINE = OFF )
GO

/****** Object:  Index [IX_ProductGroups_UnicName]    Script Date: 06/14/2009 03:18:25 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ProductGroups_UnicName] ON [dbo].[ProductGroups] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO




SET ANSI_PADDING OFF
GO













insert into ProductGroups (Name) select distinct case when p.[ТоварнаяГруппа] is not null then p.[ТоварнаяГруппа] else '' end n from dbo.Products p order by n

--select * from ProductGroups



update Products set ТоварнаяГруппа='' where Products.ТоварнаяГруппа is null


update Products
set ProductGroupID=pg.ProductGroupID
from
ProductGroups pg
where pg.Name=Products.ТоварнаяГруппа




/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
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
GO
ALTER TABLE dbo.ProductGroups SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Products ADD CONSTRAINT
	FK_Products_ProductGroups FOREIGN KEY
	(
	ProductGroupID
	) REFERENCES dbo.ProductGroups
	(
	ProductGroupID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Products SET (LOCK_ESCALATION = DISABLE)
GO
COMMIT






/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
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
GO
ALTER TABLE dbo.Products
	DROP CONSTRAINT DF_Products_CanDelete
GO
ALTER TABLE dbo.Products
	DROP CONSTRAINT DF_Products_CreationDate
GO
ALTER TABLE dbo.Products
	DROP CONSTRAINT DF_Products_ModifyDate
GO
CREATE TABLE dbo.Tmp_Products
	(
	ProductID bigint NOT NULL IDENTITY (1, 1),
	Barcode varchar(20) NOT NULL,
	ProductGroupID int NOT NULL,
	Article varchar(100) NULL,
	ОптоваяЦена decimal(8, 2) NOT NULL,
	РозничнаяЦена decimal(8, 2) NOT NULL,
	DiscountForbidden bit NOT NULL,
	CanDelete bit NOT NULL,
	CreationDate datetime2(7) NOT NULL,
	ModifyDate datetime2(7) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Products SET (LOCK_ESCALATION = DISABLE)
GO
ALTER TABLE dbo.Tmp_Products ADD CONSTRAINT
	DF_Products_CanDelete DEFAULT ((1)) FOR CanDelete
GO
ALTER TABLE dbo.Tmp_Products ADD CONSTRAINT
	DF_Products_CreationDate DEFAULT (getdate()) FOR CreationDate
GO
ALTER TABLE dbo.Tmp_Products ADD CONSTRAINT
	DF_Products_ModifyDate DEFAULT (getdate()) FOR ModifyDate
GO
SET IDENTITY_INSERT dbo.Tmp_Products ON
GO
IF EXISTS(SELECT * FROM dbo.Products)
	 EXEC('INSERT INTO dbo.Tmp_Products (ProductID, Barcode, ProductGroupID, Article, ОптоваяЦена, РозничнаяЦена, DiscountForbidden, CanDelete, CreationDate, ModifyDate)
		SELECT ProductID, Barcode, ProductGroupID, Article, ОптоваяЦена, РозничнаяЦена, DiscountForbidden, CanDelete, CreationDate, ModifyDate FROM dbo.Products WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Products OFF
GO
ALTER TABLE dbo.Закупки
	DROP CONSTRAINT FK_Закупки_Products
GO
DROP TABLE dbo.Products
GO
EXECUTE sp_rename N'dbo.Tmp_Products', N'Products', 'OBJECT' 
GO
ALTER TABLE dbo.Products ADD CONSTRAINT
	PK_Products PRIMARY KEY CLUSTERED 
	(
	ProductID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Products ADD CONSTRAINT
	IX_Barcode_Unique UNIQUE NONCLUSTERED 
	(
	Barcode
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Products ADD CONSTRAINT
	CK_Products_BarcodeNotEmptyString CHECK (([Barcode]<>''))
GO
ALTER TABLE dbo.Products ADD CONSTRAINT
	CK_Products_ОптоваяЦенаMoreZero CHECK (((0)<[ОптоваяЦена]))
GO
ALTER TABLE dbo.Products ADD CONSTRAINT
	CK_Products_РозничнаяЦенаMoreZero CHECK (((0)<[РозничнаяЦена]))
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Закупки ADD CONSTRAINT
	FK_Закупки_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Закупки SET (LOCK_ESCALATION = TABLE)
GO
COMMIT








/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
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
GO
ALTER TABLE dbo.ProductGroups SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Products ADD CONSTRAINT
	FK_Products_ProductGroups FOREIGN KEY
	(
	ProductGroupID
	) REFERENCES dbo.ProductGroups
	(
	ProductGroupID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Products SET (LOCK_ESCALATION = DISABLE)
GO
COMMIT



SELECT *
  FROM [dbo].[Products]
