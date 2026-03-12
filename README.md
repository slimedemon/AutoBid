# AutoBid

## Overview
AutoBid is a microservices-based auction platform built with .NET and Next.js. It enables users to create auctions and place bids in real time, using an event-driven architecture designed for scalability and maintainability.

## Key Features
- Real-time bidding workflow
- User authentication and authorization
- Auction creation, updates, and lifecycle tracking
- Independent, containerized microservices
- REST and gRPC communication across services

## Technology Stack
- Backend: .NET 8+, C#, Duende IdentityServer, MassTransit, EF Core, SignalR, Grpc
- Frontend: Next.js, React, TypeScript, Tailwind CSS
- Databases: PostgreSQL, MongoDB
- Infrastructure: Docker, Kubernetes, RabbitMQ, Nginx, YARP

## Web App Environment Files (.env.*)

The Next.js app in frontend/web-app uses .env files for auth and API URLs.

### Which Files to Create
- frontend/web-app/.env.local: used for local development (npm run dev)
- frontend/web-app/.env.production.local: used for production-style frontend runtime

### Create from Example Files

Windows (PowerShell):
```powershell
Copy-Item frontend/web-app/.example.env.local frontend/web-app/.env.local
Copy-Item frontend/web-app/.example.env.production.local frontend/web-app/.env.production.local
```

macOS/Linux:
```bash
cp frontend/web-app/.example.env.local frontend/web-app/.env.local
cp frontend/web-app/.example.env.production.local frontend/web-app/.env.production.local
```

### Required Variables
In frontend/web-app/.env.local:
- AUTH_SECRET
- API_URL
- ID_URL
- AUTH_URL
- NEXT_PUBLIC_NOTIFY_URL

In frontend/web-app/.env.production.local:
- NEXT_PUBLIC_NOTIFY_URL

### Important Notes
- Replace AUTH_SECRET with your own secure value for real deployments.
- Do not commit real secrets. frontend/web-app/.gitignore ignores .env* by default.
- If you run the web app via Docker Compose, environment variables are already supplied by compose for the web-app container.

## Local Development with Docker Compose

### Prerequisites
- Docker Desktop (or Docker Engine + Compose plugin)

### Quick Start
1. Clone the repository.
2. Open a terminal in the project root.
3. Start all services:

```bash
docker compose up -d --build
```

### What Starts
The compose setup includes:
- API services: Auction, Search, Bidding, Notification, Identity, Gateway
- Frontend: Next.js web app
- Databases: PostgreSQL and MongoDB
- Messaging: RabbitMQ
- Reverse proxy: Nginx

### URLs for Each Service
- Web App (Next.js): http://localhost:3000
- Gateway Service (YARP): http://localhost:6001
- Identity Service: http://localhost:5001
- Auction Service: http://localhost:7001
- Search Service: http://localhost:7002
- Bidding Service: http://localhost:7003
- Notification Service: http://localhost:7004
- Nginx Proxy: http://localhost
- RabbitMQ Management UI: http://localhost:15672 (user: guest, password: guest)
- RabbitMQ AMQP: amqp://localhost:5672
- PostgreSQL: postgres://postgres:postgrespw@localhost:5432/AuctionDb
- MongoDB: mongodb://root:mongopw@localhost:27017

### Stop Services
```bash
docker compose down
```

### Useful Commands
Show running containers:

```bash
docker compose ps
```

Follow logs:

```bash
docker compose logs -f
```

For full service configuration, see docker-compose.yml and docker-compose.override.yml.

## Production with Docker Compose

### Prerequisites
- Docker Desktop (or Docker Engine + Compose plugin)
- A local hosts file mapping for production-style domains
- mkcert installed for local TLS certificates

### Configure Hostnames
Add the following entries to your hosts file.

- 127.0.0.1 app.autobid.local
- 127.0.0.1 api.autobid.local
- 127.0.0.1 id.autobid.local

Hosts file locations:
- Windows: C:\Windows\System32\drivers\etc\hosts (open editor as Administrator)
- Linux/macOS: /etc/hosts (use sudo)

### Certificates
Use mkcert to create and trust local certificates for *.autobid.local.

1. Install mkcert.

Windows (Chocolatey):
```powershell
choco install mkcert
```

macOS (Homebrew):
```bash
brew install mkcert nss
```

Linux (Ubuntu/Debian example):
```bash
sudo apt update
sudo apt install mkcert libnss3-tools
```

2. Install the local certificate authority (one-time).

```bash
mkcert -install
```

3. Generate cert and key into devcerts.

```bash
mkcert -cert-file devcerts/autobid.local.crt -key-file devcerts/autobid.local.key app.autobid.local api.autobid.local id.autobid.local
```

4. Verify the files exist before starting containers.

Current certificate files:
- devcerts/autobid.local.crt
- devcerts/autobid.local.key

### Start Production Compose Stack
Run production using the base compose file plus the production override:

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

### Production URLs
- Web App: https://app.autobid.local
- API Gateway: https://api.autobid.local
- Identity Service: https://id.autobid.local

### Internal Service Ports (optional direct access)
- Gateway Service: http://localhost:6001
- Identity Service: http://localhost:5001
- Auction Service: http://localhost:7001
- Search Service: http://localhost:7002
- Bidding Service: http://localhost:7003
- Notification Service: http://localhost:7004

### Stop Production Stack
```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml down
```

### Useful Commands
Show running containers:

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml ps
```

Follow logs:

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml logs -f
```
