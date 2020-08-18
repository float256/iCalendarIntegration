IF NOT EXISTS (SELECT [name] FROM [sys].[foreign_keys] WHERE [name] = 'FK_booking_info_id_room')
	ALTER TABLE [booking_info]
		ADD CONSTRAINT [FK_booking_info_id_room]
		FOREIGN KEY ([id_room]) 
		REFERENCES [room]([id_room]) 
		ON DELETE CASCADE
GO
