# 🧩 Workplace Tasks – Gestão de Tarefas com RBAC

Um sistema completo de **gestão de tarefas** desenvolvido em **.NET 8 (C#)** com **PostgreSQL** e **Angular 19**, implementando **autenticação JWT**, **autorização baseada em roles (RBAC)** e **políticas com AuthorizationHandler**.  
O projeto também inclui **Docker Compose** para orquestração de containers do backend, banco e frontend.

---

## 🚀 Sumário

1. [Requisitos](#-requisitos)
2. [Arquitetura do Projeto](#-arquitetura-do-projeto)
3. [Backend (API .NET)](#-backend-api-net)
4. [Frontend (Angular)](#-frontend-angular)
5. [Execução com Docker](#-execução-com-docker)
6. [Seed inicial e usuários padrão](#-seed-inicial-e-usuários-padrão)
7. [RBAC – Regras de Acesso](#-rbac--regras-de-acesso)
8. [Endpoints principais](#-endpoints-principais)
9. [Autenticação via JWT](#-autenticação-via-jwt)
10. [Estrutura de diretórios](#-estrutura-de-diretórios)

---

## 🧱 Requisitos

### 🖥️ Backend
- **.NET 8 SDK**
- **PostgreSQL 16+**
- **Docker Desktop** (para ambiente containerizado)
- **Visual Studio Code** ou **Rider**

### 💻 Frontend
- **Node.js 20+**
- **Angular CLI 19+**
- **Docker Desktop**

---

## 🏗️ Arquitetura do Projeto

```
WorkplaceTasks/
│
├── backend/
│   └── Workplace.Tasks.Api/
│       ├── Controllers/
│       ├── Services/
│       ├── Repositories/
│       ├── Data/
│       ├── Authorization/
│       ├── Middlewares/
│       ├── Validators/
│       ├── Program.cs
│       ├── appsettings.json
│       └── Dockerfile
│
├── frontend/
│   └── workplace-frontend/
│       ├── src/
│       │   ├── app/
│       │   │   ├── components/
│       │   │   ├── services/
│       │   │   ├── guards/
│       │   │   ├── models/
│       │   │   └── environments/
│       │   ├── index.html
│       │   └── main.ts
│       ├── Dockerfile
│       └── angular.json
│
└── docker-compose.yml
```

---

## ⚙️ Backend (API .NET)

### 🧩 Tecnologias
- **.NET 8 Web API**
- **Entity Framework Core 8**
- **PostgreSQL**
- **JWT Authentication**
- **FluentValidation**
- **Swagger UI (com suporte a Bearer Token)**
- **AuthorizationHandlers para RBAC**

### 🔑 Configuração JWT
Definida em `appsettings.json`:
```json
"Jwt": {
  "Key": "f2E4n9R6x8B3p1T7z5U2a0K4w6M8r3Y9"
}
```

### 🐘 Banco de dados
O banco PostgreSQL é configurado via connection string:
```
Host=postgres;Port=5432;Database=workplace;Username=postgres;Password=postgres
```

### 🧠 Inicialização
O método `DbInitializer.Seed()` cria automaticamente:
- 3 usuários padrão (Admin, Manager, Member)
- 1 tarefa associada a cada usuário

---

## 🌐 Frontend (Angular 19 Material)

### 🧰 Tecnologias
- **Angular 19**
- **Angular Material**
- **Reactive Forms**
- **Auth0 JWT Module**
- **Interceptors para Token e Erros**
- **Guards para RBAC**
- **Arquitetura Standalone**

### 🖼️ UI Features
- Navbar dinâmica com nome e role do usuário
- Botão de **logout** que limpa o token e redireciona para login
- Botão **Painel de Admin** visível apenas para administradores
- Formulário de criação/edição de tarefas com Material Design
- Lista de tarefas com botões condicionais conforme role

### ⚙️ Arquivo `environment.ts`
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:8080/api'
};
```

---

## 🐳 Execução com Docker

### 📦 1. Construir e subir containers
Na raiz do projeto:
```bash
docker-compose up --build
```

Isso irá subir:
- **PostgreSQL** (porta 5433)
- **Backend .NET** (porta 8080)
- **Frontend Angular** (porta 4200)

### 🌍 Acessos
- API: http://localhost:8080/swagger  
- Frontend: http://localhost:4200

---

## 👥 Seed inicial e usuários padrão

| Usuário             | Email               | Senha         | Role     |
|---------------------|---------------------|----------------|-----------|
| Admin User          | admin@example.com   | Password123!   | Admin     |
| Manager User        | manager@example.com | Password123!   | Manager   |
| Member User         | member@example.com  | Password123!   | Member    |

Cada usuário possui uma tarefa criada automaticamente.

---

## 🔐 RBAC – Regras de Acesso

### 👑 **Admin**
- Acesso total a todos os endpoints e recursos.  
- Pode criar, ler, atualizar e excluir **qualquer tarefa**.  
- Pode gerenciar usuários e roles.  

### 🧑‍💼 **Manager**
- Pode criar tarefas (`POST /tasks`).  
- Pode ler todas as tarefas (`GET /tasks`).  
- Pode atualizar qualquer tarefa (`PUT /tasks/{id}`).  
- Pode excluir **apenas tarefas que criou** (`DELETE /tasks/{id}`).  

### 👤 **Member**
- Pode criar tarefas (`POST /tasks`).  
- Pode ler tarefas (`GET /tasks`).  
- Pode atualizar **apenas tarefas que criou**.  
- Pode excluir **apenas tarefas que criou**.  

---

## 🧭 Endpoints principais

### 🔐 Autenticação
| Método | Rota | Descrição |
|--------|------|------------|
| `POST` | `/api/auth/token` | Login e geração do JWT |

### 📋 Tarefas
| Método | Rota | Descrição |
|--------|------|------------|
| `GET` | `/api/tasks` | Lista todas as tarefas (Admin/Manager) |
| `GET` | `/api/tasks/{id}` | Retorna detalhes de uma tarefa |
| `POST` | `/api/tasks` | Cria uma nova tarefa |
| `PUT` | `/api/tasks/{id}` | Atualiza tarefa (respeitando role) |
| `DELETE` | `/api/tasks/{id}` | Exclui tarefa (respeitando role) |

### 👤 Usuários (apenas Admin)
| Método     | Rota              | Descrição                                                                        | Permissão |
| :--------- | :---------------- | :------------------------------------------------------------------------------- | :-------- |
| `GET`      | `/api/users`      | Lista todos os usuários cadastrados no sistema.                                  | Admin     |
| `GET`      | `/api/users/{id}` | Retorna os detalhes de um usuário específico.                                    | Admin     |
|  `POST`    | `/api/users`      | Cria um novo usuário informando nome, e-mail, senha e role.                      | Admin     |
| `PUT`      | `/api/users/{id}` | Atualiza informações de um usuário existente ou altera sua role (exceto Admins). | Admin     |
| `DELETE`   | `/api/users/{id}` | Remove um usuário do sistema (exceto Admins).                                    | Admin     |

---

## 🔑 Autenticação via JWT

O token JWT é retornado no login e deve ser enviado no **header** das requisições protegidas:

```http
Authorization: Bearer <token>
```

O Swagger já está configurado com **Bearer Authentication**, permitindo testar diretamente da interface `/swagger`.

---

## 📂 Estrutura geral de execução

### Backend
```bash
cd backend/Workplace.Tasks.Api
dotnet restore
dotnet run
```

### Frontend
```bash
cd frontend/workplace-frontend
npm install
ng serve --open
```

---
