IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'room_upload_status')
BEGIN
	CREATE TABLE [dbo].[room_upload_status](
		[id_room_upload_status] [int] IDENTITY(1,1) NOT NULL,
		[id_room][int] NOT NULL,
		[status][nvarchar](max) NOT NULL,
		[message][nvarchar](max) NOT NULL,
		CONSTRAINT [PK_room_upload_status] PRIMARY KEY CLUSTERED
		(
			[id_room_upload_status] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
		CONSTRAINT [FK_room_upload_status_id_room]
        FOREIGN KEY ([id_room]) 
            REFERENCES [room]([id_room]) 
            ON DELETE CASCADE
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
