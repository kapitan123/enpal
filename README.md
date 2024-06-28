# How to run
## Run
* Install docker <a href="https://docs.docker.com/desktop/">Docker engine and Compose plugin</a></br>
* Run `docker compose --profile all up` from `./SchedulerApi/SchedulerApi`
* Open swagger on localhost:3000 <a href="http://localhost:3000/swagger/index.html">this link</a> 

# Shortcuts and assumptions 
No unit tests were created as they were not listed as a requirement, and there isn't much to test without a database.
It uses a default dockerfile without security best practices