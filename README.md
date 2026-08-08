# Games4Trade

## 1. Jak to odpalić

Najprostszy sposób to Docker Compose. Wymagane są Docker Desktop z uruchomionym silnikiem (na Windows także WSL 2) oraz wolne porty `80`, `5000` i `5432`.

1. W głównym katalogu repozytorium ustaw poświadczenia PostgreSQL w `.env`:

   ```env
   DB_USER=postgres
   # Jeżeli hasło zawiera $, ujmij je w pojedyncze cudzysłowy.
   DB_PASSWORD='<silne_haslo>'
   ```

2. Zbuduj i uruchom usługi:

   ```powershell
   docker compose up --build -d
   ```

3. Sprawdź stan i otwórz aplikację:

   ```powershell
   docker compose ps
   ```

   - frontend: http://localhost:80
   - API: http://localhost:5000
   - PostgreSQL: `localhost:5432`

API czeka na zdrowy kontener PostgreSQL i automatycznie wykonuje migracje. Aby zatrzymać środowisko, użyj `docker compose down`; aby usunąć również dane deweloperskiej bazy, użyj `docker compose down -v`.

Jeżeli Docker zwraca błąd `mcr.microsoft.com/...: EOF`, połączenie z Microsoft Container Registry zostało przerwane. Ponów `docker compose up --build -d` po chwili lub zrestartuj Docker Desktop.

Opcjonalnie frontend można uruchomić lokalnie po wejściu do `Client/Games4Trade` i wykonaniu `npm ci`, a następnie `npm run dev`. Backend wymaga .NET SDK 10 oraz działającego PostgreSQL; uruchomisz go z katalogu głównego przez:

```powershell
dotnet run --project 'Web Api/Games4Trade/Games4TradeAPI.csproj'
```

## 2. Passy dla admina

W środowisku `Development` aplikacja tworzy konto administratora przy pierwszym uruchomieniu bazy:

| Pole | Wartość |
| --- | --- |
| Login | `admin` |
| Hasło | `Admin123!` |
| E-mail | `admin@games4trade.pl` |

To konto jest wyłącznie do uruchamiania lokalnego i testów. Nie używaj tych danych w środowisku produkcyjnym. Stare domyślne konto deweloperskie jest automatycznie aktualizowane do tych danych, o ile jego hasło nie zostało wcześniej zmienione.

## 3. Jak odpalić testy e2e

Testy Playwright sprawdzają pełny przepływ w Chromium: logowanie administratora i utworzenie gatunku/systemu, a następnie rejestrację użytkownika, logowanie oraz dodanie ogłoszenia.

Najpierw uruchom aplikację przez Docker Compose z pierwszego rozdziału. Następnie w drugim terminalu wykonaj:

```powershell
Set-Location E2E
npm ci
npx playwright install chromium
npm run test:e2e
```

`npx playwright install chromium` pobiera przeglądarkę tylko przy pierwszym uruchomieniu albo po zmianie wersji Playwright. Testy domyślnie używają aplikacji pod `http://localhost:80`; inny adres można podać przez zmienną `E2E_BASE_URL`:

```powershell
$env:E2E_BASE_URL = 'http://localhost:8080'
npm run test:e2e
```

Do interaktywnego uruchamiania użyj `npm run test:e2e:ui`. Testy tworzą własne, oznaczone dane w lokalnej bazie; można je wyczyścić komendą `docker compose down -v`.
