USE TennisChallenge

-- rollback if anything fails
SET XACT_ABORT ON

BEGIN TRANSACTION TableCreation

CREATE TABLE UsersInClubs (
	UsersInClubsKey			UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
	UserFK					UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE ON UPDATE CASCADE,
	ClubFK					UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Club(ClubKey) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT IX_UserFK_ClubFK_unique UNIQUE (UserFK, ClubFK)
)


CREATE TABLE UsersInClubsInRoles (
	UsersInClubsFK			UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES UsersInClubs(UsersInClubsKey) ON DELETE CASCADE ON UPDATE CASCADE,
	RolesFK					UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Roles(RoleId) ON DELETE CASCADE ON UPDATE CASCADE,
	PRIMARY KEY				(UsersInClubsFK, RolesFK),
	CONSTRAINT IX_UsersInClubsFK_RolesFK_unique UNIQUE (UsersInClubsFK, RolesFK)
)

ALTER TABLE Club ADD Name NVARCHAR(250)

GO
-- update current entries
UPDATE Club SET Name = (
	SELECT Name FROM ClubName WHERE Club.ClubKey = ClubName.ClubFk
)

-- add non existing ones
INSERT INTO Club(Name)
	SELECT Name FROM ClubName cn WHERE NOT EXISTS
		(SELECT ClubKey FROM Club c WHERE cn.ClubFk = c.ClubKey)

-- this is only so that we can later have an easier time to enter the users
UPDATE ClubName SET ClubFk = (
	-- this is technically not safe, but we'll probably not run into any problems
	SELECT ClubKey FROM Club WHERE ClubName.Name = Club.Name
)

GO
-- Insert the Guestusers
-- because infoboard users don't have members
INSERT INTO UsersInClubs(UserFK, ClubFK)
	SELECT uir.UserId, c.ClubKey FROM UsersInRoles uir
	JOIN Member m ON uir.UserId = m.MemberKey
	JOIN ClubName cn ON m.ClubNameFk = cn.ClubNameKey
	JOIN Club c ON cn.ClubFk = c.ClubKey
	GROUP BY uir.UserId, c.ClubKey

INSERT INTO UsersInClubsInRoles(UsersInClubsFK, RolesFK)
	SELECT uic.UsersInClubsKey, uir.RoleId FROM UsersInClubs uic
		JOIN UsersInRoles uir ON uic.UserFK = uir.UserId

-- insert the the infoboard users
INSERT INTO UsersInClubs(UserFK, ClubFK)
	SELECT BoardUserFk, ClubFk  FROM InfoboardsInClub

-- give all the infoboard users their new UsersInClubsRoles
INSERT INTO UsersInClubsInRoles (UsersInClubsFK, RolesFK)
	SELECT uic.UsersInClubsKey, r.RoleID FROM UsersInClubs uic
		JOIN Roles r on r.RoleName = 'infoboard'
		WHERE NOT EXISTS
			(SELECT MemberKey FROM Member m WHERE m.MemberKey = uic.UserFK)

GO
INSERT INTO UsersInClubs(UserFK, ClubFK)
	SELECT m.MemberKey, c.Clubkey FROM Member m
		JOIN ClubName cn ON m.ClubNameFk = cn.ClubNameKey
		JOIN Club c ON c.ClubKey = cn.ClubFk
		WHERE NOT EXISTS
			(SELECT uic.UserFK FROM UsersInClubs uic WHERE uic.UserFK = m.MemberKey)
		-- Infoboards don't have a Member, so I don't have to filter anything

GO
INSERT INTO UsersInClubsInRoles(UsersInClubsFK, RolesFK)
	SELECT uic.UsersInClubsKey, r.RoleId FROM UsersInClubs uic
		JOIN Member m ON uic.UserFK = m.MemberKey
		JOIN Roles r ON r.RoleName = 'member'
		WHERE NOT EXISTS
		(SELECT uicir.UsersInClubsFK FROM UsersInClubsInRoles uicir WHERE uicir.UsersInClubsFK = uic.UsersInClubsKey)

ALTER TABLE [Member] DROP CONSTRAINT [FK_Member_ClubName]
ALTER TABLE Member DROP COLUMN ClubNameFK
DROP TABLE InfoboardsInClub
DROP TABLE ClubName
DELETE FROM Club WHERE Name = 'Kein Klub'
DELETE FROM UsersInRoles
DELETE FROM Roles WHERE RoleName = 'Member'

