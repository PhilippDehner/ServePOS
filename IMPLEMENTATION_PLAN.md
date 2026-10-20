# ServePOS Implementation Plan

Dieses Dokument teilt die Umsetzung in voneinander abgrenzbare Arbeitspakete
auf. Jede Session arbeitet nur an ihrem Paket, aktualisiert den Status und
erstellt einen eigenen, thematisch geschlossenen Commit. Änderungen anderer
Pakete werden weder gestaged noch in eigene Commits aufgenommen.

## Regeln für parallele Arbeit

- Vor dem Bearbeiten: `git status --short` prüfen und fremde Änderungen
  unberührt lassen.
- Nur die in einem Arbeitspaket aufgeführten Dateien und neue, dazugehörige
  Dateien ändern.
- Vor dem Commit nur konkrete Pfade mit `git add -- <pfade>` stagen und mit
  `git diff --cached --name-only` prüfen.
- API-Vertragsänderungen werden zuerst im Backend umgesetzt; anschließend wird
  der OpenAPI-Client neu generiert.
- Datenbankänderungen enthalten immer Entity-Konfiguration, Migration und
  aktualisierten EF-Model-Snapshot.

## Arbeitsübersicht

| ID | Arbeitspaket | Abhängigkeiten | Status |
| --- | --- | --- | --- |
| WP-01 | PWA und Offline-Synchronisierung | WP-02 für idempotente Übertragung | abgeschlossen |
| WP-02 | Idempotente Bestell-API | keine | abgeschlossen |
| WP-03 | Veranstaltungen und Rollen | keine | offen |
| WP-04 | Kassiervorgang und Bestand | WP-03 | offen |
| WP-05 | Bons und Ausgabestellen | WP-03, WP-04 | offen |
| WP-06 | Storno und Audit-Protokoll | WP-03, WP-04 | offen |
| WP-07 | Auswertungen und Veranstaltungsabschluss | WP-03, WP-04, WP-06 | offen |
| WP-08 | Tests, CI und Betriebsdokumentation | alle fachlichen Arbeitspakete | offen |

## WP-01: PWA und Offline-Synchronisierung

**Ziel:** Die React-Anwendung ist installierbar, lädt die App-Shell offline und
legt Bestellungen bei fehlender Verbindung lokal ab.

**Scope:** `Frontend/` einschließlich Vite-PWA-Konfiguration, Service Worker,
Installationshinweis, IndexedDB-Warteschlange und Bedienansicht.

**Ergebnis:**

- Eine Bestellung erhält beim Anlegen eine `clientOrderId`.
- Fehlgeschlagene oder Offline-Übertragungen bleiben in IndexedDB erhalten.
- Die Warteschlange wird beim Start und beim `online`-Ereignis übertragen.
- Die Bedienansicht zeigt den Synchronisierungsstatus.

**Abnahme:** Frontend-Build ist erfolgreich. Offline angelegte Bestellungen
werden nach Wiederherstellung der Verbindung genau einmal übertragen.

## WP-02: Idempotente Bestell-API

**Ziel:** Wiederholte Übertragungen derselben Offline-Bestellung dürfen keine
Doppelbestellung anlegen.

**Scope:** `Backend/ServePos.Domain/Entities/Order.cs`,
`Backend/ServePos.Application/Dtos/Order/`,
`Backend/ServePos.Application/Repositoryies/IOrderRepository.cs`,
`Backend/ServePos.Application/Services/OrderService.cs`,
`Backend/ServePos.Infrastructure/` und die API-Projektdatei.

**Ergebnis:**

- `OrderInformation` verlangt `clientOrderId`.
- `Orders.ClientOrderId` ist nullable für Bestandsdaten, aber für neue
  Bestellungen gesetzt.
- Ein gefilterter Unique-Index sichert die UUID in PostgreSQL ab.
- Wiederholungen sind sowohl bei sequenziellen als auch parallelen Requests
  erfolgreich und legen nur einen Datensatz an.

**Abnahme:** Backend-Build und
`dotnet ef migrations has-pending-model-changes` sind erfolgreich.

## WP-03: Veranstaltungen und Rollen

**Ziel:** Fachliche Daten werden einer Veranstaltung zugeordnet; Benutzer
melden sich per Benutzername und PIN an und handeln innerhalb ihrer Rolle.

