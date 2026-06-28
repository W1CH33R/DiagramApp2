# Generator diagramów

Prosta aplikacja desktopowa (WinForms, C#) do tworzenia diagramów słupkowych i liniowych.

## Funkcje
- Edycja danych w tabeli (etykieta + wartość)
- Wybór typu diagramu: słupkowy / liniowy
- Automatyczny zapis i odczyt stanu z dysku (plik `diagram_state.json` obok pliku `.exe`) — po zamknięciu i ponownym otwarciu programu dane wracają.

## Wymagania
- .NET 6.0 SDK lub nowszy
- Windows (WinForms)

## Jak uruchomić
1. Otwórz `DiagramApp.csproj` w Visual Studio lub JetBrains Rider.
2. Uruchom projekt (F5 / Shift+F10).

## Struktura projektu
- `Program.cs` – punkt wejścia
- `ChartProject.cs` – modele danych (punkt diagramu, typ diagramu)
- `ProjectStorage.cs` – zapis/odczyt stanu w formacie JSON
- `MainForm.cs` – interfejs i logika rysowania diagramu
