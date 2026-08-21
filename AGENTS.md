# Games4Trade agent handbook

This file is the repository-level operating guide for coding agents. Read it before editing. It is intentionally explicit so that an agent with limited repository context can find the correct layer, make a small change, and validate it safely.

## Start here

1. Run `git status --short --branch` and preserve all existing user changes.
2. Identify the affected feature in the feature map below.
3. Trace the complete path before editing: Vue route/view/component -> Vuex or Axios -> API controller -> service -> repository -> EF model/DTO/mapping.
4. Make the smallest coherent change. Do not perform unrelated cleanup or large rewrites.
5. Run the checks from the validation matrix that match the files and behavior changed.
6. Finish with `git diff --check`, review `git diff`, and report both successful checks and remaining warnings.

All paths and commands below are relative to the repository root unless a section says otherwise. Quote paths under `Web Api` because the directory name contains a space.

## Non-negotiable rules

- Never discard, overwrite, reset, or reformat unrelated user changes.
- Do not commit, push, create a pull request, deploy, publish, or contact external people unless the user explicitly asks. Local installs, builds, tests, and disposable Docker validation are allowed when required by the task.
- Do not commit secrets or copy existing credentials into new files. Treat connection strings, JWT keys, passwords, tokens, `.env`, and local settings as sensitive even if a legacy value is already tracked.
- Do not add generated output: `node_modules`, `dist`, `bin`, `obj`, `test-results`, Playwright reports, logs, uploaded photos, or `*.tsbuildinfo`.
- Use the root `.editorconfig`. Avoid whole-repository formatting when the task only touches a few files.
- Use `npm`, not Yarn or pnpm. When dependencies change, update and commit the matching `package-lock.json`; never edit a lockfile manually.
- Use `Web Api/Games4Trade.slnx`. Do not recreate a `.sln` file.
- Do not edit or squash historical EF migrations. Add a new migration only for an intentional schema/model change.
- Do not weaken authentication, authorization, validation, file checks, or error handling merely to make a test pass.
- Preserve public API shapes, URLs, JWT claim handling, Vuex action/getter names, and SignalR event names unless the requested change explicitly alters the contract.
- Some legacy identifiers are misspelled, including `IAdvertisementReposiotry`, `GetAdvetisements`, `AdvetisementControllerTests`, `AnnoucementSaveValidator`, and the SignalR event `Recieve`. Treat them as contracts; do not rename them opportunistically.
- Keep user-facing text in Polish unless the task explicitly changes localization. Preserve Polish characters and UTF-8 encoding.

## Technology and runtime

- Backend: .NET 10, ASP.NET Core controllers, Entity Framework Core 10, PostgreSQL/Npgsql, FluentValidation, JWT bearer authentication, SignalR, SkiaSharp, xUnit, and Moq.
- Frontend: Vue 3, Vue Router, Vuex 4, Vite, Axios, Bootstrap, Vuelidate, SweetAlert2, Sass, and TypeScript.
- End-to-end tests: Playwright with an isolated Docker Compose project.
- Local orchestration: Docker Compose runs PostgreSQL, the API, and an nginx-hosted frontend.
- Required local tools for full validation: .NET SDK 10, Node.js/npm, and Docker Desktop. Restore the local EF CLI with `dotnet tool restore` when migrations are involved.

## Repository map

