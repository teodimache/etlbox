# Todo
- Tests: Use RowCountTask instead of SqlTask where a count is involved

- Custom Parameter should be part of a control flow (as a static object?) or a typed control flow? Or a parameter helper object that can be used for own paramter object?
   - Add Parameter object to ControlFlow which can contain parameter (also, add a check parameter() action and the xml parse logic)

- RowCountTask
  Adding group by and having to RowCount?

- New Tasks:
	Add Ola Hallagren script for database maintenance (backup, restore, ...)

- Dataflow: 
  Mapping to objects has some kind of implicit data type checks - there should be a dataflow task which explicit type check on data 

- CreateTableTask
  Function for adding test data into table (depending on table definition)  
