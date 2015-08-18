BuildVersioning.Commands

The command classes implemented in this assembly will be used in-process with Microsoft Team Foundation Server
and thus some special programming rules need to be implemented.

There are issues when using Entity Framework in-process with Microsoft's Team Build agent host process
therefore, all SQL Server data access must be done using ADO.NET and not Enity Framework.

Issues were encountered when attempting to use Entity Framework 6.1.3 code first with programmatic configuration
that caused either the data access code to break or the host process to crash depending upon the configuration settings used.
