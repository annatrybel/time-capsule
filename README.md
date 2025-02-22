# pipsi1
Projekt z projektowania i programowania systemów internetowych I

## Temat:
Kapsuła Czasu

## Opis:
Użytkownik ma możliwość utworzenia własnych, spersonalizowanych listów, które mogą zawierać tekst, odpowiedzi na predefiniowane pytania, zdjęcia, nagrania głosowe, oraz filmy, a następnie zablokować odczyt wiadomości na wybrany przez siebie czas, po którym będzie możliwe ponowne odczytanie, a także udostępnianie zawartości innym użytkownikom.

## Lista członków grupy:
- Krzysztof Sułkowski
- Anna Trybel
- Aneta Walczak

## Język programowania:
C#

## Uruchomienie projektu przy użyciu Makefile

W projekcie dostępny jest plik `makefile`, który zawiera przydatne komendy do uruchomienia poszczególnych elementów projektu oraz obsługi migracji bazy danych.

### Wymagania dla systemu Windows

1. **Instalacja Chocolatey**
   Aby korzystać z poleceń make, najpierw należy zainstalować menedżera pakietów Chocolatey. Otwórz PowerShell jako administrator i wykonaj poniższe polecenie:
   ```powershell
   Set-ExecutionPolicy Bypass -Scope Process -Force; `
   [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; `
   iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
   ```

2. **Instalacja make**
   Po zainstalowaniu Chocolatey, zainstaluj narzędzie `make` przez wykonanie poniższego polecenia (uruchomionego jako administrator):
   ```powershell
   choco install make
   ```

3. **Używanie komend z Makefile**
   W terminalu przejdź do katalogu, w którym znajduje się plik makefile (np. TimeCapsule/TimeCapsule), następnie możesz korzystać z komend:
   
  - `make run-db`  
     Uruchamia tylko kontener bazy danych (`db`) w trybie detached.
   
  - `make run-containers`  
     Uruchamia oba kontenery (`db` oraz `timecapsule`) jednocześnie w trybie detached.

  - `make run-app`
     Uruchamia oba kontenery (`db` oraz `timecapsule`), a następnie uruchamia aplikację.
   
  - `make migrate-add NAME=TwojaNazwaMigracji`  
     Dodaje nową migrację Entity Framework Core o podanej nazwie (np. `NAME=InitialCreate`). Upewnij się, że nazwa migracji jest podana.
   
  - `make migrate-update`  
     Aktualizuje bazę danych, stosując wszystkie oczekujące migracje.