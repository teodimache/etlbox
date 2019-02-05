# Release Notes

## Version 1.0.0

Initial release working on .NET Core.

## Version 1.0.1

Reorganization of namespaces.

## Version 1.1.0

* `DropDatabaseTask`: static "convenience" method name changed from delete to drop 
* `ConnectionManager` (general) improved:verified that the underlying ADO.NET connection pooling is working (see Issue#1)
* `OdbcConnectionManager`: ETLBox now supports connection via Odbc. (64bit only)
* `AccessOdbcConnectionManager`: ETLBox can now connect to access databases via ODBC (64bit ODBC driver required)
* `DBSource`: now accepts table name (instead of full table definition)
* `ExcelSource`: ETLBox can now read from excel files. 
* `CSVSource<CSVData>`: Adding a generic implementation for the CSVSource.
* `RowTransformation`: Adding a non generic implementation (same as `RowTansformation<string[],string[]`>)
* `DBDestination`: Adding a non generic implemnetation. (same as `DBDestination<string[]`>)

## Version 1.1.1
* `DBDestination`: Fixed issue (#4) when destination table has more columns than input type.