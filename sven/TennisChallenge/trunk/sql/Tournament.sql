use TennisChallenge

CREATE TABLE Tournaments (
TournamentId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
TournamentType INT,
TournamentOpen INT,
Mode INT,
)

CREATE TABLE TournamentLadder (
LadderId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
List NVARCHAR(max)
)

ALTER TABLE TournamentLadder 
ADD  FOREIGN KEY (LadderId) REFERENCES Tournaments(TournamentId)
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE BookingBase
ADD TournamentId UNIQUEIDENTIFIER, TournamentRound INT;

ALTER TABLE BookingBase
ADD FOREIGN KEY (TournamentId)
REFERENCES Tournaments(TournamentId)
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE Member 
ADD TournamentId UNIQUEIDENTIFIER, TournamentPoints INT;

ALTER TABLE Member
ADD FOREIGN KEY (TournamentId)
REFERENCES Tournaments(TournamentId)
ON UPDATE CASCADE
ON DELETE SET NULL