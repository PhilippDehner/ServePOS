# ServePOS

ServePOS ist ein Kassensystem fuer Vereinsfeste in Hallen mit getrennten
Ausgabestellen, zum Beispiel Kueche und Getraenkeausgabe. Es unterstuetzt
mehrere zeitlich getrennte Veranstaltungen. Pro Zeitpunkt ist genau eine
Veranstaltung aktiv.

Dieses Dokument definiert den fachlichen Ausgangspunkt des Projekts. Es ist
bewusst die verbindliche Referenz fuer weitere Architektur- und
Implementierungsentscheidungen.

## Ziel und Umfang

Bei einer aktiven Veranstaltung nimmt Bedienung/Kasse Bestellungen fuer einen
Tisch entgegen, kassiert direkt bar und druckt die erforderlichen Bons. Die
Ausgabestellen arbeiten mit Bons; austeilende Personen markieren einzelne
Positionen am Tablet bei der Abholung als ausgeteilt.

Nicht Teil des ersten Umfangs sind Kartenzahlungen, Wertmarken, ein
Kassenabschluss oder die separate Ausweisung der Umsatzsteuer. Wertmarken sind
als spaetere Zahlungsart vorgesehen.

## Teilnehmer und Rollen

Alle Benutzer melden sich mit einem persoenlichen Benutzernamen und PIN an.
Jeder angelegte Benutzer darf mit seiner Rolle an allen Veranstaltungen
arbeiten. Austeilende Personen verwenden den Login der jeweiligen
Ausgabestelle und erhalten keine eigene Rolle.

| Rolle | Verantwortlichkeiten |
| --- | --- |
| Admin | Veranstaltungen aktivieren und abschliessen; Benutzer, Tische, Artikeltypen, Ausgabestellen und Artikel verwalten; Preise aendern; Anfangsbestand setzen; Auswertungen einsehen. |
| Bedienung/Kasse | Bestellungen erfassen, Bargeld kassieren, Wechselgeld berechnen, Bons drucken und Nachdrucke ausloesen; bezahlte Positionen mit Stornogrund stornieren. |
| Kueche | Eigene Bestandsmengen waehrend einer Veranstaltung korrigieren und den Ausgabelogin am Tablet verwenden. |
| Getraenkeausgabe | Eigene Bestandsmengen waehrend einer Veranstaltung korrigieren und den Ausgabelogin am Tablet verwenden. |

Bestellungen, Ausgaben, Stornierungen, Bestandskorrekturen und Bon-Nachdrucke
werden jeweils mit Benutzer, Zeitpunkt und verwendeter Geraeteinstanz
protokolliert.

## Veranstaltungen

- Es kann mehrere Veranstaltungen geben, jedoch nie gleichzeitig.
- Der Admin setzt genau eine Veranstaltung als aktiv. Alle Benutzer arbeiten
  nach dem Login automatisch in dieser aktiven Veranstaltung.
- Eine abgeschlossene Veranstaltung ist schreibgeschuetzt. Ihre Auswertungen
  bleiben abrufbar.
- Artikelangebot, Preise, Tische, Bestandsmengen und Auswertungen gehoeren zu
  einer Veranstaltung.

## Artikel, Typen und Bestand

Der Admin verwaltet Artikel mit Name, Bruttopreis in Euro, Artikeltyp und
Anfangsbestand. Artikeltypen sind frei konfigurierbar. Jeder Typ ist genau
einer Ausgabestelle zugeordnet, etwa `Essen` zur `Kueche` oder `Getraenke` zur
`Getraenkeausgabe`.

Der Bestand wird beim erfolgreichen Kassieren einer Bestellung automatisch
reduziert. Kueche und Getraenkeausgabe duerfen ihn waehrend einer Veranstaltung
korrigieren. Preise darf nur der Admin aendern; bereits bestellte Positionen
behalten immer ihren beim Kauf gespeicherten Preis.

Bei einer Stornierung einer noch nicht ausgeteilten Position kann die
berechtigte Person entscheiden, ob deren Bestand wieder erhoeht wird. Bei
Offline-Bestellungen kann der Bestand nach der Synchronisation kurzfristig
negativ sein; er wird dann manuell korrigiert.

## Bestellungen und Zahlung

Jede Bestellung muss einem Tisch zugeordnet sein. Bedienelemente bieten die vom
Admin fuer die Veranstaltung gepflegten Tischnummern an, erlauben bei Bedarf
aber auch eine neue manuelle Eingabe, zum Beispiel `A1`.

Eine Bestellung kann beliebig viele Artikel und Artikeltypen enthalten. Fuer
denselben Tisch duerfen beliebig viele getrennte, sofort bezahlte Bestellungen
existieren; jede bekommt eine eigene fortlaufende Bestellnummer.

Im ersten Umfang gilt:

