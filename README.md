# 🏋️ GimDeportivo API

API RESTful desarrollada en **C# con .NET Core** para la gestión de un gimnasio deportivo. Permite administrar socios, entrenadores, membresías y asistencias con autenticación segura mediante **JWT** y control de acceso basado en roles.

---

## 📋 Requisitos previos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) (LocalDB o instancia completa)
- [Postman](https://www.postman.com/) (para pruebas)
- [Git](https://git-scm.com/)

---

## ⚙️ Instalación de dependencias

Ejecutar los siguientes comandos en la raíz del proyecto:

```bash
# Entity Framework Core — ORM para acceso a base de datos relacional
dotnet add package Microsoft.EntityFrameworkCore --version 10.0.0

# Proveedor SQL Server para Entity Framework Core
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.0

# Herramientas de EF Core (migraciones, scaffold)
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 10.0.0

# CLI global de EF Core para comandos de migración desde terminal
dotnet tool install --global dotnet-ef

# Swagger — documentación y prueba interactiva de endpoints
dotnet add package Swashbuckle.AspNetCore --version 10.1.5

# Autenticación JWT Bearer para .NET Core
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 10.0.0
```

### ¿Para qué sirve cada paquete?

| Paquete | Función |
|---|---|
| `Microsoft.EntityFrameworkCore` | ORM principal que permite mapear clases C# a tablas de base de datos sin escribir SQL manual. |
| `Microsoft.EntityFrameworkCore.SqlServer` | Proveedor que conecta EF Core específicamente con SQL Server. |
| `Microsoft.EntityFrameworkCore.Tools` | Habilita comandos como `Scaffold-DbContext` dentro de Visual Studio. |
| `dotnet-ef` | Herramienta global de línea de comandos para crear y aplicar migraciones desde la terminal. |
| `Swashbuckle.AspNetCore` | Genera automáticamente la documentación interactiva de la API (interfaz Swagger UI). |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | Middleware que valida tokens JWT en cada petición HTTP para proteger los endpoints. |

---

## 🗄️ Configuración de base de datos

1. Abrir el archivo `appsettings.json` y verificar la cadena de conexión:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=GimDeportivo;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

2. Crear la base de datos aplicando las migraciones:

```bash
# Crear el archivo de migración
dotnet ef migrations add PrimeraMigracion

# Aplicar la migración a SQL Server (crea las tablas)
dotnet ef database update
```

---

## ▶️ Ejecución del proyecto

```bash
dotnet run
```

La API estará disponible en: `https://localhost:5001` o `http://localhost:5000`

---

## 🔐 Autenticación

La API usa **JWT (JSON Web Token)** para autenticación sin estado. Para acceder a los endpoints protegidos:

1. Realizar login en `POST /api/auth/login`
2. Copiar el token recibido
3. En Postman: `Authorization > Bearer Token` → pegar el token

### Ejemplo de login

**Request:**
```json
POST /api/auth/login
{
  "nombre": "admin",
  "password": "1234"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

## 👥 Roles del sistema

| Rol | Permisos |
|---|---|
| `ADMIN` | Acceso total: gestiona usuarios, roles, membresías y registros. |
| `ENTRENADOR` | Consulta usuarios, registra y actualiza asistencias. |
| `SOCIO` | Consulta membresías y su historial de registros. |

---

## 📡 Endpoints disponibles

### 🔑 Autenticación
| Método | Ruta | Acceso | Descripción |
|---|---|---|---|
| POST | `/api/auth/login` | Público | Inicia sesión y retorna token JWT |

### 👤 Usuarios
| Método | Ruta | Roles permitidos | Descripción |
|---|---|---|---|
| GET | `/api/usuario` | ADMIN, ENTRENADOR | Lista todos los usuarios |
| GET | `/api/usuario/{id}` | ADMIN, ENTRENADOR | Obtiene un usuario por ID |
| POST | `/api/usuario` | ADMIN | Registra un nuevo usuario |
| PUT | `/api/usuario/{id}` | ADMIN | Actualiza datos de un usuario |
| DELETE | `/api/usuario/{id}` | ADMIN | Elimina un usuario |

### 🏅 Membresías
| Método | Ruta | Roles permitidos | Descripción |
|---|---|---|---|
| GET | `/api/membresia` | ADMIN, ENTRENADOR, SOCIO | Lista todas las membresías |
| GET | `/api/membresia/{id}` | ADMIN, ENTRENADOR, SOCIO | Obtiene una membresía por ID |
| POST | `/api/membresia` | ADMIN | Crea una nueva membresía |
| PUT | `/api/membresia/{id}` | ADMIN | Actualiza una membresía |
| DELETE | `/api/membresia/{id}` | ADMIN | Elimina una membresía |

### 📋 Registros de Asistencia
| Método | Ruta | Roles permitidos | Descripción |
|---|---|---|---|
| GET | `/api/registro` | ADMIN, ENTRENADOR, SOCIO | Lista todos los registros |
| GET | `/api/registro/{id}` | ADMIN, ENTRENADOR, SOCIO | Obtiene un registro por ID |
| POST | `/api/registro` | ADMIN, ENTRENADOR | Crea un nuevo registro de asistencia |
| PUT | `/api/registro/{id}` | ADMIN, ENTRENADOR | Actualiza un registro |
| DELETE | `/api/registro/{id}` | ADMIN | Elimina un registro |

### 🔒 Roles
| Método | Ruta | Roles permitidos | Descripción |
|---|---|---|---|
| GET | `/api/Roles` | ADMIN | Lista todos los roles |
| GET | `/api/Roles/{id}` | ADMIN | Obtiene un rol por ID |
| POST | `/api/Roles` | ADMIN | Crea un nuevo rol |
| PUT | `/api/Roles/{id}` | ADMIN | Actualiza un rol |
| DELETE | `/api/Roles/{id}` | ADMIN | Elimina un rol |

---

## 🗂️ Estructura del proyecto

```
gimDeportivo/
├── Controllers/
│   ├── AuthController.cs
│   ├── MembresiaController.cs
│   ├── RegistroController.cs
│   ├── RolesController.cs
│   └── UsuarioController.cs
├── Data/
│   └── AppDbContext.cs
├── Models/
│   ├── Membresia.cs
│   ├── Registro.cs
│   ├── Roles.cs
│   └── Usuario.cs
├── Migrations/
├── appsettings.json
├── Program.cs
└── gimDeportivo.csproj
```

---

## 🧪 Pruebas con Postman

1. Importar la colección de Postman incluida en el repositorio.
2. Hacer login para obtener el token JWT.
3. Configurar la variable de entorno `{{token}}` con el valor recibido.
4. Ejecutar las peticiones de cada módulo.

---

## 📦 Repositorio

🔗 (([https://github.com/ArturoU5/ExmFinal.git]))



---