| Path | Purpose | Start here when... |
| --- | --- | --- |
| `Web Api/Games4Trade.slnx` | Solution containing API and tests | Building or testing .NET |
| `Web Api/Games4Trade/Program.cs` | Dependency injection, middleware, JWT, SignalR, startup migrations, development seed | Adding a service/repository/strategy or changing startup behavior |
| `Web Api/Games4Trade/Controllers` | HTTP routes, authorization, model-state checks, HTTP status mapping | Adding or changing an API endpoint |
| `Web Api/Games4Trade/Interfaces/Services` | Service contracts | Changing business operations |
| `Web Api/Games4Trade/Services` | Business rules and orchestration | Implementing backend behavior |
| `Web Api/Games4Trade/Interfaces/Repositories` | Persistence contracts | Changing data access behavior |
| `Web Api/Games4Trade/Repositories` | EF Core queries and persistence | Changing filtering, includes, paging, or database writes |
| `Web Api/Games4Trade/Data/ApplicationContext.cs` | EF model configuration, relationships, indexes, seeds, TPH discriminator | Changing schema or relationships |
| `Web Api/Games4Trade/Data/Migrations` | Database migration history and snapshot | Reviewing schema history; add, never rewrite |
| `Web Api/Games4Trade/Models` | EF entities | Changing persisted data |
| `Web Api/Games4Trade/Dtos` | API contracts, query options, and the legacy-located `OperationResult.cs` | Changing payloads or service results |
| `Web Api/Games4Trade/Validators` | FluentValidation request rules | Changing input validation |
| `Web Api/Games4Trade/Core/Mapping` | Explicit entity/DTO mapping extensions | Adding fields to responses or requests |
| `Web Api/Games4Trade/Core/Advertisements` | Advertisement subtype strategies and resolver | Changing Game/Console/Accessory behavior |
| `Web Api/Games4Trade/Core/Images` | Thumbnail abstraction and SkiaSharp implementation | Changing thumbnail generation |
| `Web Api/Games4TradeTests` | xUnit unit/service/repository/controller tests | Testing backend behavior |
| `Client/Games4Trade/src/main.ts` | Vue bootstrap, Axios base URL, JWT request header | Changing app-wide client setup |
| `Client/Games4Trade/src/App.vue` | Root layout, initial store loads, and auth-driven SignalR lifecycle | Changing startup UI or real-time messaging |
| `Client/Games4Trade/src/services/messageHub.ts` | Singleton SignalR connection and typed message subscriptions | Changing hub connection behavior or consuming real-time messages |
| `Client/Games4Trade/src/router` | Route definitions grouped by feature | Adding or changing browser routes |
| `Client/Games4Trade/src/views` | Route-level screens | Changing page behavior |
| `Client/Games4Trade/src/components` | Reusable and feature UI components | Changing a portion of a page |
| `Client/Games4Trade/src/store` | Non-namespaced Vuex state, getters, mutations, and actions | Changing shared state or common API calls |
| `Client/Games4Trade/src/mixins/mixins.js` | Shared legacy SweetAlert/error helpers | Changing shared dialog behavior |
| `Client/Games4Trade/.env.*` | Build-time Vite API and SignalR URLs | Changing local/production client endpoints |
| `E2E/tests` | Critical browser flows, including the two-user SignalR scenario | Extending end-to-end coverage |
| `E2E/run-e2e.mjs` | Builds/starts/cleans the isolated E2E stack | Changing E2E orchestration |
| `docker-compose.yml` and `docker-compose.override.yml` | Local application topology and ports | Changing Docker runtime behavior |

## Architecture and request flow

The normal request path is:

`Vue route/view/component -> Vuex action or direct Axios call -> /api/<controller> -> controller -> service -> repository -> ApplicationContext/PostgreSQL`

- Axios receives its base URL from `VITE_API_URL` in `src/main.ts`. API calls normally use relative paths such as `advertisements`, `users/{id}`, or `login`.
- Authentication state lives in the non-namespaced `user` Vuex module. The raw JWT is stored in `localStorage`; the Axios interceptor sends the stored `Bearer ...` value in the `Authorization` header.
- API routes use `[Route("api/[controller]")]`. Controllers are responsible for binding, `[Authorize]`/role checks, model-state validation, and translating service outcomes into HTTP responses.
- Services return `OperationResult` with `IsSuccessful`, `IsClientError`, `Message`, and `Payload`. Preserve the established distinction: client/domain failures become 4xx responses, unexpected failures become 5xx, and successful payloads become response DTOs.
- Repositories own EF queries and `SaveChangesAsync`. Services own authorization-sensitive business rules and coordination across repositories/files.
- Do not return EF entities directly from new endpoints. Add/update DTOs and mapping extensions in `Core/Mapping/MappingExtensions.cs`.
- The API applies EF migrations automatically at startup. In `Development`, it also seeds/repairs the development administrator. A missing or unhealthy database can therefore prevent the API from starting.
- SignalR is mapped at `/messagehub`. The frontend URL comes from `VITE_MESSAGE_HUB_URL`; the hub token is passed through `accessTokenFactory`. If an event name or payload changes, update the hub/client contract and `App.vue` together.

## Feature map

Use this map to avoid searching the whole repository blindly.

### Authentication and accounts

- Backend: `LoginController`, `LoginService`, `ILoginService`, `UsersController`, `UserService`, `UserRepository`, user DTOs, and user validators.
- Frontend: `src/store/modules/user.js`, `src/views/users/Login.vue`, `SignUp.vue`, `ChangePassword.vue`, `Navbar.vue`, and auth/user router files.
- Important contract: JWT claim URIs are read explicitly in the Vuex module. Do not change claim names on only one side.

