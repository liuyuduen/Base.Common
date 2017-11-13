
USE DB_Common
GO
if exists(select * from sys.objects where name='UserInfo')
	drop table UserInfo
go

CREATE TABLE UserInfo(
	Id  INT identity(1,1) primary key  NOT NULL,
	LoginName VARCHAR(50) NULL, 
	Password VARCHAR(50) NULL,
	NiceName VARCHAR(50) NULL,
	Name VARCHAR(50) NULL,
	Gender INT NULL,
	Mobile VARCHAR(50) NULL,
	IdentityCard VARCHAR(50) NULL,
	Birthday VARCHAR(50) NULL,
	Address VARCHAR(200) NULL,
	Status INT NOT NULL default(0)
) 


