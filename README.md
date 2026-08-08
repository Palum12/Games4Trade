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

Test uruchamia własne, odizolowane środowisko Docker: frontend na porcie `8080`, API na `5001` i osobną bazę PostgreSQL. Baza jest tworzona od nowa przed każdym uruchomieniem oraz usuwana razem z wolumenem po zakończeniu testu — również po niepowodzeniu. Nie trzeba uruchamiać aplikacji z pierwszego rozdziału; może ona działać równolegle, a jej dane nie zostaną zmienione.

Przy pierwszym uruchomieniu potrzebny jest lokalny obraz API. Zbuduj go raz w katalogu głównym repozytorium (po zmianie backendu powtórz tę komendę):

```powershell
docker compose build api
```

Skrypt E2E tworzy własny obraz frontendu dla portu API `5001`; nie modyfikuje obrazu ani kontenerów zwykłej aplikacji.

Przy pierwszym uruchomieniu przygotuj zależności i przeglądarkę:

```powershell
Set-Location E2E
npm ci
npx playwright install chromium
```

Następnie uruchom testy:

```powershell
npm run test:e2e
```

`npx playwright install chromium` pobiera przeglądarkę tylko przy pierwszym uruchomieniu albo po zmianie wersji Playwright. Do interaktywnego uruchamiania użyj `npm run test:e2e:ui` — środowisko E2E zostanie usunięte po zamknięciu interfejsu Playwright.

Jeśli celowo chcesz uruchomić testy przeciwko już działającej aplikacji, użyj `npm run test:e2e:existing` i opcjonalnie podaj jej adres przez zmienną `E2E_BASE_URL`:

```powershell
$env:E2E_BASE_URL = 'http://localhost:80'
npm run test:e2e:existing
```