### Advertisements and photos

- Backend: `AdvertisementsController`, `AdvertisementService`, `AdvertisementRepository`, advertisement DTOs/validators, `Core/Advertisements`, `Core/Mapping`, `ApplicationContext`, and photo/image classes.
- Frontend: `src/views/advertisements`, `src/components/advertisements`, advertisement routes, plus genre/system/region/state store modules.
- Advertisement items use EF TPH inheritance with discriminator values `AdvertisementItem`, `Game`, `Console`, and `Accessory`.
- Subtype creation, update, relationship validation, and DTO mapping belong in `IAdvertisementItemStrategy` implementations. Do not reintroduce discriminator switch statements into services.
- Adding a new subtype usually requires an entity, DTO fields, strategy, DI registration in `Program.cs`, EF discriminator registration, mapping, frontend form/display support, tests, and possibly a migration.
- Search page numbers are 1-based at the HTTP/frontend boundary and converted to 0-based values in `AdvertisementsController` before repository `Skip` calculations. Preserve this boundary.
- Uploaded files are stored under API-relative `photos/...` paths while metadata is stored in the database. Keep file creation/deletion and database changes consistent, clean up partial failures, and test thumbnail behavior when the first photo changes.

### Users and recommendations

- Backend: `UsersController`, `UserService`, `UserRepository`, user relationship entities/DTOs, and `AdvertisementRepository.GetRecommendedAdvertisements`.
- Frontend: `src/views/users`, `src/components/users`, the user Vuex module, and user routes.
- Liked genres, owned systems, and observed-user relationships feed advertisement recommendations; changing those relations can affect both profiles and the home/recommendation results.

### Catalog administration

- Genres: `GenresController` -> `GenreService` -> `GenreRepository`; frontend `components/admin/Genres.vue` and `store/modules/genre.js`.
- Systems: `SystemsController` -> `SystemService` -> `SystemRepository`; frontend `components/admin/Systems.vue` and `store/modules/system.js`.
- Regions and states are read-only API catalogs seeded in `ApplicationContext`; their frontend Vuex modules are loaded from `App.vue`.
- Genre/system mutations and announcement management are Admin-only. Preserve role authorization on the API even if the UI hides the controls.

### Announcements

- Backend: `AnnouncementsController`, `AnnouncementService`, `AnnouncementRepository`, announcement DTOs/validator.
- Frontend: announcement views/components, announcement routes, and `store/other/other.js`.

### Messages and real-time updates

- Backend: `MessagesController`, `MessageService`, `MessageRepository`, `MessagesHub`, and `IMessagesClient`.
- Frontend: `views/Messages.vue`, `components/messages`, the auth watcher in `App.vue`, and the singleton connection/subscription layer in `src/services/messageHub.ts`.
- Messages endpoints are authenticated. Preserve both HTTP behavior and SignalR notification behavior when changing message delivery/read state.
- The legacy event name is `Recieve`. Its payload is the persisted `MessageDto` with `id`, `content`, `isDelivered`, `dateCreated`, `senderId`, and `receiverId`; do not broadcast the smaller POST request DTO.
- Hub connection state follows the JWT getter. A user who logs in after the root component mounts must connect without reloading, and logout/unmount must stop the connection.
- `Messages.vue` refreshes conversation summaries from hub notifications. `Conversation.vue` inserts messages for the currently open sender immediately. The older HTTP polling remains a fallback, so a SignalR test must prevent polling from making a broken hub appear healthy.
- Real-time E2E coverage requires two independent Playwright browser contexts (one storage/auth session per user), an established WebSocket for both, and delivery in both directions. Keep the `isUpdate` polling route stubbed to `false` in that test.

## Backend implementation rules

- Follow the existing dependency direction: controller -> service interface/implementation -> repository interface/implementation -> EF context.
- Register every new service, repository, validator, strategy, or singleton in `Program.cs` with an appropriate lifetime.
- Keep controllers thin. Put business decisions in services and query composition in repositories.
- Validate request DTOs with FluentValidation when a validator pattern already exists; continue to handle model-state errors consistently.
- Enforce ownership and roles server-side. UI checks are not authorization.
- Use async EF/file APIs. Avoid `async void`; tests and application methods should return `Task`.
- Be explicit about nullability. Do not silence warnings with `!` unless the invariant is proven at that exact point.
- Preserve PostgreSQL behavior. EF InMemory tests do not model all PostgreSQL features such as `ILike`, relational constraints, transactions, or provider SQL.
- `Games4TradeAPI.Models.System` and `Games4TradeAPI.Models.Console` collide with BCL names. Use the existing aliases or fully qualified names where necessary.
- Mapping is explicit; AutoMapper is not used. When a DTO changes, search every `ToDto`, `ToModel`, `MapTo`, strategy, controller response, and frontend consumer.
- Existing compiler warnings are technical debt, not a baseline to expand. Do not suppress them globally or add new warnings.