**Scope:** Neue Domain-Entitäten und Migrationen für `Event`, `User`, `Role`,
`Table`, `ServingStation` und frei konfigurierbare `MenuItemType`. Neue
Controller, DTOs und Frontend-Administrationsansichten.

**Ergebnis:**

- Genau eine Veranstaltung kann aktiv sein.
- Abgeschlossene Veranstaltungen sind schreibgeschützt.
- Rollen sind mindestens Admin, Bedienung/Kasse, Küche und Getränkeausgabe.
- Artikeltypen sind einer Ausgabestelle zugeordnet.

**Abnahme:** Rollen und der aktive Veranstaltungsbezug werden serverseitig an
jedem schreibenden Endpunkt durchgesetzt.

## WP-04: Kassiervorgang und Bestand

**Ziel:** Bedienungen erfassen Bestellungen für einen Tisch und kassieren bar.

**Scope:** Bestellung, Zahlung, Preis-Snapshot, Bestandsbewegungen und
Bedienoberfläche.

**Ergebnis:**

- Tisch wird aus der Verwaltung gewählt oder manuell angegeben.
- Gegebener Betrag und Wechselgeld werden berechnet.
- Erst die erfolgreiche Barzahlung macht eine Bestellung verbindlich.
- Beim Kassieren werden Bestände reduziert; der Preis je Position bleibt
  unveränderlich gespeichert.

**Abnahme:** Eine bezahlte Bestellung kann nicht erneut kassiert werden und
verändert den Bestand genau einmal.

## WP-05: Bons und Ausgabestellen

**Ziel:** Küche und Getränkeausgabe erhalten die richtigen Bons und können
Positionen einzeln als ausgegeben markieren.

**Scope:** Bon-Domänenmodell, Druckadapter-Schnittstelle, Nachdruck-Audit und
Ausgabeansichten.

**Ergebnis:**

- Jede Speisenposition erzeugt einen separaten Küchenbon.
- Getränke einer Bestellung erscheinen gemeinsam auf einem Getränkebon.
- Nachdrucke sind gekennzeichnet und protokolliert.
- Es gibt keinen Status `bereit`; Positionen sind offen oder ausgegeben.

**Abnahme:** Eine Speisen- und eine Getränkeposition derselben Bestellung
können unabhängig ausgegeben werden.

## WP-06: Storno und Audit-Protokoll

**Ziel:** Bezahlt erfasste Positionen können nachvollziehbar storniert werden.

**Scope:** Storno- und Rückerstattungsmodell, Bestandsrückgabe, Audit-Ereignisse
und UI.

**Ergebnis:**

- Admin und Bedienung/Kasse benötigen einen verpflichtenden Stornogrund.
- Für nicht ausgegebene Positionen kann der Bestand wieder erhöht werden.
- Bestellungen, Ausgaben, Stornos, Bestandskorrekturen und Nachdrucke speichern
  Benutzer, Zeitpunkt und Geräteinstanz.

**Abnahme:** Stornos erscheinen als Rückerstattung in den Auswertungsdaten.

## WP-07: Auswertungen und Veranstaltungsabschluss

**Ziel:** Admins können eine Veranstaltung abschließen und deren Kennzahlen
einsehen.

**Scope:** Reporting-Endpunkte und Admin-Ansichten.

**Ergebnis:**

- Tagesumsatz, Bestellungsdetails, Stornos und Verkäufe je Artikel sind je
  Veranstaltung abrufbar.
- Der Abschluss sperrt alle schreibenden Fachvorgänge für die Veranstaltung.

**Abnahme:** Eine abgeschlossene Veranstaltung liefert Auswertungen, lässt aber
keine neuen oder veränderten Bestellungen zu.

## WP-08: Tests, CI und Betriebsdokumentation

**Ziel:** Die Anwendung ist reproduzierbar baubar, testbar und lokal betreibbar.

**Scope:** Unit-, Integrations- und End-to-End-Tests, GitHub Actions und
Dokumentation.

**Ergebnis:**

- Tests decken Zahlung, Bestand, Storno und Offline-Wiederholung ab.
- CI baut Frontend und Backend und führt Tests aus.
- README beschreibt `docker compose up --build`, PWA-Installation und lokale
  Entwicklungsabläufe.

**Abnahme:** Ein sauberer Checkout kann mit dokumentierten Schritten gebaut,
getestet und als Container-Stack gestartet werden.
