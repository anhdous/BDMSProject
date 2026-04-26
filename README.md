# 🩸 Blood Donation Management System

A full-stack web application for managing blood donors, appointments, and blood inventory.  
Built with ASP.NET Core MVC and deployed to the cloud using Docker.

---

## 🚀 Live Demo
👉 https://bdms-app.onrender.com

---

## 📌 Features

- Manage donor records and medical history  
- Schedule and manage donation appointments  
- Track blood inventory and availability  
- Handle blood requests with prioritization logic  
- Full CRUD operations with validation  
- Responsive UI using Razor Views and Bootstrap  

---

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core MVC  
- **Frontend:** Razor Views, Bootstrap  
- **Database:** SQL Server  
- **Data Access:** Dapper  
- **Architecture:** Repository Pattern, Dependency Injection  
- **DevOps:** Docker, Render (Cloud Deployment)  

---

## 🧠 Key Highlights

- Implemented business logic for blood allocation and request prioritization  
- Optimized SQL queries for improved performance  
- Designed a clean layered architecture (ApplicationCore, Infrastructure, UI)  
- Containerized application using Docker for consistent deployment  
- Configured environment variables for secure production setup  

---

## ⚙️ Environment Variables

To run this project locally or in production, configure the following environment variable:
```bash
ConnectionStrings__Default=connection_string_here 
```
---

## **🐳 Docker Setup**
```bash
docker build -t bdms-app .
```
```bash
docker run -p 10000:10000 bdms-app
```

---

**📂 Project Structure**

BDMSApp/
├── ApplicationCore/     # Business logic and domain models  
├── Infrastructure/      # Data access (Dapper, repositories)  
├── BDMSApp/             # UI layer (Controllers, Views)  
├── Dockerfile  
└── BDMSApp.sln

---

**👩‍💻 Author**

Anh Do
