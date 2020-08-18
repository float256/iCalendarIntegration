IF NOT EXISTS (SELECT [name] FROM [sys].[foreign_keys] WHERE name = 'FK_id_hotel')
	ALTER TABLE [room]
		ADD CONSTRAINT FK_id_hotel 
		FOREIGN KEY (id_hotel) 
		REFERENCES [hotel](id_hotel) 
		ON DELETE CASCADE
GO
