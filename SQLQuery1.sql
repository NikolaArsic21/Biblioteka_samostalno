USE[Biblioteka1];
GO

CREATE TABLE Clanovi (
	ClanID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	BrClanske INT NOT NULL,
	Ime NVARCHAR(50) NOT NULL,
	Prezime NVARCHAR(50) NOT NULL
);

CREATE TABLE Iznajmljivanja (
	ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	ClanID INT FOREIGN KEY REFERENCES Clanovi(ClanID),
	NaslovKnjige NVARCHAR(100) NOT NULL,
	DatumIznajmljivanja DATE NULL,
	PeriodIznajmljivanja INT NOT NULL
);

INSERT INTO [dbo].[Clanovi] (BrClanske, Ime, Prezime)
VALUES (2001, 'Milica', 'Petrovic'),
	   (2002, 'Stefan', 'Nikolic'),
	   (2003, 'Jelena', 'Jovanovic'),
	   (2004, 'Marko', 'Stojanovic'),
	   (2005, 'Aleksandar', 'Miskovic')

INSERT INTO [dbo].[Iznajmljivanja] (ClanID, NaslovKnjige, PeriodIznajmljivanja)
VALUES (1,'Na Drini cuprija', 14),
	   (2,'Seobe',21),
	   (3,'Dervis i smrt',30),
	   (4,'Seobe',14),
	   (5,'Dervis i smrt',10)
