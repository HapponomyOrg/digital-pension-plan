FROM ubuntu:latest

# Install necessary packages
RUN apt-get update && apt-get install -y \
    postgresql \
    nats-server \
    pgadmin4

# Configure PostgreSQL
# You may need to copy your PostgreSQL configuration files into the container
# and set up the database schema, users, and permissions here

# Configure NATS
# You may need to copy your NATS configuration file into the container here

# Expose ports
EXPOSE 5432 4222 8222 6222 8080 5050

# Start services
CMD service postgresql start && \
    service nats-server start && \
    service apache2 start && \
    service cron start && \
    service pgadmin4 start && \
    /bin/bash