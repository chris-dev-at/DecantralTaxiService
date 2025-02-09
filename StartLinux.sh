#!/bin/bash

# Change the directory to the project directory
cd AndersTaxi

# Build and start the Docker containers
docker-compose up -d --build

# Tell user what he is waiting for
echo "Waiting for Blazor to start..."

# Wait for Blazor to start
sleep 15

# Open the webpage in the default browser on Linux
xdg-open "http://localhost:8000"