## Database and migration workflow

Only use this workflow when the requested change alters the EF model or database schema:

1. Update entities and `ApplicationContext.OnModelCreating` together.
2. Restore the local tool: `dotnet tool restore`.
3. Add a migration from the repository root:
   `dotnet ef migrations add <DescriptiveName> --project 'Web Api/Games4Trade/Games4TradeAPI.csproj' --startup-project 'Web Api/Games4Trade/Games4TradeAPI.csproj'`.
4. Inspect both migration directions and `ApplicationContextModelSnapshot.cs`. Do not accept unrelated destructive operations.
5. Run the .NET tests and, when Docker is available, start against a fresh PostgreSQL database or run E2E.

Never rename old migration classes/files merely to fix spelling or style warnings.

## Frontend implementation rules

- The codebase is transitional: many legacy components/modules use JavaScript and the Options API; newer files use TypeScript and `<script setup>`. Do not rewrite a feature wholesale solely to change style.
- Prefer TypeScript for new modules/components and keep `strict` checks passing. Avoid new `any`, `@ts-ignore`, `@ts-expect-error`, or disabled checks unless a narrow comment explains an unavoidable external typing issue.
- Vuex modules are not namespaced. Before adding an action/getter/mutation, search all modules for the same name.
- Route-level pages belong in `src/views`; reusable pieces belong in `src/components`; route definitions belong in the matching file under `src/router/routes`.
- Use the `@/` alias for `src`. Do not duplicate the Axios base URL or hard-code environment-specific hosts in components.
- Keep request/response payload casing and field names aligned with the backend DTOs. Search both client and server before renaming a property.
- Keep loading-state cleanup and user-visible error handling on both success and failure paths. Existing shared dialog helpers live in `mixins/mixins.js`.
- Preserve semantic selectors used by Playwright, including accessible labels and existing `data-testid` attributes. If UI wording/selectors must change, update E2E tests in the same change.
- Vite environment values are embedded at build time and exposed only when prefixed with `VITE_`.

### TypeScript 7 compatibility contract

The dual compiler setup is deliberate and must not be simplified without rechecking current Vue/Volar support:

- `@typescript/native` aliases native TypeScript 7 and provides `tsc` for regular `.ts` checking.
- `typescript` aliases `@typescript/typescript6`; Vue's `vue-tsc` currently needs the TypeScript 6 programmatic API to check `.vue` SFCs.
- `npm run type-check` runs both `vue-tsc --noEmit` and native `tsc --noEmit`.
- `npm run build` must continue to run the full type-check before `vite build`.
- `tsconfig.json` uses `moduleResolution: "Bundler"`; TypeScript 7 does not support the old `Node`/`node10` mode or `baseUrl`.
- The explicit `vuex` type path works around Vuex 4's package export metadata. Remove it only after proving both type-checkers pass.

## Running the application

Preferred full-stack path:

1. Create a local root `.env` with `DB_USER` and `DB_PASSWORD`; never commit it.
2. Run `docker compose up --build -d`.
3. Check `docker compose ps`.
4. Default endpoints are frontend `http://localhost:80`, API `http://localhost:5000`, and PostgreSQL `localhost:5432`.
5. Stop with `docker compose down`. Only use `docker compose down -v` when intentionally deleting local database data.

Local frontend only:

```powershell
Set-Location 'Client/Games4Trade'
npm ci
npm run dev
```

Local API only requires a reachable PostgreSQL connection:

```powershell
dotnet run --project 'Web Api/Games4Trade/Games4TradeAPI.csproj'
```

Do not use development seed credentials as production examples or add more hard-coded secrets.

## Validation matrix

Run all checks that match the change. A passing narrower check does not replace a required broader check.

