ALTER TABLE [FooTable] ADD [Column1] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20191010153212_001', N'3.0.0');

GO

