services:
  postgres:
    image: postgres:latest
    restart: always
    environment:
      POSTGRES_USER: myuser
      POSTGRES_PASSWORD: mypassword
      POSTGRES_DB: mydatabase
    ports:
      - "5432:5432"
    volumes:
      - ./db/postgresql/data:/var/lib/postgresql/data  # Relative path
    platform: linux
    
  nats:
    image: nats:latest
    restart: always
    ports:
      - "4222:4222"
      - "8222:8222"
      - "6222:6222"
    platform: linux
    
  pgadmin:
    image: dpage/pgadmin4:latest
    restart: always
    environment:
      PGADMIN_DEFAULT_EMAIL: myemail@example.com
      PGADMIN_DEFAULT_PASSWORD: mypassword
    ports:
      - "5050:80"
    depends_on:
      - postgres
    platform: linux