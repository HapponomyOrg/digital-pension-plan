## Docker

To make life easier we have made a `docker-compose.yml` file.

This docker container contains the following applications:
 - [Nats](../nats/nats.md)
 - [Postgres](../postgres/postgres.md)
 - [PGAdmin](../postgres/pgadmin.md)

First run : ``docker compose up`` in your terminal. \
Afterwords you can either start the docker image from the docker environment.

## Hosting
During our project we made use of an [AWS EC2](https://aws.amazon.com/ec2/) environment to host services at one global place.\
What you can do is that you [transfer this docker container to their service](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/create-container-image.html).
You also have to change all credentials in the files like the url of the NATS instance in ./NATS/Connection.cs
