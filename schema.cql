CREATE TABLE users.vaccinegiven ( userhash text PRIMARY KEY, siteid text, vaccine text, timegiven timestamp );

CREATE TABLE users.location_info (latlocation float, longlocation float, age int, pushToken text, PRIMARY KEY((pushToken),latlocation,longlocation,age));