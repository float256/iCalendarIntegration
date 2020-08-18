IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'hotel')
BEGIN
	CREATE TABLE [dbo].[hotel](
		[id_hotel] [int] IDENTITY(1,1) NOT NULL,
		[hotel_code] [nvarchar](max) NOT NULL,
		[login] [nvarchar](max) NOT NULL,
		[password] [nvarchar](max) NOT NULL,
		[name] [nvarchar](max) NOT NULL,
		CONSTRAINT [PK_hotel] PRIMARY KEY CLUSTERED
		(
			[id_hotel] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
