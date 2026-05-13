# Image Management – opis działania

## Przykłady kodu

### 1. Publikacja żądania resize (API)

Wywołanie endpointu HTTP POST:

```csharp
[HttpPost("resize/async", Name = nameof(ResizeImageRequest))]
public async Task<IActionResult> ResizeImageRequest([FromBody] ResizeImageRequested command)
{   
   await _bus.PublishAsync(command);
   return Accepted();
}
```
Plik: [ImagesController.cs](../src/Services/ImagesManagement/ImagesManagement.Api/Controllers/ImagesController.cs)

Przykładowy payload:
```json
{
  "storageKey": "images",
  "imageName": "photo.jpg",
  "width": 200,
  "height": 200,
  "compression": 90
}
```

Definicja eventu:
```csharp
[TopicName("resize-image-requested")]
public record ResizeImageRequested : IntegrationEvent
{
   public string StorageKey { get; set; }
   public string ImageName{ get; set; }
   public int? Width { get; set; }
   public int? Height { get; set; }
   public int? MaxRes { get; set; }
   public int? Compression { get; set; }
   public string? DestinationFileName { get; set; }
}
```
Plik: [ResizeImageCommand.cs](../src/Services/ImagesManagement/ImagesManagement.Application/IntegrationEvents/ResizeImageCommand.cs)

### 2. Konsumpcja żądania (handler)

```csharp
public class ResizeImageCommandHandler : IntegrationEventHandler<ResizeImageRequested>
{
   protected override async Task HandleEvent(ResizeImageRequested request, CancellationToken cancellationToken)
   {
      var command = new ResizeImageCommand(request.StorageKey, request.ImageName, request.Width,
         request.Height, request.MaxRes, fileEncoder, request.Compression ?? 90, destinationFileName, false);
      await _mediator.Send(command, cancellationToken);
   }
}
```
Plik: [ResizeImageCommandHandler.cs](../src/Services/ImagesManagement/ImagesManagement.Application/IntegrationEvents/ResizeImageCommandHandler.cs)

### 3. Przetwarzanie polecenia resize

```csharp
public class ResizeImageCommand : ICommand<ResizeImageCommandResponse>
{
   // ...właściwości i konstruktor...
}

internal class GetImageQueryQueryHandler : ICommandHandler<ResizeImageCommand, ResizeImageCommandResponse>
{
   public async Task<ResizeImageCommandResponse> Handle(ResizeImageCommand request, CancellationToken cancellationToken)
   {
      // Pobranie pliku, resize, zapis, zwrot URL
   }
}
```
Plik: [ResizeImageCommand.cs](../src/Services/ImagesManagement/ImagesManagement.Application/Images/ResizeImageCommand.cs)

---

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
