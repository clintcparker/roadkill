CREATE TABLE [roadkill_siteconfiguration]
(
	[Id] [uniqueidentifier] NOT NULL,
	[Version] [nvarchar](255) NOT NULL,
	[Xml] NTEXT NOT NULL,
	PRIMARY KEY NONCLUSTERED (Id)
);