IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'booking_info')
BEGIN
	CREATE TABLE [dbo].[booking_info](
		[id_booking_info] [int] IDENTITY(1,1) NOT NULL,
		[id_room] [int] NOT NULL,
		[start_booking] [datetime] NOT NULL,
		[end_booking] [datetime] NOT NULL,
		CONSTRAINT [PK_booking_info] PRIMARY KEY CLUSTERED
		(
			[id_booking_info] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
	) ON [PRIMARY]
END
GO
