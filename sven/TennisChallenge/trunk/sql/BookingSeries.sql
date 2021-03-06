USE TennisChallenge
GO

CREATE TABLE BookingSeries(
	Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL DEFAULT NEWID(),
	Name VARCHAR(MAX) NOT NULL
)

ALTER TABLE BookingBase ADD BookingSeriesFk UNIQUEIDENTIFIER REFERENCES BookingSeries(Id) ON UPDATE CASCADE ON DELETE CASCADE