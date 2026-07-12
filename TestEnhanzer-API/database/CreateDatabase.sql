 

IF DB_ID(N'TestEnhanzerDb') IS NULL
BEGIN
    CREATE DATABASE [TestEnhanzerDb];
END;
GO

USE [TestEnhanzerDb];
GO
 


BEGIN TRANSACTION;
CREATE TABLE [UserDetails] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(150) NOT NULL,
    [CreatedAtUTC] datetime2 NOT NULL,
    [UpdateAtUTC] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_UserDetails] PRIMARY KEY ([Id])
);

CREATE TABLE [Location_Details] (
    [Id] int NOT NULL IDENTITY,
    [UserDetailsId] int NOT NULL,
    [Location_Code] nvarchar(100) NOT NULL,
    [Location_Name] nvarchar(200) NOT NULL,
    [CreatedAtUTC] datetime2 NOT NULL,
    [UpdateAtUTC] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Location_Details] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Location_Details_UserDetails_UserDetailsId] FOREIGN KEY ([UserDetailsId]) REFERENCES [UserDetails] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_Location_Details_UserDetailsId_Location_Code] ON [Location_Details] ([UserDetailsId], [Location_Code]);

CREATE UNIQUE INDEX [IX_UserDetails_Username] ON [UserDetails] ([Username]);
