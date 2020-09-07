IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'availability_status_message')
BEGIN
	CREATE TABLE [dbo].[availability_status_message](
		[id_availability_status_message] [int] IDENTITY(1,1) NOT NULL,
		[id_room][int] NOT NULL,
		[start_date][datetime] NOT NULL,
		[end_date][datetime] NOT NULL,
		[tl_api_code][nvarchar](max) NOT NULL,
		[state][nvarchar](max) NOT NULL,
		CONSTRAINT [PK_availability_status_message] PRIMARY KEY CLUSTERED
		(
			[id_availability_status_message] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
		CONSTRAINT [FK_availability_status_message_id_room]
            FOREIGN KEY ([id_room]) 
            REFERENCES [room]([id_room]) 
            ON DELETE NO ACTION,
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
