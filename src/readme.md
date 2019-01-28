This should be set of tools to watch users curfew. 

It has been developed since Windows 10 allows to manage curfew for Microsoft based accounts only. However I do not suppose that my children have to have MS account to use home computer. But I'd like to restrict their computer usage that is why I've set up Windows 8 curfew. The problem is that this functionality has gone after Windows 10 update for local account.

The idea it to manage certain local account curfew from local administrator account. 

Atropos has two major components - server and client.

### server

Atropos is a tool that counts how many time person is logged on the computer. Server is started always and watches for user logged in. It constantly updates database if computer is used.

Please note that by defailt server logs *every* user time consumption even if it was not configured to be restricted with curfew.

It was built using packages below
* LibLog to add logging framework abstraction
* ling2db to store data in the Sqlite database
* NLog to log data. However it can be changed to any of logging package that is supported by LibLog 
* nlog.IndentException to adjust exceptions format
* StructureMap as DI container
* Topshelf to run as Windows service

### client
(development is not started)

This is small application that should allow to manage users that has to be restricted with curfew. 