1. Bedienung/Kasse waehlt oder erfasst einen Tisch und fuegt Positionen hinzu.
2. Die Zahlung erfolgt ausschliesslich bar.
3. Der gegebene Betrag wird eingegeben; ServePOS berechnet das Wechselgeld.
4. Nach erfolgreicher Zahlung ist die Bestellung verbindlich und der Bestand
   wird reduziert.
5. Die erforderlichen Bons werden gedruckt.

Bedienung/Kasse und Admin duerfen bezahlte Positionen mit einem verpflichtenden
Stornogrund stornieren. Eine Stornierung erzeugt automatisch eine
Rueckerstattung in den Auswertungen.

## Bons und Ausgabe

Bons enthalten die Bestellnummer und Tischnummer, damit austeilende Personen
die Position richtig zuordnen koennen.

- Jede einzelne Speisenposition erzeugt einen eigenen Kuechenbon, der auf den
  Teller gelegt werden kann.
- Je Bestellung wird ein Bon mit allen Getraenkepositionen fuer die
  Getraenkeausgabe gedruckt.
- Bei Druckproblemen darf Bedienung/Kasse einen Bon erneut drucken. Der
  Nachdruck wird sichtbar als solcher gekennzeichnet und protokolliert.
- Es gibt keinen Status `bereit`. Eine Position ist entweder noch nicht
  ausgeteilt oder wurde bei der Abholung am Tablet als `ausgeteilt` markiert.
- Die Ausgabe wird fuer jede Position einzeln erfasst. Damit koennen Speisen
  und Getraenke derselben Bestellung unabhaengig ausgegeben werden.

## Offline-Betrieb

Jedes Tablet muss bei einem Netzwerkausfall weiter Bestellungen erfassen und
spaeter synchronisieren koennen. Offline erfasste und kassierte Bestellungen
bleiben auch bei Bestandskonflikten gueltig. Bei der Synchronisation kann der
Bestand deshalb negativ werden; eine Bestandskorrektur erfolgt anschliessend
manuell durch Kueche oder Getraenkeausgabe.

Die konkrete technische Umsetzung muss lokale Datenspeicherung,
Synchronisationswarteschlangen, eindeutige IDs und eine nachvollziehbare
Konfliktprotokollierung vorsehen.

## Auswertungen

Der Admin benoetigt pro Veranstaltung mindestens:

- Tagesumsatz
- Anzahl und Details der Bestellungen
- Stornierungen einschliesslich Rueckerstattungsbetrag und Stornogrund
- Verkaeufe je Artikel

## Technischer Ausgangspunkt

Das Frontend liegt in `Frontend` und verwendet React, TypeScript und Vite. Das
Backend liegt in `Backend/ServePOS.API` und verwendet ASP.NET Core auf .NET 10.
PostgreSQL ist als Datenbank festgelegt, aber noch nicht integriert.

Weitere Informationen zum aktuellen Codebestand, lokalen Start und den
vorhandenen Beispielendpunkten stehen in
[REPOSITORY_CONTEXT.md](REPOSITORY_CONTEXT.md).

## Authentication configuration

Before starting the API for the first time, configure these environment variables.
The bootstrap credentials are consumed only while no user exists:

```text
Jwt__Issuer=ServePOS
Jwt__Audience=ServePOS
Jwt__SigningKey=<at-least-32-random-characters>
BootstrapAdmin__Username=admin
BootstrapAdmin__Pin=<at-least-4-digits>
```

## Lokaler Betrieb

1. Lege im Repository eine `.env`-Datei an und setze mindestens
   `Jwt__SigningKey` auf einen zufälligen Wert mit mindestens 32 Zeichen sowie
   `BootstrapAdmin__Username` und `BootstrapAdmin__Pin`. Docker Compose leitet
   diese Werte als Konfiguration an die API weiter. `Jwt__Issuer` und
   `Jwt__Audience` sind optional und verwenden standardmäßig `ServePOS`.
2. Starte den vollständigen Stack:

   ```bash
   docker compose up --build
   ```

   Die Anwendung ist anschließend unter `http://localhost:5173` erreichbar;
   die API läuft auf `http://localhost:8080`. Der erste konfigurierte
   Administrator wird beim Start angelegt.
3. Beende den Stack mit `docker compose down`. Mit `-v` werden zusätzlich die
   lokalen Datenbankvolumes entfernt. PostgreSQL 18 speichert Daten im
   `database_data`-Volume unterhalb von `/var/lib/postgresql`; ein Upgrade von
   älteren PostgreSQL-Major-Versionen erfordert deshalb eine Datenmigration.

## Lokale Entwicklung

Starte PostgreSQL über `docker compose up database`, dann die API aus
`Backend` mit `dotnet run --project ServePOS.API` und das Frontend aus
`Frontend` mit `npm ci && npm run dev`.

Für Vertragsänderungen muss die API laufen, bevor der Client aktualisiert wird:

```bash
cd Frontend
npm run update-api-client
```

Die App kann in aktuellen Browsern über das Installationssymbol als PWA
installiert werden. Offline erfasste Bestellungen werden lokal zwischengespeichert
und beim nächsten Online-Ereignis synchronisiert.
