# 🎁 GiftShuffle

Aplicación de **Amigo Invisible / Secret Santa** con backend en .NET 10 y frontend en React.

---

## Tecnologías

| Capa | Tecnología |
|------|-----------|
| Backend | .NET 10, Clean Architecture |
| Frontend | React 19, Vite 8, TypeScript 6 |
| Estilos | Tailwind CSS v4 |
| Base de datos | SQLite (EF Core) |
| Autenticación | ASP.NET Core Identity + JWT |
| Emails | MailKit (SMTP Gmail) |
| Testing | xUnit, FluentAssertions, Moq |

---

## Estructura del proyecto

```
GiftShuffle/                          ← raíz del repo
├── GiftShuffleBackend/
│   ├── GiftShuffle.Domain/           → Entidades (Friend, ShuffleHistory)
│   ├── GiftShuffle.Application/      → Interfaces, DTOs, Servicios
│   ├── GiftShuffle.Infraestructure/  → EF Core, Identity, MailKit, JWT
│   ├── GiftShuffle.Api/              → Controllers, Program.cs, middleware
│   ├── GiftShuffle.Application.Tests/ → Tests unitarios
│   ├── GiftShuffle.Api.Tests/        → Tests de integración
│   └── GiftShuffle.slnx
├── GifShuffleFrontEnd/               ← frontend Vite + React
│   ├── src/
│   │   ├── api/                      → cliente Axios con interceptor JWT
│   │   ├── components/               → FriendList, FriendForm, ShufflePanel, Navbar
│   │   ├── pages/                    → LoginPage, RegisterPage, DashboardPage
│   │   └── types.ts
│   └── vite.config.ts                → proxy /api → localhost:5036
├── AGENTS.md
└── opencode.json
```

---

## Prerrequisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/)
- Cuenta de Gmail con [contraseña de aplicación](https://myaccount.google.com/apppasswords) (2FA requerida)

---

## Configuración

La configuración sensible se guarda en [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```bash
cd GiftShuffleBackend

dotnet user-secrets set "Jwt:Key" "una-clave-de-32-caracteres-o-mas" --project GiftShuffle.Api
dotnet user-secrets set "Smtp:Username" "tu-email@gmail.com" --project GiftShuffle.Api
dotnet user-secrets set "Smtp:Password" "la-contraseña-de-aplicacion" --project GiftShuffle.Api
```

| Secret | Descripción |
|--------|-------------|
| `Jwt:Key` | Clave simétrica para firmar tokens (mín. 32 caracteres) |
| `Smtp:Username` | Correo Gmail que envía las notificaciones |
| `Smtp:Password` | App Password de 16 caracteres (sin espacios) |

Los valores por defecto están en `appsettings.json` — solo el `Jwt:Key` es obligatorio para arrancar.

---

## Ejecución

### Backend (puerto 5036)

```bash
cd GiftShuffleBackend
dotnet run --project GiftShuffle.Api
```

La base de datos SQLite (`giftshuffle.db`) se crea automáticamente al iniciar.

La referencia de la API está disponible en `/scalar/v1` (solo en desarrollo).

### Frontend (puerto 5173)

```bash
cd GifShuffleFrontEnd
npm install
npm run dev
```

El frontend redirige las llamadas a `/api/*` hacia `http://localhost:5036` (configurado en `vite.config.ts`).

---

## API endpoints

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| POST | `/api/auth/register` | ❌ | Registrar nuevo usuario |
| POST | `/api/auth/login` | ❌ | Iniciar sesión (devuelve JWT) |
| GET | `/api/friends/getAll` | ✅ JWT | Listar amigos |
| GET | `/api/friends/{id}` | ✅ JWT | Obtener amigo por ID |
| POST | `/api/friends/create` | ✅ JWT | Crear amigo |
| PUT | `/api/friends/{id}` | ✅ JWT | Actualizar amigo |
| DELETE | `/api/friends/{id}` | ✅ JWT | Eliminar amigo |
| POST | `/api/shuffle` | ✅ JWT | Ejecutar sorteo |
| DELETE | `/api/shuffle/history` | ✅ JWT | Limpiar historial |

---

## Algoritmo de sorteo

1. **Fisher-Yates shuffle** + asignación circular (nadie se regala a sí mismo).
2. Excluye pares (regalador, regalado) presentes en `ShuffleHistory` para evitar repetir asignaciones.
3. Si después de 100 intentos no se encuentra una asignación válida, reintenta ignorando el historial (garantiza éxito con ≥ 2 participantes).
4. Cada par se persiste en `ShuffleHistory`.
5. Los emails se envían en paralelo con tolerancia a fallos (un error de envío no rompe el sorteo).

---

## Testing

```bash
cd GiftShuffleBackend
dotnet test GiftShuffle.slnx
```

- **36 tests**: 19 unitarios + 17 de integración.
- Los tests de integración usan SQLite en memoria.
- El envío de emails se reemplaza por un stub en los tests.

---

## Licencia

Uso privado.
