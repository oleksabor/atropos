This should be set of tools to watch for users curfew. 

It has been developed since Windows 10 allows to manage curfew for Microsoft based accounts only. However I do not suppose that my children have to have MS account to use home computer. But I'd like to restrict their computer usage that is why I've used Windows 8 curfew. 

The problem is that this functionality has gone for local accounts in Windows 10.

The idea it to manage certain local account curfew from local administrator account. Or you can leave curfew empty and use Atropos as additional argument to help your child set up daily tasks.

Atropos has two major components - server and client. Service is started using local system account. Service checks is computer used and should it be blocked or not.

Client is started under current user account and can be used to check how many time had been used by user.

### client

This is small application that shows registred users that can be restricted with curfew. Read only.

![client window](doc/images/clientWindow.png)

Curfew are editable (but there is no validation yet). Users list still is read-only. You have to login with each user to edit curfew.
 

### server

Atropos is a tool that counts how many time person is logged on the computer. Server is started always and watches for user logged in. It constantly updates database if computer is used.

Please note that by defailt server logs *every* user time consumption even if it was not configured to be restricted with curfew.

It was built on .NETFramework,Version=v4.7.2 using packages below
* LibLog to add logging framework abstraction
* ling2db to store data in the Sqlite database
* NLog to log data. However it can be changed to any of logging package that is supported by LibLog 
* nlog.IndentException to adjust exceptions format
* StructureMap as DI container
* Topshelf to run as Windows service

service version is ready. It can read db parameterization and lock current user session if curfew is exceeded. 

#### installation
You can use GUI setup or install service and client manually 

##### GUI setup

Please download the latest setup package from the Releases and run it. The serice will be installed and started automatically. Setup files are named _atropos.VERSIONNUMBER.exe_

##### manual installation 

Please download client and service binaries from the Releases and extract archives to the local drive. Client and service zip files are named _atroposClientRelease.VERSIONNUMBER.7z_ and _atroposServiceRelease.VERSIONNUMBER.7z_

You have to 
* [install service](https://topshelf.readthedocs.io/en/latest/overview/commandline.html#topshelf-command-line-reference) by running `atropos.server install --localsystem` from command line using elevated command prompt (Run As Administrator). This will install service that is started using built-in LocalSystem Windows account.
* start service. If service is started first time and there is no database created it creates empty database named `AtroposData.sqlite`

client executable has to be placed somewhere on the computer. When started it will show icon in the system tray area.

#### configuration

Atropos client that is started as local administrator can be used to adjust curfew for local users. You can add, edit or remove user's curfews using approtpriate buttons on the client form.

You have to logon with each user with Atropos service installed to enable curfew parametrization.

It is ready and will lock user screen each 30 seconds if user has exceeded allowed time. I'm going to implement option to shutdown Windows if time was exceeded. 


