# Message Broker App

## Overview
This application consists of two .NET services that communicate using RabbitMQ:
- **Producer**: Sends messages containing a timestamp and a counter.
- **Consumer**: Processes messages based on their timestamp and applies business logic.

## Features
- **Message Production**: The producer continuously generates messages with a timestamp and a counter.
- **Message Processing**: The consumer retrieves messages from the queue and processes them based on their timestamp:
  - If the message is older than 1 minute, it is discarded.
  - If the message’s timestamp has an even number of seconds, it is stored in a database.
  - If the message’s timestamp has an odd number of seconds, the counter is incremented, and the message is requeued.
- **Unit Testing**: Core business logic is covered by unit tests.

## Technologies Used
- **.NET 8 **
- **RabbitMQ** (Message Broker)
- **PostgreSQL** (Database for storing valid messages)
- **Docker** (Optional, for running RabbitMQ in a container)

## Installation & Setup
### Prerequisites
- Install [.NET SDK](https://dotnet.microsoft.com/download)
- Install [RabbitMQ](https://www.rabbitmq.com/download.html) or use Docker
- Install PostgreSQL or MongoDB if using a database

### Running with Docker
You can use Docker to run RabbitMQ locally:
```sh
docker-compose up -d
```

### Running the Producer
```sh
cd Producer
 dotnet run
```

### Running the Consumer
```sh
cd Consumer
dotnet run
```

## Project Structure
```
MessageBrokerApp/
│── Producer/        # Producer Service
│── Consumer/        # Consumer Service
│── Shared/          # Shared models and utilities
│── Tests/           # Unit Tests
│── docker-compose.yml  # Docker setup for RabbitMQ
│── README.md        # Documentation
│── MessageBrokerApp.sln  # Solution file
```

## Future Enhancements
- Dynamic Configuration of values


