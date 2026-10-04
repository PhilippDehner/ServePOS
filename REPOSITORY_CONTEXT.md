# ServePOS Repository Context

Dieses Dokument ist der schnelle Einstieg für neue Chats und Mitwirkende. Es
beschreibt den **tatsächlich vorhandenen** Stand des Repositories, nicht die
geplante Produktfunktionalitaet.

## Projektzweck

ServePOS soll eine Anwendung zum Servieren von Getraenken und Speisen werden.
Der aktuelle Stand ist ein frisches Full-Stack-Grundgeruest ohne implementierte
POS-Fachlogik.

## Struktur

```text
/
|- Backend/
|  |- ServePOS.API.slnx              # .NET-Solution
|  `- ServePOS.API/                  # ASP.NET-Core-Web-API
`- Frontend/                         # React-/Vite-Anwendung
```

| Bereich | Technologie | Einstiegspunkt | Aktueller Status |
| --- | --- | --- | --- |
| Frontend | React 19, TypeScript, Vite 7 | `Frontend/src/main.tsx`, `Frontend/src/App.tsx` | Vite-Standardansicht mit Zaehler |
| Backend | ASP.NET Core auf .NET 10 | `Backend/ServePOS.API/Program.cs` | Standard-API mit WeatherForecast-Beispiel |
| Container | Docker | `Backend/ServePOS.API/Dockerfile` | Mehrstufiger Produktions-Build fuer die API |

## Lokal starten

Das Frontend und Backend sind getrennte Anwendungen. Es gibt aktuell keinen
Proxy, keine CORS-Konfiguration und keine Client-API-Anbindung.

```powershell
# Frontend (Vite-Entwicklungsserver)
Set-Location Frontend
npm ci
npm run dev

# Backend (in einem zweiten Terminal)
Set-Location Backend\ServePOS.API
dotnet run
```

Die HTTP-Entwicklungskonfiguration der API verwendet standardmaessig
`http://localhost:5289`; HTTPS ist unter `https://localhost:7220` konfiguriert.

## Vorhandene Schnittstellen

| Methode | Route | Beschreibung |
| --- | --- | --- |
| `GET` | `/weatherforecast` | Liefert fuenf zufaellige Beispiel-Wettervorhersagen |

OpenAPI wird nur in der Entwicklungsumgebung registriert. Es gibt derzeit
keine fachlichen Endpunkte, implementierte Datenbankanbindung,
Authentifizierung, Autorisierung oder persistente Konfiguration. PostgreSQL ist
als Datenbank festgelegt; das Datenmodell und die Anbindung stehen noch aus.

## Wichtige Dateien

| Pfad | Zweck |
| --- | --- |
| `Frontend/package.json` | Frontend-Skripte und Abhaengigkeiten |
| `Frontend/vite.config.ts` | Vite-Konfiguration; nur React-Plugin, kein Backend-Proxy |
| `Frontend/eslint.config.js` | ESLint-Regeln fuer TypeScript und React |
| `Backend/ServePOS.API/Program.cs` | Service- und HTTP-Pipeline der API |
| `Backend/ServePOS.API/Properties/launchSettings.json` | Lokale API-Profile und Ports |
| `Backend/ServePOS.API/ServePOS.API.http` | Beispielaufruf fuer den WeatherForecast-Endpunkt |
| `Backend/ServePOS.API/Dockerfile` | Mehrstufiger Docker-Build auf .NET-10-Images |

## Verifikation

```powershell
# Frontend
Set-Location Frontend
npm run lint
npm run build

# Backend
Set-Location Backend\ServePOS.API
dotnet build
```

Es sind aktuell keine automatisierten Tests im Repository vorhanden.

## Leitplanken fuer weitere Arbeit

- Bestehende Beispielkomponenten und der WeatherForecast-Endpunkt sind
  Startcode; sie sind keine Produktanforderungen.
- Neue Frontend-zu-Backend-Kommunikation braucht eine bewusste Entscheidung
  fuer Basis-URL, CORS und Entwicklungs-Proxy.
- PostgreSQL ist die festgelegte Datenbank. Bei der ersten persistierenden
  Fachfunktion sollten Entity Framework Core, der PostgreSQL-Provider und
  versionierte Migrationen eingerichtet werden.
- Die API verwendet bereits Controller (`AddControllers` und
  `MapControllers`); neue HTTP-Endpunkte sollten diesem Muster folgen, sofern
  keine Architekturentscheidung es aendert.
- Secrets gehoeren in User Secrets oder eine lokale, nicht versionierte
  Konfiguration; die Projektdatei enthaelt bereits eine User-Secrets-ID.

## Kontext fuer einen neuen Chat

Bei Aufgaben zu diesem Repository kann folgender Ausgangskontext verwendet
werden:

> ServePOS ist ein noch unimplementiertes Full-Stack-Grundgeruest. Das
> Frontend liegt in `Frontend` und verwendet React 19, TypeScript und Vite 7.
> Das Backend liegt in `Backend/ServePOS.API` und ist eine ASP.NET-Core-.NET-10
> Web API mit Controllern. Es existiert nur der Beispielendpunkt
> `GET /weatherforecast`; es gibt noch keine Datenbankanbindung,
> Authentifizierung, fachlichen Modelle, API-Anbindung im Frontend oder
> automatisierten Tests.
> Beruecksichtige bei neuen Features die getrennten Anwendungen sowie die
> noch fehlende CORS-/Proxy-Konfiguration. PostgreSQL ist als Datenbank
> entschieden, aber noch nicht in die Anwendung integriert.
