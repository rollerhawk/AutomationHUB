## 🎯 MVP-Ziel:
Ein Gerät (Industriewaage) wird als Prozess gestartet, registriert sich beim HUB-Core und sendet per REST Gewichtswerte.

## ✅ Muss-Funktionen
- Gerät startet eigenständig
- Registrierung via `GET /devices/register`
- Gewichts-POST via `POST /devices/{id}/weight`
- HUB speichert Daten im RAM
- REST-API zur Anzeige

## 🛠 Aufgaben
- #1 Erstellen MVP
- #2 Programmieren des Devices
- #3 Programmieren des HUBs

## ❌ Nicht im MVP
- Webhooks
- GUI
- Docker-Containerisierung
