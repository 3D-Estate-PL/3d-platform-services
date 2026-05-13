# Image Management – opis działania

## Przegląd

Moduł Image Management odpowiada za zarządzanie zdjęciami w platformie 3D Estate. Kluczową funkcjonalnością jest dynamiczne skalowanie (resize) zdjęć na żądanie, realizowane w architekturze asynchronicznej z użyciem kolejek (pub/sub).

## Dynamiczne skalowanie zdjęć

1. **Zgłoszenie żądania**
   - Gdy użytkownik lub inny serwis potrzebuje zdjęcia w określonej rozdzielczości, wysyłane jest żądanie resize (np. przez API lub bezpośrednio do kolejki pub/sub).

2. **Publikacja komunikatu**
   - Żądanie resize jest publikowane jako komunikat do kolejki (np. Dapr pub/sub, Azure Service Bus).
   - Komunikat zawiera m.in. identyfikator zdjęcia, docelową rozdzielczość oraz miejsce docelowe zapisu.

3. **Konsumpcja komunikatu**
   - Dedykowany handler (np. ResizeImageCommandHandler) nasłuchuje na kolejce i odbiera komunikaty resize.
   - Handler pobiera oryginalny plik zdjęcia z magazynu (np. Azure Blob Storage).

4. **Przetwarzanie i zapis**
   - Zdjęcie jest przetwarzane (skalowane) do żądanej rozdzielczości.
   - Nowa wersja zdjęcia jest zapisywana w magazynie (np. w osobnym kontenerze lub pod inną ścieżką).

5. **Powiadomienie lub dalsze przetwarzanie**
   - Po zakończeniu operacji system może opublikować kolejny event (np. o zakończonym resize), który może być konsumowany przez inne usługi.

## Zalety rozwiązania
- **Asynchroniczność** – żądania resize nie blokują użytkownika, są realizowane w tle.
- **Skalowalność** – obsługa wielu żądań równolegle dzięki kolejkom.
- **Odporność na błędy** – w przypadku awarii komunikat pozostaje w kolejce do ponownego przetworzenia.
- **Integracja z innymi usługami** – dzięki pub/sub inne mikroserwisy mogą reagować na zdarzenia związane z przetwarzaniem zdjęć.

## Przykładowy przepływ
1. Użytkownik przesyła zdjęcie w oryginalnej rozdzielczości.
2. Frontend lub inny serwis zgłasza zapotrzebowanie na miniaturę 200x200px.
3. System publikuje komunikat `ResizeImageCommand` do kolejki.
4. Handler odbiera komunikat, przetwarza zdjęcie i zapisuje miniaturę.
5. Po zakończeniu przetwarzania system może opublikować event o dostępności nowej wersji zdjęcia.

## Pliki i klasy
- `ResizeImageCommand` – definicja komunikatu żądania resize.
- `ResizeImageCommandHandler` – logika przetwarzania resize.
- Integracja z Dapr pub/sub lub Azure Service Bus.
- Przechowywanie plików: Azure Blob Storage.

---

W razie potrzeby szczegółowej dokumentacji API lub diagramu przepływu – daj znać!