| Change | Required checks |
| --- | --- |
| Backend C#, DTO, validator, repository, service, controller | `dotnet test 'Web Api/Games4Trade.slnx' --configuration Release` |
| EF model/migration | Backend tests plus inspect migration/snapshot and exercise PostgreSQL when possible |
| Frontend `.js`, `.ts`, or `.vue` | From `Client/Games4Trade`: `npm run type-check` and `npm run build` |
| Frontend dependency or lockfile | From `Client/Games4Trade`: `npm ci`, `npm outdated`, `npm audit`, `npm run build` |
| E2E code/dependency | From `E2E`: `npm ci`, then the relevant Playwright command |
| API contract, auth, persistence, upload, critical UI flow, Docker | Backend + frontend checks and `npm run test:e2e` from `E2E` when Docker is available |
| Docker Compose only | `docker compose config`; start affected services when practical |
| Documentation/instructions only | Inspect rendered text/paths, `git diff --check`, and verify every documented command/path against the repository |

### End-to-end test prerequisites

- `npm run test:e2e` uses a separate Compose project with frontend `8080`, API `5001`, and a disposable PostgreSQL volume.
- It requires a local API image. Build it from the repository root with `docker compose build api` after backend changes.
- From `E2E`, run `npm ci`; install Chromium once with `npx playwright install chromium` if missing.
- The runner tears down containers, network, and volume in `finally`. If a run is interrupted, return to the repository root and clean only the named E2E project with `docker compose --project-name games4trade-e2e --file 'E2E/docker-compose.e2e.yml' down --volumes --remove-orphans`.
- `npm run test:e2e:existing` targets an already-running application and does not create the isolated stack.

## Safe change recipes

### Add or change an API field

1. Find the request/response DTO.
2. Update entity only if persistence changes.
3. Update the relevant mapping extensions; for advertisement subtype fields, update the strategy mapping too.
4. Update service logic and controller response.
5. Update every Axios/Vue consumer of the payload.
6. Add backend tests; add/update E2E if user-visible.
7. Add a migration only if the persisted model changed.

### Add or change an endpoint

1. Add the controller route with correct binding and authorization.
2. Add/update the service interface and implementation.
3. Add/update repository methods only for persistence/query work.
4. Register new implementations in `Program.cs`.
5. Add controller/service tests for success, invalid input, unauthorized/forbidden ownership, and not-found behavior as relevant.
6. Update frontend calls and validate the complete flow.

### Change a Vue page

1. Locate its route, route-level view, child components, Vuex actions/getters, and API calls.
2. Preserve loading, empty, success, validation, and error states.
3. Keep API field names and auth assumptions synchronized with the server.
4. Run both type-checkers and the production build.
5. Run/update Playwright when the critical path or selector changes.

## Known traps

- The directory `Web Api` contains a space; unquoted shell paths commonly fail.
- API startup runs migrations immediately, so a database connection problem can look like an application startup failure.
- Vuex is non-namespaced; duplicate global action/getter names silently create confusing behavior.
- Vite builds transpile TypeScript but do not replace `npm run type-check`.
- Authentication is enforced by API attributes and ownership checks, not by route visibility. There are no global Vue Router guards to rely on.
- The SignalR event name `Recieve` is intentionally preserved for compatibility despite its spelling.
- Vite embeds the hub URL during the image build. Keep `VITE_MESSAGE_HUB_URL` wired through both `Client/Games4Trade/Dockerfile` and the relevant Compose build arguments; an API URL alone is insufficient for messaging E2E.
- Advertisement subtype behavior spans EF TPH configuration, strategies, DTO mapping, frontend forms, and filters. Changing only one layer produces partial failures.
- Photo records and files are separate resources. A successful database change with failed file cleanup, or the reverse, leaves inconsistent state.
- Repository tests using EF InMemory can pass while PostgreSQL-specific behavior fails; use E2E/fresh PostgreSQL for relational query changes.
- `Dtos/OperationResult.cs` declares `Games4TradeAPI.Models.OperationResult`; search by symbol instead of assuming its folder matches its namespace.
- `appsettings.json` and `Program.cs` contain legacy local configuration patterns. Do not duplicate their sensitive values or treat them as secure production guidance.

## Definition of done

Before handing work back:

- The requested behavior is implemented across every affected layer.
- Authorization, validation, error handling, and cleanup paths were considered.
- Relevant tests/builds from the matrix pass, or an exact environmental blocker is reported.
- No new package is stale or vulnerable after dependency work.
- `git diff --check` passes and generated files are absent from `git status`.
- The final report names the changed areas, commands run and outcomes, known pre-existing warnings, and anything not verified.
