# Start from Alpine since your compose file uses it
FROM alpine:latest

# Install NATS server and basic tools
RUN apk add --no-cache nats-server bash

# Copy your websocket-proxy binary
WORKDIR /app
COPY websocket-proxy /app/websocket-proxy

# Make sure it’s executable
RUN chmod +x /app/websocket-proxy

# Expose ports (optional)
EXPOSE 8080 4222 6222 8222

# Start both NATS and websocket-proxy
CMD bash -c "nats-server -DV & \
  echo 'Waiting for NATS to start...' && \
  sleep 3 && \
  echo 'Starting websocket-proxy...' && \
  /app/websocket-proxy"
