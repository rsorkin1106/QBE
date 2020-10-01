# QBE

Query By Example Users Guide
By: Robert Sorkin
 
## Table of Contents

Introduction

Starting Screen

Add Query

Edit Query

Execute Query

 
##Introduction

This QBE has the basic functionalities of any QBE such as filtering, adding, editing, and executing queries.
The values in the table on the starting screen go as follow:

-	ID

o	The specific ID of that query


-	StatusID

o	Whether or not the query is currently active (1 – active, 2 – inactive)


-	TypeID

o	The type of query you wish to perform (1 – Stored Procedure, 2 – View, 3 – Table)


-	Name

o	The name given to the saved query and specific parameter/values inputted


-	Description

o	Description of what that query does


-	Query

o	The name of the query


-	Parameters

o	List of parameters chosen as well as values entered


-	CreateDate

o	The time the query was first added to the database


-	UpdateDate

o	The time the query was most recently added/edited/executed 


## Starting Screen

 
The starting screen displays all available queries in the system in the top half of the application. Filling in the textboxes and pressing “Search” will filter the grid to only display queries that match every entered field. Pressing “Clear” will display all queries and clear all text box entries.
To add a query to the database, type in a 1 (Stored Procedure), 2 (View), or 3 (Table) into the TypeID field and the query dropdown box will display all possible options for the type you picked. Once a query is selected, the “Add” button becomes available to press.
To edit, select an entry in the display grid. Once a query is selected, the “Edit” button becomes available to press.
To execute, select an entry in the display grid. Once a query is selected, the “Execute” button becomes available to press.
 
## Add Query
 
Once “Add” is pressed on the starting screen, a window will popup to add a query. The TypeID and Query values are received from when they were entered on the starting screen as well as any description or name. If no description or name is typed on the starting screen, they will start off blank. The box at the bottom left of the screen holds all parameters/columns depending on TypeID where you can select which parameters/columns you wish to use. For every box checked, a blank textbox will appear next to it where you can input the value for every parameter. Once you have filled everything in, press “Add” to add the query to the database.
 

## Edit Query

 
Once “Edit” is pressed on the starting screen, a window will popup to edit the selected query. All parameters/values recently entered will appear. You may check/uncheck additional boxes if required for your edit. You are also allowed to change the name and description of the queries. When you are done, press edit to finalize changes. 
Execute Query
 

## Execute Query


Once “Execute” is pressed on the starting screen, a window will popup showing the name of the query and the selected parameter/value pairs where you can change any values if you wish. Pressing execute will execute the query and display the returned dataset in the grid at the top.
