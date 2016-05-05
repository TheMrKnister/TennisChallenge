USE [TennisChallenge]
GO

/****** Object:  Table [dbo].[UserAd]    Script Date: 04/15/2014 10:11:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[UserAd](
	[UserAdId] [uniqueidentifier] NOT NULL,
	[UserInClubFk] [uniqueidentifier] NULL,
	[AdText] [varchar](max) NULL,
	[AdType] [int] NULL,
	[CreationTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserAdId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[UserAd]  WITH CHECK ADD FOREIGN KEY([UserInClubFk])
REFERENCES [dbo].[UsersInClubs] ([UsersInClubsKey])
GO

ALTER TABLE [dbo].[UserAd] ADD  DEFAULT (newid()) FOR [UserAdId]
GO


