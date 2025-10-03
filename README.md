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

* For testing the program should be run through visual studio(2022 was used in the development)

### Installing

* If testing download the source code from the latest release. Otherwise download the compiled version.
* Extract the downloaded zip

### Executing program
For testing do the following
* If testing double click on the solution file to open in visual studio.
* Then ensure CerealAPI is selected at the top and that it runs as https.
* Click the https text to run it.
* A swagger window should open in which the various API calls can be triggered.

For general use
* TODO

## Version History

* 1.0
    * Initial Release
* 1.1
   * Moved functionallity of Server Configuration to API and removed server configuration 
