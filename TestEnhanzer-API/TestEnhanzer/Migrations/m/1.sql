IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
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

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260711203026_jul12', N'9.0.8');

COMMIT;
GO

