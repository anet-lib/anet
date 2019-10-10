ALTER TABLE [FooTable] ADD [Column2] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20191010153422_002', N'3.0.0');

GO

