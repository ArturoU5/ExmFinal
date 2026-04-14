# 🏋️ Gimnasio API

API RESTful para la gestión integral de un gimnasio, desarrollada con **ASP.NET Core** y **Entity Framework Core**. Permite administrar socios, entrenadores, rutinas, ejercicios, membresías y asistencias, con autenticación JWT y control de acceso por roles.

---

## 📋 Tabla de Contenidos

- [Tecnologías](#tecnologías)
- [Arquitectura](#arquitectura)
- [Requisitos previos](#requisitos-previos)
- [Instalación y configuración](#instalación-y-configuración)
- [Roles y permisos](#roles-y-permisos)
- [Autenticación](#autenticación)
- [Endpoints documentados](#endpoints-documentados)
- [Validaciones y manejo de errores](#validaciones-y-manejo-de-errores)

---

## 🛠 Tecnologías

| Tecnología | Versión | Uso |
|---|---|---|
| .NET | 10.0 | Framework principal |
| ASP.NET Core | 10.0 | Web API |
| Entity Framework Core | 10.0 | ORM / acceso a datos |
| SQL Server | - | Base de datos |
| JWT (JwtBearer) | 10.0 | Autenticación |
| Swagger / Swashbuckle | 10.1.5 | Documentación de API |

---

## 🏗 Arquitectura

El proyecto sigue una arquitectura en capas dentro de un único proyecto ASP.NET Core Web API:

```
ExmFinal/
├── Controllers/          # Controladores de la API (endpoints HTTP)
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── RolesController.cs
│   ├── UserRolesController.cs
│   ├── SocioController.cs
│   ├── EntrenadorController.cs
│   ├── MembresiaController.cs
│   ├── SocioMembresiaController.cs
│   ├── RutinaController.cs
│   ├── RutinaEjercicioController.cs
│   ├── EjercicioController.cs
│   └── AsistenciaController.cs
├── Models/               # Entidades del dominio (modelos EF Core)
│   ├── Users.cs
│   ├── Roles.cs
│   ├── UserRoles.cs
│   ├── Socios.cs
│   ├── Entrenadores.cs
│   ├── Membresias.cs
│   ├── SocioMembresia.cs
│   ├── Rutinas.cs
│   ├── RutinaEjercicios.cs
│   ├── Ejercicios.cs
│   ├── Asistencias.cs
│   └── Login.cs
├── Data/
│   └── AppDbContext.cs   # Contexto de EF Core
├── Program.cs            # Configuración de servicios y middleware
├── appsettings.json      # Configuración (cadena de conexión, JWT key)
└── Gimnasio.csproj       # Definición del proyecto y dependencias
```

**Flujo de una solicitud:**

```
Cliente HTTP
    └──► [Middleware: Authentication / Authorization]
              └──► Controller
                        └──► AppDbContext (EF Core)
                                  └──► SQL Server
```

---

## ✅ Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/) (local o remoto)
- [Git](https://git-scm.com/)

---

## 🚀 Instalación y configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/ArturoU5/ExmFinal.git
cd ExmFinal
```

### 2. Configurar la base de datos y JWT

Editar el archivo `appsettings.json` con tu cadena de conexión y una clave JWT segura:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GimnasioDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "TuClaveSecretaDe32CaracteresComoMínimo2024!"
  }
}
```

> ⚠️ **Importante:** La clave JWT debe tener al menos 32 caracteres para HMAC-SHA256.

### 3. Restaurar dependencias

```bash
dotnet restore
```

### 4. Aplicar migraciones y crear la base de datos

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> Si `dotnet ef` no está instalado, instálalo con:
> ```bash
> dotnet tool install --global dotnet-ef
> ```

### 5. Ejecutar la aplicación

```bash
dotnet run
```

La API estará disponible en:

```
https://localhost:5027
```

La documentación Swagger estará en:

```
http://localhost:5000/swagger
```

---

## 👥 Roles y permisos

El sistema maneja tres roles. El acceso a cada endpoint depende del rol incluido en el token JWT.

| Rol | Descripción |
|---|---|
| `ADMIN` | Acceso completo a todos los endpoints |
| `ENTRENADOR` | Gestión de rutinas, ejercicios y asistencias. Ve su propio perfil |
| `SOCIO` | Solo lectura de su propio perfil, sus rutinas y sus membresías |

### Matriz de permisos por módulo

| Módulo | ADMIN | ENTRENADOR | SOCIO |
|---|:---:|:---:|:---:|
| Usuarios | ✅ CRUD | ❌ | ❌ |
| Roles | ✅ CRUD | ❌ | ❌ |
| Asignación de roles | ✅ CRUD | ❌ | ❌ |
| Socios (listado) | ✅ | ✅ | ❌ |
| Socios (perfil propio) | ✅ CRUD | ✅ (solo lectura) | ✅ (solo el suyo) |
| Entrenadores (listado) | ✅ | ❌ | ❌ |
| Entrenadores (perfil propio) | ✅ CRUD | ✅ (solo el suyo) | ❌ |
| Membresías | ✅ CRUD | ✅ (lectura) | ✅ (lectura) |
| Socio-Membresía | ✅ CRUD | ❌ | ✅ (solo la suya) |
| Rutinas (listado) | ✅ | ✅ | ❌ |
| Rutinas (detalle) | ✅ | ✅ | ✅ (solo la suya) |
| Ejercicios | ✅ CRUD | ✅ (lectura) | ✅ (lectura) |
| Rutina-Ejercicio | ✅ CRUD | ✅ CRUD | ✅ (lectura) |
| Asistencias | ✅ CRUD | ✅ (lectura + crear/editar) | ✅ (lectura) |

---

## 🔐 Autenticación

### `POST /api/auth/login`

Genera un token JWT para acceder a los endpoints protegidos. El token tiene una vigencia de **2 horas**.

**Request body:**

```json
{
  "nombre": "admin",
  "password": "tu_password_hash"
}
```

**Respuesta exitosa `200 OK`:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Respuesta fallida `401 Unauthorized`:**

```json
"Credenciales inválidas"
```

**Uso del token en solicitudes posteriores:**

Incluir el token en el encabezado `Authorization` de cada request:

```
Authorization: Bearer <token>
```

---

## 📡 Endpoints documentados

### 👤 Usuarios — `/api/users`

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/users` | Listar todos los usuarios activos | ADMIN |
| GET | `/api/users/{id}` | Obtener un usuario por ID | ADMIN |
| POST | `/api/users` | Crear un nuevo usuario | ADMIN |
| PUT | `/api/users/{id}` | Actualizar un usuario | ADMIN |
| DELETE | `/api/users/{id}` | Eliminar un usuario | ADMIN |

**Body para POST/PUT:**

```json
{
  "userName": "johndoe",
  "normalizedUserName": "JOHNDOE",
  "email": "john@example.com",
  "normalizedEmail": "JOHN@EXAMPLE.COM",
  "passwordHash": "hash_de_la_contraseña",
  "phoneNumber": "999999999",
  "isActive": true
}
```

---

### 🏷 Roles — `/api/roles`

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/roles` | Listar todos los roles activos | ADMIN |
| GET | `/api/roles/{id}` | Obtener un rol por ID | ADMIN |
| POST | `/api/roles` | Crear un nuevo rol | ADMIN |
| PUT | `/api/roles/{id}` | Actualizar un rol | ADMIN |
| DELETE | `/api/roles/{id}` | Eliminar un rol | ADMIN |

**Body para POST/PUT:**

```json
{
  "name": "ENTRENADOR",
  "normalizedName": "ENTRENADOR",
  "isActive": true
}
```

---

### 🔗 Asignación de roles — `/api/user-roles`

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/user-roles/{userId}` | Ver roles de un usuario | ADMIN |
| POST | `/api/user-roles` | Asignar un rol a un usuario | ADMIN |
| DELETE | `/api/user-roles/{userId}/{roleId}` | Revocar un rol | ADMIN |

**Body para POST:**

```json
{
  "userId": 1,
  "roleId": 2
}
```

---

### 🧑 Socios — `/api/socio`

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/socio` | Listar todos los socios activos | ADMIN, ENTRENADOR |
| GET | `/api/socio/{id}` | Ver perfil de un socio | ADMIN, ENTRENADOR, SOCIO* |
| POST | `/api/socio` | Registrar un nuevo socio | ADMIN |
| PUT | `/api/socio/{id}` | Actualizar datos del socio | ADMIN, SOCIO* |
| DELETE | `/api/socio/{id}` | Eliminar un socio | ADMIN |

> *SOCIO solo puede ver/editar su propio perfil. No puede modificar `isActive`.

**Body para POST/PUT:**

```json
{
  "userId": 1,
  "fechaNacimiento": "1990-05-15",
  "genero": "M",
  "alturaCm": 175.50,
  "pesoKg": 70.00,
  "emergenciaNombre": "María Pérez",
  "emergenciaTelefono": "987654321",
  "isActive": true
}
```

---

### 🏃 Entrenadores — `/api/entrenador`

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/entrenador` | Listar todos los entrenadores activos | ADMIN |
| GET | `/api/entrenador/{id}` | Ver perfil de un entrenador | ADMIN, ENTRENADOR* |
| POST | `/api/entrenador` | Registrar un entrenador | ADMIN |
| PUT | `/api/entrenador/{id}` | Actualizar un entrenador | ADMIN |
| DELETE | `/api/entrenador/{id}` | Eliminar un entrenador | ADMIN |

> *ENTRENADOR solo puede ver su propio perfil.

**Body para POST/PUT:**

```json
{
  "userId": 2,
  "especialidad": "Musculación",
  "certificaciones": "NSCA-CPT, CrossFit Level 1",
  "fechaIngreso": "2024-01-15",
  "isActive": true
}
```

---

### 🎫 Membresías — `/api/membresia`

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/membresia` | Listar membresías activas | ADMIN, ENTRENADOR, SOCIO |
| GET | `/api/membresia/{id}` | Ver una membresía | ADMIN, ENTRENADOR, SOCIO |
| POST | `/api/membresia` | Crear una membresía | ADMIN |
| PUT | `/api/membresia/{id}` | Actualizar una membresía | ADMIN |
| DELETE | `/api/membresia/{id}` | Eliminar una membresía | ADMIN |

**Body para POST/PUT:**

```json
{
  "nombre": "Membresía Mensual",
  "descripcion": "Acceso completo al gimnasio por 30 días",
  "duracionDias": 30,
  "precio": 99.90,
  "esRenovable": true,
  "isActive": true
}
```

---

### 🔗 Socio-Membresía — `/api/socio-membresia`

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/socio-membresia/{socioId}` | Ver membresías de un socio | ADMIN, SOCIO* |
| GET | `/api/socio-membresia/{socioId}/{socioMembresiaId}` | Ver membresía específica | ADMIN, SOCIO* |
| POST | `/api/socio-membresia` | Asignar membresía a un socio | ADMIN |
| PUT | `/api/socio-membresia/{socioMembresiaId}` | Actualizar membresía del socio | ADMIN |
| DELETE | `/api/socio-membresia/{socioMembresiaId}` | Eliminar membresía del socio | ADMIN |

> *SOCIO solo puede consultar sus propias membresías.

**Body para POST:**

```json
{
  "socioId": 1,
  "membresiaId": 2,
  "fechaInicio": "2024-01-01",
  "fechaFin": "2024-01-31",
  "estado": "ACTIVA",
  "montoPagado": 99.90,
  "notas": "Primer mes"
}
```

---

### 📋 Rutinas — `/api/rutina`

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/rutina` | Listar todas las rutinas activas | ADMIN, ENTRENADOR |
| GET | `/api/rutina/{id}` | Ver una rutina | ADMIN, ENTRENADOR, SOCIO* |
| POST | `/api/rutina` | Crear una rutina | ADMIN, ENTRENADOR |
| PUT | `/api/rutina/{id}` | Actualizar una rutina | ADMIN, ENTRENADOR |
| DELETE | `/api/rutina/{id}` | Eliminar una rutina | ADMIN, ENTRENADOR |

> *SOCIO solo puede ver las rutinas que le pertenecen.

**Body para POST/PUT:**

```json
{
  "socioId": 1,
  "entrenadorId": 1,
  "nombre": "Rutina de Fuerza",
  "objetivo": "Aumentar masa muscular en tren superior",
  "fechaInicio": "2024-01-15",
  "fechaFin": "2024-03-15",
  "activa": true
}
```

---

### 💪 Ejercicios — `/api/ejercicio`

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/ejercicio` | Listar ejercicios activos | ADMIN, ENTRENADOR, SOCIO |
| GET | `/api/ejercicio/{id}` | Ver un ejercicio | ADMIN, ENTRENADOR, SOCIO |
| POST | `/api/ejercicio` | Crear un ejercicio | ADMIN |
| PUT | `/api/ejercicio/{id}` | Actualizar un ejercicio | ADMIN |
| DELETE | `/api/ejercicio/{id}` | Eliminar un ejercicio | ADMIN |

**Body para POST/PUT:**

```json
{
  "nombre": "Press de Banca",
  "descripcion": "Ejercicio de empuje para pecho con barra",
  "grupoMuscular": "Pecho",
  "isActive": true
}
```

---

### 🔀 Rutina-Ejercicio — `/api/rutina-ejercicio`

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/rutina-ejercicio/{rutinaId}` | Ver ejercicios de una rutina | ADMIN, ENTRENADOR, SOCIO |
| POST | `/api/rutina-ejercicio` | Agregar ejercicio a rutina | ADMIN, ENTRENADOR |
| PUT | `/api/rutina-ejercicio/{rutinaId}/{ejercicioId}` | Actualizar ejercicio en rutina | ADMIN, ENTRENADOR |
| DELETE | `/api/rutina-ejercicio/{rutinaId}/{ejercicioId}` | Quitar ejercicio de rutina | ADMIN, ENTRENADOR |

**Body para POST/PUT:**

```json
{
  "rutinaId": 1,
  "ejercicioId": 3,
  "orden": 1,
  "series": 4,
  "repeticiones": 10,
  "pesoObjetivoKg": 60.00,
  "duracionSegundos": null,
  "descansoSegundos": 90,
  "notas": "Controlar la fase excéntrica"
}
```

---

### 📅 Asistencias — `/api/asistencia`

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/asistencia` | Listar todas las asistencias | ADMIN, ENTRENADOR, SOCIO |
| GET | `/api/asistencia/{id}` | Ver una asistencia | ADMIN, ENTRENADOR, SOCIO |
| POST | `/api/asistencia` | Registrar asistencia | ADMIN, ENTRENADOR |
| PUT | `/api/asistencia/{id}` | Actualizar asistencia | ADMIN, ENTRENADOR |
| DELETE | `/api/asistencia/{id}` | Eliminar asistencia | ADMIN |

**Body para POST/PUT:**

```json
{
  "socioId": 1,
  "fechaHoraEntrada": "2024-01-15T08:00:00",
  "fechaHoraSalida": "2024-01-15T10:00:00",
  "observaciones": "Entrenamiento completado sin novedades",
  "registradaPorUserId": 2
}
```

---

## ⚠️ Validaciones y manejo de errores

### Validaciones de modelo (Data Annotations)

Todos los modelos aplican validaciones automáticas mediante `[Required]`, `[MaxLength]`, y `[Precision]`. Si el cuerpo del request no las cumple, la API retorna automáticamente `400 Bad Request` con los detalles del error.

### Validaciones de negocio por módulo

**Usuarios**
- `UserName` y `Email` deben ser únicos en el sistema.

**Roles**
- `Name` del rol debe ser único.

**Asignación de roles**
- El usuario y el rol deben existir.
- Un usuario no puede tener el mismo rol asignado dos veces.

**Socios**
- El `UserId` referenciado debe existir.
- Un usuario no puede estar registrado como socio más de una vez.

**Entrenadores**
- El `UserId` referenciado debe existir.
- Un usuario no puede estar registrado como entrenador más de una vez.

**Membresías**
- `Precio` debe ser mayor a `0`.
- `DuracionDias` debe ser mayor a `0`.

**Socio-Membresía**
- El `SocioId` y el `MembresiaId` deben existir.
- `FechaFin` debe ser posterior a `FechaInicio`.
- `MontoPagado` no puede ser negativo.

**Rutinas**
- El `SocioId` y el `EntrenadorId` deben existir.
- Si se proporciona `FechaFin`, debe ser posterior a `FechaInicio`.

**Rutina-Ejercicio**
- La `RutinaId` y la `EjercicioId` deben existir.

**Asistencias**
- El `SocioId` y el `RegistradaPorUserId` deben existir.
- `FechaHoraSalida` debe ser posterior a `FechaHoraEntrada`.

### Códigos de respuesta HTTP

| Código | Significado | Cuándo ocurre |
|---|---|---|
| `200 OK` | Éxito | GET o PUT exitoso |
| `201 Created` | Recurso creado | POST exitoso |
| `204 No Content` | Eliminado | DELETE exitoso |
| `400 Bad Request` | Error de validación | Datos inválidos o reglas de negocio |
| `401 Unauthorized` | No autenticado | Token ausente o inválido |
| `403 Forbidden` | Sin permiso | Rol insuficiente o recurso ajeno |
| `404 Not Found` | No encontrado | ID inexistente |

### Formato de errores de negocio

Los errores de validación de negocio retornan un JSON con la siguiente estructura:

```json
{
  "mensaje": "Error de validación",
  "detalle": "La fecha y hora de salida debe ser posterior a la de entrada."
}
```

Los errores de base de datos retornan:

```json
{
  "mensaje": "Error al registrar la asistencia",
  "detalle": "Descripción técnica del error..."
}
```
