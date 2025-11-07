# Cereal API

This project contains an API and basic database managment system for a database containing info about various cereals.

## Description

This program is build around a MySQL database containing information about cereals.
It contains the following tables and collumns:
* api_log
    * acces_time
    * command
    * arguments
    * result
* users
    * username
    * psswrd
* cereal
    * id
    * cereal_name
    * mfr
    * cereal_type
    * calories
    * protein
    * fat
    * sodium
    * fiber
    * carbo
    * sugars
    * potass
    * vitamins
    * shelf
    * weight
    * cups
    * rating

To communicate with the database it uses the CerealContext class. It has the following API's and endpoints(cerealinfo means it takes any collumn from the cereal table as an argument):
* Admin(used for various server administration tasks)
    * GetLogs(username, password)
    * AddUser(username, password)
    * UpdatePassword(username, password)
* File
    * GetImage(id)
    * InsertFromCSV(location, name, username, password)
* Cereal
    * GetCereal(cerealinfo, sort)
    * DeleteCereal(username, password, id)
    * AddCereal(cerealinfo, username, password)
    * UpdateCereal(cerealinfo, username, password)
## Getting Started

### Dependencies
* BCrypt.Net-NExt
* coverlet.collector
* Csv
* Microsoft.NET.Test.Sdk
* Microsoft.VisualSTudio.Web.CodeGeneration.Design
* MySql.Data
* NUnit
* NUnit.Analyzers
* Nunit3TestAdapter
* Pomelo.EntityFrameworkCore.MySql
* Swashbuckle.AspNetCore
* Docker
* Docker Compose
* Docker Desktop(Optional)
### Installing

* Download the source code
* Extract the downloaded zip

### Executing program
Run the following docker command: 
```
docker-compose  -f "docker-compose.yml" -f "docker-compose.override.yml" -f "docker-compose.overideextra.yml" --ansi never up -d
```
Or start it through visual studio

## Version History

* 1.0
    * Initial Release
* 1.1
   * Moved functionallity of Server Configuration to API and removed server configuration 
* 1.2
   * Reconfigured program to use docker
