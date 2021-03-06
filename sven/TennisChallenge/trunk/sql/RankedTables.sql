use TennisChallenge

CREATE TABLE RankedMember (
RankedMemberKey UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
RankedMemberFk UNIQUEIDENTIFIER NOT NULL UNIQUE FOREIGN KEY REFERENCES UsersInClubs(UsersInClubsKey),
ClubRank INT,
FormerClubRank INT,
SwissTennisRank INT,
)

CREATE TABLE RankedGame (
RankedGameKey	UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
BookingFk UNIQUEIDENTIFIER UNIQUE FOREIGN KEY REFERENCES BookingBase(BookingKey),
WinnerFk UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Member(MemberKey),
Player1ScoreFirst INT,
Player2ScoreFirst INT,
Player1ScoreSecond INT,
Player2ScoreSecond INT,
Player1ScoreTie INT,
Player2ScoreTie INT
)