INSERT INTO Roles(ApplicationId, RoleId, RoleName, Description) VALUES('97D2FC2E-C45F-4AA7-AB94-F844E320CEEA', NEWID(), 'clubadmin', 'Kann das Klubprofil verändern.')
INSERT INTO Roles(ApplicationId, RoleId, RoleName, Description) VALUES('97D2FC2E-C45F-4AA7-AB94-F844E320CEEA', NEWID(), 'tennisteacher', 'Kann die Tennisplätze im voraus und beliebig lange buchen.')
INSERT INTO Roles(ApplicationId, RoleId, RoleName, Description) VALUES('97D2FC2E-C45F-4AA7-AB94-F844E320CEEA', NEWID(), 'intercluborganizer', 'Kann die Tennisplätze für Interklubmatches buchen.')
INSERT INTO Roles(ApplicationId, RoleId, RoleName, Description) VALUES('97D2FC2E-C45F-4AA7-AB94-F844E320CEEA', NEWID(), 'casualtournamentorganizer', 'Kann die Tennisplätze für ein Plauschturnier buchen.')
INSERT INTO Roles(ApplicationId, RoleId, RoleName, Description) VALUES('97D2FC2E-C45F-4AA7-AB94-F844E320CEEA', NEWID(), 'janitor', 'Kann Tennisplätze sperren.')
INSERT INTO Roles(ApplicationId, RoleId, RoleName, Description) VALUES('97D2FC2E-C45F-4AA7-AB94-F844E320CEEA', NEWID(), 'eventmanager', 'Kann Partys definieren.')
INSERT INTO Roles(ApplicationId, RoleId, RoleName, Description) VALUES('97D2FC2E-C45F-4AA7-AB94-F844E320CEEA', NEWID(), 'host', 'Kann Tagesmenu definieren.')
INSERT INTO Roles(ApplicationId, RoleId, RoleName, Description) VALUES('97D2FC2E-C45F-4AA7-AB94-F844E320CEEA', NEWID(), 'advertisementmanager', 'Kann Werbung festlegen.')
INSERT INTO Roles(ApplicationId, RoleId, RoleName, Description) VALUES('97D2FC2E-C45F-4AA7-AB94-F844E320CEEA', NEWID(), 'topicalitymanger', 'Kann aktualitäten bearbeiten.')

DELETE FROM BookingBase WHERE Member0Fk IS NULL
ALTER TABLE BookingBase ALTER COLUMN Member0Fk UNIQUEIDENTIFIER NOT NULL
ALTER TABLE BookingBase ADD Comment NVARCHAR(MAX)

CREATE TABLE Advertisements (
	Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL DEFAULT NEWID(),
	Name VARCHAR(MAX) NOT NULL,
	ImageUrl VARCHAR(MAX) NOT NULL,
	StartTime DATETIME NOT NULL,
	EndTime DATETIME NOT NULL,
	Duration INT NOT NULL,
	Monday BIT NOT NULL,
	Tuesday BIT NOT NULL,
	Wednesday BIT NOT NULL,
	Thursday BIT NOT NULL,
	Friday BIT NOT NULL,
	Saturday BIT NOT NULL,
	Sunday BIT NOT NULL,
	Active BIT NOT NULL,
	EMail VARCHAR(MAX), -- eventually for later
	Feedback BIT, -- eventually for later
	ClubKey UNIQUEIDENTIFIER NOT NULL REFERENCES Club(ClubKey) ON UPDATE CASCADE ON DELETE CASCADE
)

CREATE TABLE NewsFeeds (
	Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL DEFAULT NEWID(),
	Name VARCHAR(MAX),
	Url VARCHAR(MAX),
	Duration INT NOT NULL,
	Active BIT NOT NULL,
	ClubKey UNIQUEIDENTIFIER NOT NULL REFERENCES Club(ClubKey) ON UPDATE CASCADE ON DELETE CASCADE
)

CREATE TABLE AdvertisementShowings (
	Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL DEFAULT NEWID(),
	Showed DATETIME NOT NULL,
	AdvertisementFk UNIQUEIDENTIFIER NOT NULL REFERENCES Advertisements(Id) ON UPDATE CASCADE ON DELETE CASCADE
)

CREATE TABLE NewsFeedShowings (
	Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL DEFAULT NEWID(),
	Showed DATETIME NOT NULL,
	NewsFeedFk UNIQUEIDENTIFIER NOT NULL REFERENCES NewsFeeds(Id) ON UPDATE CASCADE ON DELETE CASCADE
)

ALTER TABLE Club ADD AdvertisementsPerNewsFeed INTEGER NOT NULL DEFAULT 0

COMMIT TRANSACTION TableCreation
