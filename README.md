# 🧩 MongoBackupBackend

**MongoBackupBackend** is a cross-platform .NET 10 backend service for automated MongoDB backups and backup status monitoring through a web interface and OpenAPI (Scalar UI).

---

## 🚀 Features

* Supports both **local** and **remote** MongoDB connections
* Scheduled automatic backups
* Web interface (Swagger / Scalar OpenAPI) for monitoring and manual backup triggers
* Works in **Docker** on **Linux** and **Windows**
* Easily configured using `appsettings.json` or environment variables

---

## ⚙️ Configuration

Example **appsettings.json**:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MongoDB": {
    "LocalConnectionString": "mongodb://user:pass@ip:port/",
    "RemoteConnectionStrings": "mongodb://user:pass@ip:port/|server2|server3|..."
  },
  "Settings": {
    "Time": "00:00:00" //server time zone
  }
}
```

---

## 🐳 Docker

### 🧱 Example docker-compose.yml

```yaml
services:
  mongo-backup-backend:
    image: ghcr.io/rafael1209/mongobackupbackend:master
    container_name: mongo-backup-backend
    restart: always
    expose:
      - "8080"
    environment:
      MongoDB:LocalConnectionString: "mongodb://mongodb:27017/"
      MongoDB:RemoteConnectionStrings: "mongodb://user:pass@ip:port/|mongodb://user:pass@ip:port/|server3|..."
      Settings:Time: "03:00"
    networks:
      - app-network
      - mongo-network

  mongodb:
    image: mongo:latest
    container_name: mongodb
    restart: always
    ports:
      - "27017:27017"
    volumes:
      - ./mongo-data:/data/db
    networks:
      - mongo-network

  nginx:
    image: nginx:alpine
    container_name: nginx
    restart: always
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx-conf:/etc/nginx/conf.d
    networks:
      - app-network

networks:
  app-network:
    driver: bridge
  mongo-network:
    driver: bridge
```

---

## 🌐 API Access

Once the container is running, the API is available at:

```
http://localhost:8080
```

OpenAPI (Scalar UI) documentation is available at:

```
http://localhost:8080/scalar/v1
```

---

## 🧪 Test the API

Example request:

```bash
curl http://localhost:8080/api/v1/ping
```

Response:

```json
"pong"
```

---

## 🧰 Running Locally (without Docker)

```bash
dotnet restore
dotnet run
```

The API will be available at:

```
http://localhost:5114/openapi
```

---

## 📦 Build Docker Image Manually

```bash
docker build -t mongobackupbackend .
docker run -p 8080:8080 mongobackupbackend
```

---

## 📅 Backup Schedule

The `Settings:Time` parameter (or `Settings__Time` environment variable) defines the daily backup time in `HH:mm:ss` format.

Example:

```json
"Settings": {
  "Time": "02:00:00"
}
```

## 🧑‍💻 Author

Developed by **Rafael Chasman**
GitHub: [@rafael1209](https://github.com/rafael1209)

---

## 📄 License

MIT © Rafael Chasman

---
