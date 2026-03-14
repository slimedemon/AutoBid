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

The Next.js app in frontend/web-app reads environment variables for auth and API endpoints.

### For Local Dev (npm run dev)
Create frontend/web-app/.env.local from the checked-in example file.

Windows (PowerShell):
```powershell
Copy-Item frontend/web-app/.example.env frontend/web-app/.env
```

macOS/Linux:
```bash
cp frontend/web-app/.example.env frontend/web-app/.env
```

Required in frontend/web-app/.env:
- AUTH_SECRET
- API_URL
- ID_URL
- AUTH_URL
- NEXT_PUBLIC_NOTIFY_URL

### For Docker Compose
If you run the web app via Docker Compose, variables are already provided for the web-app container in compose files. You usually do not need a local .env.local for the containerized app.

### Important Notes
- Replace AUTH_SECRET with your own secure value for real deployments.
- Do not commit real secrets. frontend/web-app/.gitignore ignores .env* by default.

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

## Local Production with Docker Compose

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

## Kubernetes with Local Cluster

This repository includes Kubernetes manifests for running the full AutoBid stack on a local cluster with ingress and TLS.

### Prerequisites
- A local Kubernetes cluster that can run Linux containers
- kubectl configured to talk to that cluster
- Docker for building the application images
- mkcert installed for local TLS certificates

Supported local cluster options:
- Docker Desktop Kubernetes
- Minikube
- kind

Important: the application deployments in infra/K8S use imagePullPolicy: Never. Build the images locally first, then make sure your cluster can see them. Docker Desktop may share the local image store directly. Minikube and kind usually require an explicit image load step.

### Configure Hostnames
The ingress manifest exposes three local domains:

- app.autobid.local
- api.autobid.local
- id.autobid.local

Add them to your hosts file.

For Docker Desktop Kubernetes, map them to localhost:

- 127.0.0.1 app.autobid.local
- 127.0.0.1 api.autobid.local
- 127.0.0.1 id.autobid.local

For Minikube or another cluster where the ingress controller gets a different external IP, replace 127.0.0.1 with that IP.

Hosts file locations:
- Windows: C:\Windows\System32\drivers\etc\hosts
- Linux/macOS: /etc/hosts

### Create Local TLS Certificates
Install and trust the local certificate authority once:

```bash
mkcert -install
```

Generate a certificate that matches the ingress hosts:

```bash
mkcert -cert-file devcerts/autobid.local.crt -key-file devcerts/autobid.local.key app.autobid.local api.autobid.local id.autobid.local
```

Create the Kubernetes TLS secret used by the ingress:

```bash
kubectl create secret tls autobid-app-tls --cert=devcerts/autobid.local.crt --key=devcerts/autobid.local.key
```

If the secret already exists, recreate it with:

```bash
kubectl delete secret autobid-app-tls
kubectl create secret tls autobid-app-tls --cert=devcerts/autobid.local.crt --key=devcerts/autobid.local.key
```

### Build the Application Images
Build the images with the same tags referenced by the Kubernetes manifests:

```bash
docker compose build auction-svc search-svc bid-svc notify-svc identity-svc gateway-svc web-app
```

If your cluster does not automatically see locally built images, load them explicitly.

Minikube:

```bash
minikube image load slimedemon/auction-svc slimedemon/search-svc slimedemon/bid-svc slimedemon/notify-svc slimedemon/identity-svc slimedemon/gateway-svc slimedemon/web-app
```

kind:

```bash
kind load docker-image slimedemon/auction-svc slimedemon/search-svc slimedemon/bid-svc slimedemon/notify-svc slimedemon/identity-svc slimedemon/gateway-svc slimedemon/web-app
```

### Deploy the Ingress Controller
Apply the bundled ingress-nginx manifests first:

```bash
kubectl apply -f infra/ingress/ingress-depl.yml
kubectl rollout status deployment/ingress-nginx-controller -n ingress-nginx
```

If you are using Minikube, run the tunnel in a separate terminal so the LoadBalancer service gets an external IP:

```bash
minikube tunnel
```

### Deploy AutoBid Manifests
Apply storage, infrastructure services, configuration, application services, and the ingress resource:

```bash
kubectl apply -f infra/K8S/local-pvc.yml
kubectl apply -f infra/K8S/postgres-depl.yml
kubectl apply -f infra/K8S/mongo-depl.yml
kubectl apply -f infra/K8S/rabbit-depl.yml
kubectl apply -f infra/K8S/config.yml
kubectl apply -f infra/K8S/auction-depl.yml
kubectl apply -f infra/K8S/search-depl.yml
kubectl apply -f infra/K8S/bid-depl.yml
kubectl apply -f infra/K8S/notify-depl.yml
kubectl apply -f infra/K8S/identity-depl.yml
kubectl apply -f infra/K8S/gateway-depl.yml
kubectl apply -f infra/K8S/webapp-depl.yml
kubectl apply -f infra/K8S/ingress-svc.yml
```

### Verify the Deployment
Check pods and services:

```bash
kubectl get pods
kubectl get svc
kubectl get ingress
kubectl get svc -n ingress-nginx
```

Open the local ingress endpoints:

- Web App: https://app.autobid.local
- API Gateway: https://api.autobid.local
- Identity Service: https://id.autobid.local

Optional direct database and management access exposed by NodePort:

- PostgreSQL: localhost:30001
- RabbitMQ Management: http://localhost:30002
- MongoDB: localhost:30003

### Useful Commands
Show deployment status:

```bash
kubectl get deployments
kubectl get pods -o wide
```

Follow logs for one service:

```bash
kubectl logs -f deployment/gateway-svc
```

Restart a deployment after rebuilding an image:

```bash
kubectl rollout restart deployment/auction-svc
```

### Stop and Clean Up
Delete the AutoBid resources:

```bash
kubectl delete -f infra/K8S/ingress-svc.yml
kubectl delete -f infra/K8S/webapp-depl.yml
kubectl delete -f infra/K8S/gateway-depl.yml
kubectl delete -f infra/K8S/identity-depl.yml
kubectl delete -f infra/K8S/notify-depl.yml
kubectl delete -f infra/K8S/bid-depl.yml
kubectl delete -f infra/K8S/search-depl.yml
kubectl delete -f infra/K8S/auction-depl.yml
kubectl delete -f infra/K8S/config.yml
kubectl delete -f infra/K8S/rabbit-depl.yml
kubectl delete -f infra/K8S/mongo-depl.yml
kubectl delete -f infra/K8S/postgres-depl.yml
kubectl delete -f infra/K8S/local-pvc.yml
kubectl delete secret autobid-app-tls
```

Delete the ingress controller when you no longer need it:

```bash
kubectl delete -f infra/ingress/ingress-depl.yml
```
