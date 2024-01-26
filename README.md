![Coders-Lab-1920px-no-background](https://user-images.githubusercontent.com/30623667/104709387-2b7ac180-571f-11eb-9b94-517aa6d501c9.png)

> Jest to gotowa - wzorcowa wersja warsztatu TaskManager z Dapper.

## Wstęp do warsztatu TaskManager z Dapper

Celem dzisiejszego warsztatu jest rozbudowa aplikacji TaskManager. Jest to aplikacja do zarządzania i planowania zadań do wykonania. Aplikacja będzie składała się z pięciu części:
- logiki biznesowej (modele),
- aplikacji (serwis),
- bazy danych,
- testów,
- interfejsu użytkownika (w postaci konsoli).

Podczas warsztatów rozbudujesz projekt o nowe funkcjonalności oraz integrację z bazą danych przy pomocy paczki NuGet `Dapper`.

Pamiętaj, że programista to nie jest "zwykły klepacz kodu" tylko pracujący kreatywnie rzemieślnik i inżynier dbający o logikę działania aplikacji oraz jej całą konstrukcję. Aby dobrze wykonywać swoją pracę należy używać dobrych praktyk progamistycznych oraz projektowych. Podział projektu na warstwy ze względu na odpowiedzialność kodu jest jedną z takich praktyk. Współcześnie projektuje się tzw. aplikacje N-wartstwowe.

Z tego powodu dodatkowym celem warsztatów jest podział aplikacji na warstwy wg odpowiedzialności. Docelowo aplikacja na koniec warsztatów będzie składała się z czterech warstw:
- warstwy logiki biznesowej (modele),
- warstwy aplikacji (serwis),
- warstwy prezentacji (konsola),
- warstwy dostępu do danych (repozytorium).

Idea działania jest następująca:
- użytkownik korzysta z warstwy prezentacji (konsola),
- program konsolowy korzysta z warstwy aplikacji (serwis),
- serwis korzysta z warstwy logiki biznesowej (modeli) i dostępu do danych (repozytorium),
- logika biznesowa w modelach jest główną częścią "biznesu",
- repozytorium korzysta z bazy danych do przechowywania stanu model biznesowych.

Dzięki modularnej budowie aplikacja staje się niezależna od swoich modułów/klocków. Wyobraź sobie sytuację, w której należy wymienić bazę danych na inną. Wówczas nie musisz zmieniać całej aplikacji. Wystarczy przepiąć moduł dostępu do danych, a pozostałe warstwy korzystające z niego nawet tego nie zauważą :) Co więcej w dalszej części kursu możesz wykorzystać obecny projekt to tego, aby użyć innej warstwy prezentacji danych i zamiast konsoli komunikować się poprzez API, które potem będzie komunikowało się ze stroną internetową.

Dzięki sprytnemu zabiegowi i podziału kodu na mniejsze części programista jest gotowy na bezgraniczny rozwój aplikacji. Dokonywanie modyfikacji w aplikacji będzie łatwiejsze, a co najważniejsze, będzie można dzielić się pracą z innymi programistami tak, aby nie mieszać swoich wersji kodu.

W kolejnych artykułach dowiesz się jakie są wymogi działania aplikacji.

### Czego nauczysz się podczas tego warsztatu?

Warsztat jest w formie wykonania jednego dużego zadania jakim jest rozbudowa projektu o integrację z bazą danych. Nauczysz się jak rozwijać swoją aplikację, porządkować ją i rozbudowywać o kolejne moduły i funkcje. Na pewno da to duży zastrzyk praktycznej wiedzy i pozwala na szybsze i bardziej pewne poruszanie się po narzędziach deweloperskich i kodzie C#.

Nauczysz się podczas warsztatów, że rozbudowa aplikacji wymaga czasami dostosowania jej działania do nowych wymogów i potrzeb, a to wiąże się z procesem tzw. refaktoru kodu. Dzięki temu poczujesz namiastkę pracy programisty w firmie.

W projekcie tym użyjesz praktycznie wszystkich rzeczy, o których mówiliśmy podczas tego modułu takie jak:
   - baza danych MS SQL Server,
   - tworzenie zapytań SQL,
   - użycie mikro ORM Dapper.

Wszystko to będzie możliwe do zastosowania w tym projekcie! To na pewno ugruntuje Twoją wiedzę.

---
---
---
---

## Zakres funkcjonalności logiki biznesowej aplikacji TaskManager z Dapper

**Rozszerzone założenia logiki biznesowej:**
- System umożliwia pobieranie użytkowników z zadaniami.
- Użytkownik posiada cechy: ID, nazwę, lista zadań. System posiada predefiniowanych użytkowników.
- Zadanie posiada dodatkowe pola: twórca zadania (jedna osoba) i osoba przypisana do zadania (jedna osoba).
- System pozwala na zarządzanie zadaniami: przypisywanie zadania do użytkownika.
- System umożliwia przechowywanie w bazie danych informacji o zadaniach, użytkownikach i ich relacji (powiązaniu).
- Jedno zadanie może mieć wyłącznie jednego przypisanego użytkownika.
- Jeden użytkownik może posiadać wiele przypisanych do siebie zadań.

> Pamiętaj, aby wszystkie metody komunikujące się z bazą danych były wywoływane asynchronicznie (`async`/`await`).

---

### **1. Zmiana nazw typów (refaktor nazw)**

Przed przystąpieniem do rozbudowy aplikacji warto przeznaczyć czas na dostosowanie i uporządkowanie aplikacji, czyli na refaktor.

Aplikacja posiada kilka nazw tj. `Task`, `TaskStatus`, które tworzą konflikt nazw z systemowymi zadaniami związanymi z programowaniem asynchronicznym. W trakcie warsztatów będziemy używali tych samych nazw dla typu naszego zadania, jak i zadania w rozumieniu asynchroniczności. Najlepiej rozwiązać ten problem zmieniając nazwę naszego zadania i statusu.

**Jeżeli w poprzednich warsztatach wybrano inne nazwy dla tych dwóch typów, to możesz pominąć tę część.**

Sugerujemy, aby nasze zadanie miało nazwę `TaskItem`. Do refaktoru nazw użyjemy polecenia `Rename` (skrót `F2` lub `CTRL+R+R`). Ułatwi to dostosowanie kodu, ponieważ IDE programistyczne automatycznie powinno zmienić nazwę klasy i pliku oraz użycia klasy jak i zmienne.

**Refaktor `Program`:**
- Zmień metodę `Main` tak, aby była asynchroniczna. Zmień `void` na `async Task`.

**Refaktor `Task`:**
- Zmień nazwę klasy `Task` na `TaskItem`.

**Refaktor `TaskStatus`:**
- Zmień nazwę enum `TaskStatus` na `TaskItemStatus`.

**Refaktor `TaskTests`:**
- Zmień nazwę klasy `TaskTests` na `TaskItemTests`.

**Refaktor `TaskManagerService`:**
- Wprowadź asynchroniczność do każdej metody. Zmień nazwę każdej metody dodając sufiks `Async` (użyj polecenia `Rename`). Dodaj słowo kluczowe `async` oraz wykorzystaj `Task`, a jeżeli metoda zwraca wartość to użyj `Task<>`.
- Wyszukaj każde wywołanie tych metod i dopisz obsługę `await` przy wywołaniu i upewnij się, że metoda wywołująca również jest asynchroniczna. Dostosuj wywołanie w klasach `TaskManagerService`, `TaskManagerServiceTests` oraz `Program`.

> Możesz wykorzystać w IDE polecenia do globalnego szukania tekstu `Find all` (skrót `CTRL+SHIFT+F`), aby poszukać tekst `Async(`. IDE wyświetli wszystkie miejsca, gdzie jest użyt ten tekst. Przejdź przez listę i dostosuj wywołania metod.

**Weryfikacja:**
- Po refaktorze należy zweryfikować poprawność działania aplikacji.
- W pierwszej kolejności przebuduj solucję, aby sprawdzić, czy nie ma błędów kompilacji. Jeżeli jakieś błędy wystąpią, zbadaj je i rozwiąż. Pamiętaj, że możesz sprawdzić w internecie co mogą oznaczać błędy, a także możesz zapytać na Slacku.
- Następnie uruchom wszystkie testy jednostkowe `TaskItemTests` i `TaskManagerServiceTests`. Wszystkie testy powinny przechodzić (mieć zielony kolor). Jeżeli testy dają błędny wynik, to sprawdź dlaczego i napraw testy. Jeżeli metoda testowa do sprawdzania autoinkrementacji zadania nie przechodzi, to zakomentuj ten test. Nie będzie nam potrzebny w tych warsztatach.
- Na końcu uruchom aplikację konsolową i przetestuj jej działanie. Jeżeli w aplikacji zauważysz jakieś błędy w działaniu, to napraw je.

**Jeżeli refaktor przebiegł pomyślnie, przejdź do dalszej części.**

---

### **2. Utworzenie typu `User`**

Reprezentuje pojedynczego użytkownika.
Utwórz klasę `User` w folderze `BusinessLogic`.

**Cechy:**
- `Id`: Unikalny identyfikator użytkownika w formie `int`.
- `Name`: Nazwa użytkownika (wartość wymagana).
- `Zadania`: Lista przypisanych zadań.

**Akcje:**
- Domyślny konstruktor bez parametrów: Utwórz **prywatny** konstruktor bez parametrów. Jest to obejście niezbędne do prawidłowego mapowania danych w ORM.
- Konstruktor z parametrami: Tworzy obiekt użytkownika na podstawie dostarczonego identyfikatora oraz nazwy. Lista zadań ma być utworzona i pusta.
- `ToString`: Użyj własnej wersji metody do wyświetlania informacji o użytkowniku w formacie `ID. Nazwa`.

---

### **3. Rozszerzenie `TaskItem`**

Reprezentuje pojedyncze zadanie.

**Dodatkowe cechy:**
- `CreatedBy`: Twórca zadania (`User`, wartość wymagana), tylko do odczytu (bez settera) z prywatnym polem o nazwie `_createdBy`.
- `AssignedTo`: Osoba przypisana do wykonania zadania (`User`, wartość opcjonalna, na początku równa `null`), tylko do odczytu (bez settera) z prywatnym polem o nazwie `_assignedTo`.

Niestety, ale ORM Dapper nie radzi sobie z mapowaniem właściwości do odczytu (bez settera), które pobrano przy użyciu łaczenia tabel (JOIN), które mają własny typ danych (inny niż systemowe). Z tego powodu, aby zachować hermetyzację danych i "ukryć przed światem" modyfikacje wartości, musimy zastosować obejście w postaci wprowadzenia prywatnego pola, które będzie przechowywało właściwą informację, a właściwość będzie tylko wyświetlała tę wartość. O tym jak będziemy używać tego obejścia wyjaśnimy później.

**Dodatkowe akcje:**
- Domyślny konstruktor bez parametrów: Utwórz **prywatny** konstruktor bez parametrów. Jest to obejście niezbędne do prawidłowego mapowania danych w ORM.
- Dostosuj konstruktor z parametrami: Tworzy obiekt zadania na podstawie dostarczonego identyfikatora, opisu zadania, twórcy zadania oraz opcjonalnej daty zakończenia zadania.
- Dostosuj metodę `ToString`: niech metoda zwraca dodatkowo informację o przypisanym użytkowniku do zadania (o ile taki istnieje).
- Usuń mechanizm autoinkrementacji z modelu zadania. Identyfikator będzie przekazywany w konstruktorze. Na razie mechanizm nadawania ID zadaniom przejmie `TaskManagerService` (o tym za chwilę), a docelowo baza danych (o tym później).
- `AssignTo(User? assignedTo)`: Przypisuje użytkownika do zadania. Jeżeli użytkownik jest `null` to ma "odpiąć użytkownika od zadania" i ustawić wartość `null`. Metoda zwraca `void`.

---

### **4. Rozszerzenie `TaskManagerService`**

Reprezentuje serwis przechowujący i zarządzający listą zadań.

**Dodatkowe cechy:**
- `_id`: Prywatna statyczna zmienna typu `int` o początkowej wartości `0`.

**Zmienione akcje:**
- `AddAsync(description, createdBy, dueDate)`: Dodaje nowe zadanie do listy zadań z podanym opisem, ID twórcy zadania i opcjonalną datą realizacji. Tworząc nowy obiekt zadania przekaż parametr użytkownika `User` (utwórz go podająć ID createdBy i wpisz dowolną nazwę, nie jest to istotne w tym momencie). Zwraca utworzone zadanie. Tworząc nowe zadanie metoda zwiększaj licznik `_id` preinkrementując go.

---

### **5. Dostosowanie testów `TaskItemTests`**

1. Utwórz w klasie testowej prywatnego użytkownika `User` z przykłdowymi danymi, np.
```csharp
private User _createdBy = new User(1, "Ja");
```
2. Uzupełnij w każdej metodzie testowej wywołanie konstruktora `TaskItem` o wartość `id` oraz `createdBy`. Jako wartość `id` podaj dowolnie wymyśloną wartość, np. `1`, `2`, itd. Natomiast jako twórcę zadania przekaż `_createdBy`.
3. Nie uruchamiaj jeszcze testów, należy jeszcze dostosować testy `TaskManagerServiceTests`.

---

### **6. Dostosowanie testów `TaskManagerServiceTests`**

1. Utwórz w klasie testowej prywatne ID użytkownika, np.
```csharp
private readonly int _createdBy = 1;
```
2. Uzupełnij w każdej metodzie testowej wywołanie metody `AddAsync` o wartość `createdBy`. Użyj zmiennej `_createdBy`.
3. Nie uruchamiaj jeszcze testów, należy jeszcze dostosować aplikację konsolową `Program`.

---

### **7. Dostosowanie aplikacji konsolowej `Program`**

1. Utwórz w klasie programu prywatne, statyczne ID użytkownika, np.
```csharp
private static int _createdBy = 1;
```
2. Uzupełnij wywołanie metody `TaskManagerService.AddAsync` o wartość `createdBy`. Użyj zmiennej `_createdBy`.
3. Przekompiluj solucję. Uruchom wszystkie testy oraz aplikację. Jeżeli testy przechodzą oraz aplikacja działa, przejdź dalej, jeżeli nie to napraw powstałe błędy i przejrzyj jeszcze raz instrukcję.

---

### **8. Utworzenie bazy danych `TaskManager`**

Baza danych do przechowywania informacji o zadaniach i użytkownikach.
Napisz skrypt SQL do utworzenia kompletnej bazy danych z tabelami i relacjami.

**Utwórz bazę danych `TaskManager`:**
- Napisz skrypt do utworzenia bazy danych `TaskManager`.
- Uruchom skrypt na bazie danych MS SQL Server.
- Ustaw nową bazę danych jako domyślną do użycia w kolejnych zapytaniach.

**Utwórz tabelę `Users`:**
- `Id`: klucz główny o typie `INT`, automatycznie numerowany `IDENTITY(1,1)`.
- `Name`: kolumna typu `NVARCHAR(MAX)`, wymagana.
- Uruchom skrypt na bazie danych MS SQL Server.

**Utwórz tabelę `TaskItems`:**
- `Id`: klucz główny o typie `INT`, automatycznie numerowany `IDENTITY(1,1)`.
- `Description`: kolumna typu `NVARCHAR(MAX)`, wymagana.
- `CreationDate`: kolumna typu `DATETIME`, wymagana.
- `DueDate`: kolumna typu `DATETIME`, niewymagana.
- `StartDate`: kolumna typu `DATETIME`, niewymagana.
- `DoneDate`: kolumna typu `DATETIME`, niewymagana.
- `Status`: kolumna typu `INT`, wymagana.
- `CreatedById`: klucz obcy typu `INT` do tabeli `Users`, wymagany.
- `AssignedToId`: klucz obcy typu `INT` do tabeli `Users`, niewymagany.
- Uruchom skrypt na bazie danych MS SQL Server.

**Dodaj predefiniowanych użytkowników:**
- Napisz skrypt i uruchom go, aby dodać predefiniowanych użytkowników:
   - W pierwszej kolejności dodaj użytkownika z Twoim imieniem i nazwiskiem.
   - Dodaj dwóch innych użytkowników, np. `Anna Pawlak`, `Jan Nowak`.
- Upewnij się, że masz co najmniej trzech użytkowników w bazie danych.

---

### **9. Konfiguracja Dapper i połączenia z bazą danych**

1. Zainstaluj w głównym projekcie `TaskManager` paczki NuGet: `Microsoft.Data.SqlClient` i `Dapper`.
2. W aplikacji konsolowej w klasie `Program` utwórz prywatną stałą `ConnectionString`. Pamiętaj, aby konfiguracja wskazywała na użycie nowej bazy danych `TaskManager`.
3. Dla testu i poprawności działania bazy danych utwórz w klasie `Program` prywatną statyczną metodę `TestDbAsync` i wywołaj ją na początku metody `Main`. Użyj kodu dostarczonego poniżej.
4. Jeżeli test przejdzie pozytywnie to usuń wywołanie metody.

<details>
<summary>Pokaż kod TestDbAsync</summary>

```csharp
private static async Task<string> TestDbAsync()
{
    using (var connection = new SqlConnection(ConnectionString))
    {
        var sql = @"SELECT CONCAT(
'Tabela TaskItems '
, CASE WHEN OBJECT_ID('TaskItems', 'U') IS NOT NULL THEN 'istnieje' ELSE 'nieistnieje' END
, CHAR(13)+CHAR(10)
, CONCAT('Tabela Users ', CASE WHEN OBJECT_ID('Users', 'U') IS NOT NULL THEN 'istnieje' ELSE 'nieistnieje' END)
)";
        var result = await connection.QueryFirstAsync<string>(sql);
        return result;
    }
}
```
</details>

---

### **10. Utworzenie szablonu repozytorium do komunikacji z bazą danych**

Repozytorium (*ang. repository*) to koncept/wzorzec projektowy dla klasy komunikującej się z bazą danych.

Klasa zawiera wyłącznie metody, które pobierają lub modyfikują dane w bazie danych.
Jej zadaniem jest rozdzielenie komunikacji z bazą danych od logiki biznesowej.

Do zarządzania logiką biznesową służą tzw. domeny (*ang. domain*) czyli modele biznesowe oraz serwisy (*ang. services*). Modele domenowe przechowują informacje biznesowe oraz zarządzają dostępem do informacji (są odpowiedzialne za hermetyzację). Serwisy łączą/integrują modele biznesowe z zewnętrznymi systemami, np. bazami danych, API, itd.

Repozytorium powinno być klasą implementującą interfejs `IRepository`. Interfejs służy temu, aby posiadać wiele implementacji repozytorium. Projekt z aplikacją konsolową może używać bazy danych, natomiast projekt z testami niekoniecznie. Testy są uruchamiane często i nie powinny mieć dostępu do głównej bazy danych. Zazywczaj testy pomijają komunikację z bazą danych poprzez użycie obiektu typu **mock** lub posiadają własną bazę danych tworzoną na żądanie, np. przy użyciu Dockera lub przechowującą dane w pamięci.

W warsztatach w projekcie testów wykorzystamy prostą implementację `IRepository` w formie mocka. Będzie ona dostarczona w formie gotowej klasy do przekopiowania. Nie chcemy, aby uruchamianie testów powodowało zmiany w naszej głównej bazie danych.

Zwróć uwagę, że odkąd będziemy używać repozytorium to metody serwisu będą hermetyczne, tj. dane zawsze będą aktualne i pobierane z bazy danych. W przypadku modyfikacji danych będzie następujący przepływ:
- Serwis najpierw pobierze aktualne dane z bazy przy pomocy repozytorium, które zwróci w formie modelu biznesowego, np. `TaskItem`.
- Następnie będziemy dokonywali modyfikacji tego modelu przy pomocy metod w klasie `TaskItem`.
- Efekt zmian wyślemy do repozytorium, aby zapisać je w bazie danych.

**UWAGA: Należy pamiętać, że Dapper jest mikro ORM-em, a więc jeżeli dokonamy zmian w modelu to zmiany będą widoczne w bazie danych dopiero kiedy wprost wywołamy metody modyfikacji w repozytorium.**

---

#### **Utwórz interfejs `IRepository` w folderze `BusinessLogic` z akcjami:**
- `GetAllUsersAsync()`: do pobierania wszystkich użytkowników.
- `GetUserByIdAsync(int userId)`: do pobierania użytkownika i podanym ID.
- `CreateTaskItemAsync(TaskItem newTaskItem)`: do tworzenia zadania, metoda ma zwracać ID utworzonego zadania.
- `UpdateTaskItemAsync(TaskItem newTaskItem)`: do aktualizacji zadania, zwraca informację czy aktualizacja powiodła się.
- `DeleteTaskItemAsync(int taskItemId)`: do usuwania zadania o podanym ID, zwraca informację czy usuwanie powiodło się.
- `GetTaskItemByIdAsync(int taskItemId)`: pobiera zadanie o podanym ID.
- `GetAllTaskItemsAsync()`: pobiera wszystkie zadania.
- `GetTaskItemsByStatusAsync(TaskItemStatus status)`: pobiera wszyskie zadania o podanym statusie.
- `GetTaskItemsByDescriptionAsync(string description)`: pobiera wszystkie zadania, w których występuje podana fraza w opisie.

<details>
<summary>Pokaż kod źródłowy interfejsu IRepository</summary>

```csharp
public interface IRepository
{
    Task<User[]> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<int> CreateTaskItemAsync(TaskItem newTaskItem);
    Task<bool> UpdateTaskItemAsync(TaskItem newTaskItem);
    Task<bool> DeleteTaskItemAsync(int taskItemId);
    Task<TaskItem?> GetTaskItemByIdAsync(int taskItemId);
    Task<TaskItem[]> GetAllTaskItemsAsync();
    Task<TaskItem[]> GetTaskItemsByStatusAsync(TaskItemStatus status);
    Task<TaskItem[]> GetTaskItemsByDescriptionAsync(string description);
}
```
</details>

---

#### **Utwórz klasę `Repository` w folderze `BusinessLogic`:**
- Zaimplementuj interfejs `IRepository` domyślnym zachowaniem, tak aby wywołanie każdej metody wyrzucało wyjątek. W tym celu w IDE wywołaj polecenie `Implement missing members` lub użyj gotowego kodu dostarczonego poniżej.
- Dodaj konstruktor przyjmujący parametr `connectionString` i zachowaj jego wartość w zmiennej tylko do odczytu `_connectionString`. Użyjemy tego później.

<details>
<summary>Pokaż domyślną implementację Repository</summary>

```csharp
public class Repository : IRepository
{
    private readonly string _connectionString;

    public Repository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Task<User[]> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserByIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateTaskItemAsync(TaskItem newTaskItem)
    {
        throw new NotImplementedException();
    }
    
    public Task<bool> UpdateTaskItemAsync(TaskItem newTaskItem)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteTaskItemAsync(int taskItemId)
    {
        throw new NotImplementedException();
    }

    public Task<TaskItem?> GetTaskItemByIdAsync(int taskItemId)
    {
        throw new NotImplementedException();
    }

    public Task<TaskItem[]> GetAllTaskItemsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TaskItem[]> GetTaskItemsByStatusAsync(TaskItemStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<TaskItem[]> GetTaskItemsByDescriptionAsync(string description)
    {
        throw new NotImplementedException();
    }
}
```
</details>

---

#### **Użyj interfejsu `IRepository` w klasie `TaskManagerService`:**
- W klasie `TaskManagerService` utwórz prywatną zmienną tylko do odczytu `_repository` typu `IRepository`.
- Utwórz konstruktor `TaskManagerService` z parametrem `IRepository` i przekaż wartość do zmiennej `_repository`. Dzięki temu będziemy mogli używać serwisu zarówno w aplikacji konsolowej (w wersji z prawdziwą bazą danych) jak i w testach (wersja mock).
- Usuń dwie zmienne z klasy: z użyciem autoinkrementowanego ID dla zadań oraz listę zadań. Zastąpimy ich użyciem repozytorium.
- W metodzie `AddAsync` zastąp użcie autoinkrementowanego ID wartością `0`.
- We wszystkich metodach serwisu zastąp użycie metod na liście `_tasks`, odpowiednią metodą z repozytorium. Pamiętaj o użyciu `async/await`:
   - `AddAsync`: na początku pobierz użytkownika metodą `_repository.GetUserByIdAsync` i wstaw go . Następnie zastąp użycie `_tasks.Add` wywołaniem `_repository.CreateTaskItemAsync`. Rezultat wywołania metody z repozytorium wykorzystaj do pobrania pełnego obiektu z bazy danych i zwróć jej wynik jako rezultat `AddAsync`. Do pobrania pełnego obiektu możesz użyć metody `GetAsync`. Docelowo metoda repozytorium zwróci użytkownika z uzupełnionym ID.
   - `RemoveAsync`: zastąp użycie `_tasks.Remove` wywołaniem `_repository.DeleteTaskItemAsync` z parametrem ID zadania.
   - `GetAsync`: zastąp użycie `_tasks.Find` wywołaniem `_repository.GetTaskItemByIdAsync`.
   - `GetAllAsync`: zastąp użycie `_tasks.ToArray()` wywołaniem `_repository.GetAllTaskItemsAsync()`.
   - `GetAllAsync(TaskItemStatus)`: zastąp użycie `_tasks.FindAll` wywołaniem `_repository.GetTaskItemsByStatusAsync`.
   - `GetAllAsync(string)`: zastąp użycie `_tasks.FindAll` wywołaniem `_repository.GetTaskItemsByDescriptionAsync`.
   - `ChangeStatusAsync`: dostosuj wywołanie metody, tak aby wynik wywołanie metod modelu `TaskItem` (np. `Open`, `Start`, `Done`) przechować w zmiennej pomocniczej. Następnie jeżeli udało się zmienić status, to należy zapisać zmiany w bazie danych poprzez wywołanie `_repository.UpdateTaskItemAsync` i przekazując aktualną wersję obiektu `TaskItem`. Metoda `ChangeStatusAsync` powinna zwracać rezultat wywołania metody z repozytorium, a jeżeli nie było to możliwe to ma zwrócić `false`.

<details>
<summary>Pokaż kod źródłowy TaskManagerService</summary>

```csharp
public class TaskManagerService
{
    private readonly IRepository _repository;

    public TaskManagerService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskItem> AddAsync(string description, int createdBy, DateTime? dueDate)
    {
        var user = await _repository.GetUserByIdAsync(createdBy);
        var task = new TaskItem(0, description, user, dueDate);
        var id = await _repository.CreateTaskItemAsync(task);
        return await GetAsync(id);
    }

    public async Task<bool> RemoveAsync(int taskId)
    {
        var task = await GetAsync(taskId);
        if (task != null)
            return await _repository.DeleteTaskItemAsync(task.Id);
        return false;
    }

    public async Task<TaskItem?> GetAsync(int taskId)
    {
        return await _repository.GetTaskItemByIdAsync(taskId);
    }

    public async Task<TaskItem[]> GetAllAsync()
    {
        return await _repository.GetAllTaskItemsAsync();
    }

    public async Task<TaskItem[]> GetAllAsync(TaskItemStatus itemStatus)
    {
        return await _repository.GetTaskItemsByStatusAsync(itemStatus);
    }

    public async Task<TaskItem[]> GetAllAsync(string description)
    {
        return await _repository.GetTaskItemsByDescriptionAsync(description);
    }

    public async Task<bool> ChangeStatusAsync(int taskId, TaskItemStatus newStatus)
    {
        var task = await GetAsync(taskId);
        if (task == null || task?.Status == newStatus)
            return false;

        var result = ChangeStatus(task, newStatus);
        if (result)
        {
            return await _repository.UpdateTaskItemAsync(task);
        }

        return false;
    }

    private bool ChangeStatus(TaskItem task, TaskItemStatus newStatus)
    {
        switch (newStatus)
        {
            case TaskItemStatus.ToDo:
                return task.Open();
            case TaskItemStatus.InProgress:
                return task.Start();
            case TaskItemStatus.Done:
                return task.Done();
            default:
                return false;
        }
    }
}
```
</details>

---

#### **W klasie `Program` dostosuj tworzenie obiektu `TaskManagerService`:**
- Poszukaj linii kodu z tworzeniem obiektu `TaskManagerService` i przekaż w jego konstruktorze obiekt repozytorium z konfiguracją połączenia z bazą danych `new Repository(ConnectionString)`. Użyjemy tego później, ale na ten moment potrzebujemy działającego kodu.

---

#### **W projekcie testów utwórz klasę `MockRepository`:**
- Zaimplementuj interfejs `IRepository` udający połączenie z bazą danych. W tym celu wykorzystaj dostarczony kod poniżej.
- `MockRepository` robi to co w pierwotnej wersji robił serwis, czyli przechowuje w pamięci listę zadań i umożliwia zarządzanie nimi na potrzeby testów.

<details>
<summary>Pokaż implementację MockRepository</summary>

```csharp
public class MockRepository : IRepository
{
    private int _taskId = 0;
    private List<TaskItem> _tasks = new List<TaskItem>();

    private List<User> _users = new List<User> { new User(1, "Ja") };

    public async Task<User[]> GetAllUsersAsync() => _users.ToArray();

    public async Task<User?> GetUserByIdAsync(int userId) => _users.FirstOrDefault(u => u.Id == userId);

    public async Task<int> CreateTaskItemAsync(TaskItem newTaskItem)
    {
        var newTask = new TaskItem(newTaskItem.Id == 0 ? ++_taskId : newTaskItem.Id, newTaskItem.Description, newTaskItem.CreatedBy, newTaskItem.DueDate);
        _tasks.Add(newTask);
        return newTask.Id;
    }

    public async Task<bool> UpdateTaskItemAsync(TaskItem newTaskItem)
    {
        var result = await DeleteTaskItemAsync(newTaskItem.Id);
        if (result)
            _tasks.Add(newTaskItem);
        return result;
    }

    public async Task<bool> DeleteTaskItemAsync(int taskItemId)
    {
        var task = await GetTaskItemByIdAsync(taskItemId);
        return _tasks.Remove(task);
    }

    public async Task<TaskItem?> GetTaskItemByIdAsync(int taskItemId) => _tasks.Find(t => t.Id == taskItemId);

    public async Task<TaskItem[]> GetAllTaskItemsAsync() => _tasks.ToArray();

    public async Task<TaskItem[]> GetTaskItemsByStatusAsync(TaskItemStatus status) => _tasks.Where(t => t.Status == status).ToArray();

    public async Task<TaskItem[]> GetTaskItemsByDescriptionAsync(string description) =>
        _tasks.FindAll(t => t.Description.Contains(description, StringComparison.InvariantCultureIgnoreCase)).ToArray();
}
```
</details>

---

#### **Użyj `MockRepository` w projekcie testów:**
- Przejdź do klasy `TaskManagerServiceTests` i dostosuj tworzenie obiektu `TaskManagerService` poprzez dodanie w konstruktorze `new MockRepository()`.

---

#### **Weryfikacja:**
- Po wstępnej rozbudowie i małym refaktorze należy zweryfikować poprawność działania aplikacji.
- W pierwszej kolejności przebuduj solucję, aby sprawdzić, czy nie ma błędów kompilacji. Jeżeli jakieś błędy wystąpią, zbadaj je i rozwiąż. Pamiętaj, że możesz sprawdzić w internecie co mogą oznaczać błędy, a także możesz zapytać na Slacku.
- Następnie uruchom wszystkie testy jednostkowe `TaskItemTests` i `TaskManagerServiceTests`. Wszystkie testy powinny przechodzić (mieć zielony kolor). Jeżeli testy dają błędny wynik, to sprawdź dlaczego i napraw testy. Jeżeli metoda testowa do sprawdzania autoinkrementacji zadania nie przechodzi, to zakomentuj ten test. Nie będzie nam potrzebny w tych warsztatach.
- Nie testuj działania aplikacj, ponieważ użyliśmy w niej repozytorium, w którym wszystkie metody zgłaszają wyjątek `throw new NotImplementedException();`.

---

### **11. Utworzenie metod repozytorium `Repository` ze skryptami SQL**

Kiedy mamy utworzony szablon klasy repozytorium, możemy przystąpić do implementacji poszczególnych metod wykorzystując Dapper i MS SQL.

W tej sekcji będziemy implementować metody w klasie `Repository`. Pamiętaj, aby w każdej metodzie najpierw nawiązać nowe połączenie z bazą danych.

**UWAGA:** w tym miejscu będziemy musieli zastosować wcześniej wspomniane obejście, aby prawidłowo mapować dane zadania związane z twórcą zadania (`CreatedBy`) i osobą do niego przypisaną (`AssignedTo`). Wykorzystamy do tego poniższy kod klasy `DapperExtensions`. Dodaj go do folderu `BusinessLogic` i użyj metodę rozszerzającą `FixDapperMappings` na obiekcie klasy `TaskItem` we wskazanych metodach.

<details>
<summary>Pokaż kod DapperExtensions</summary>

Niniejsze rozwiązanie używa tzw. refleksji. Refleksja w C# to zdolność programu do analizy własnej struktury, informacji o typach i manipulowania nimi w trakcie działania programu. Pozwala na dynamiczne badanie, dostęp i modyfikację typów, metod, właściwości, pól itp. w czasie wykonania. Refleksja jest zaawansowanym i potężnym narzędziem, ale należy z nią obchodzić się ostrożnie, ponieważ może prowadzić do kodu trudnego do zrozumienia i utrzymania. Nie będziemy omawiali na tym kursie szczegółowego działania refleksji.

```csharp
using System.Reflection;

namespace TaskManager.BusinessLogic
{
    public static class DapperExtensions
    {
        public static TaskItem FixDapperMapping(this TaskItem taskItem, User createdBy, User assignedTo)
        {
            SetValueToObject(taskItem, "_createdBy", createdBy);
            SetValueToObject(taskItem, "_assignedTo", assignedTo);
            return taskItem;
        }

        private static void SetValueToObject(object obj, string fieldName, object value)
        {
            var type = obj.GetType();
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(obj, value);
        }
    }
}
```
</details>

#### **Wersja podstawowa:**

- `GetAllUsersAsync`: Użyj odpowiedniej metody Dappera do pobrania wszystkich użytkowników (bez zadań).
- `GetUserByIdAsync`: Użyj odpowiedniej metody Dappera do pobrania jednego użytkownika (bez zadań).
- `CreateTaskItemAsync`: Użyj odpowiedniej metody Dappera do wywołania skryptu, który utworzy zadanie (przekazując opis, użytkownika tworzącego, datę utworzenia, satus, i datę ważności zadania) i zwróci na końcu ID nowego zadania (nadane przez bazę danych). Następnie pobierz na podstawie tego ID obiekt z bazy danych i zwróć go (możesz wykorzystać do tego metodę `GetTaskItemByIdAsync`). Do pobrania nadanego ID możesz użyć skryptu `SELECT SCOPE_IDENTITY();`
- `UpdateTaskItemAsync`: Użyj odpowiedniej metody Dappera do wywołania skryptu modyfikacji zadania (przekazując status, datę startu, datę zakończenia zadania, osobę przypisaną do zadania) na podstawie podanego ID. Zwróć informację z metody o tym, czy udało się zaktualizować zadanie.
- `DeleteTaskItemAsync`: Użyj odpowiedniej metody Dappera do wywołania skryptu, który usunie jedno zadanie na podstawie podanego ID. Zwróć informację z metody o tym, czy udało się usunąć zadanie.
- `GetTaskItemByIdAsync`: Użyj odpowiedniej metody Dappera do wywołania skryptu, który pobierze pełny obiekt zadania (tzn. wraz z przypisanymi do niego użytkownikami jako twórcy i wykonawcy zadania). Zwróć obiekt zadania. W tym miejscu musimy wykorzystać nasze obejście `FixDapperMapping`. Sprawdź podpowiedź znajdującą się poniżej.
- `GetAllTaskItemsAsync`: Użyj odpowiedniej metody Dappera do wywołania skryptu, który pobierze wszystkie pełne obiekty zadania (tzn. wraz z przypisanymi do niego użytkownikami jako twórcy i wykonawcy zadania). Zwróć tablicę zadań. W tym miejscu musimy wykorzystać nasze obejście `FixDapperMapping`. Sprawdź podpowiedź znajdującą się poniżej.
- `GetTaskItemsByStatusAsync`: Użyj odpowiedniej metody Dappera do wywołania skryptu, który pobierze wszystkie pełne obiekty zadania (tzn. wraz z przypisanymi do niego użytkownikami jako twórcy i wykonawcy zadania) na podstawie podanego statusu zadania. Zwróć tablicę zadań. W tym miejscu musimy wykorzystać nasze obejście `FixDapperMapping`. Sprawdź podpowiedź znajdującą się poniżej.
- `GetTaskItemsByDescriptionAsync`: Użyj odpowiedniej metody Dappera do wywołania skryptu, który pobierze wszystkie pełne obiekty zadania (tzn. wraz z przypisanymi do niego użytkownikami jako twórcy i wykonawcy zadania) na podstawie podanej frazy występującej w opisie zadania (użyj `%FRAZA%`). Zwróć tablicę zadań. W tym miejscu musimy wykorzystać nasze obejście `FixDapperMapping`. Sprawdź podpowiedź znajdującą się poniżej.

<details>
<summary>Podpowiedź do użycia FixDapperMapping</summary>

Przykład pobrania jednego elementu, ale przy użyciu `QueryAsync` (w przypadku pobierania relacji JOIN dla pojedynczego rekordu, musimy użyć `QueryAsync`):
```csharp
var tasks = await connection.QueryAsync<TaskItem, User, User, TaskItem>(
    sql,
    (task, createdBy, assignedTo) =>
    {
        return task.FixDapperMapping(createdBy, assignedTo);
    },
    new {TaskId = taskItemId});
return tasks.FirstOrDefault();
```

Przykład pobierania wielu elementów bez filtrowania:
```csharp
var tasks = await connection.QueryAsync<TaskItem, User, User, TaskItem>(
    sql,
    (task, createdBy, assignedTo) =>
    {
        return task.FixDapperMapping(createdBy, assignedTo);
    });
return tasks.ToArray();
```

Przykład pobrania wielu elementów z filtrowaniem:
```csharp
var tasks = await connection.QueryAsync<TaskItem, User, User, TaskItem>(
    sql,
    (task, createdBy, assignedTo) =>
    {
        return task.FixDapperMapping(createdBy, assignedTo);
    },
    new {TaskStatus = status});
return tasks.ToArray();
```
</details>

#### **Wersja rozszerzona (dla chętnych):**
- `GetAllUsersAsync`: Zmodyfikuj metodę tak, aby pobrała pełne obiekty użytkownika wraz z przypisanymi do niego zadaniami.
- `GetUserByIdAsync`: Zmodyfikuj metodę tak, aby pobrała pełny obiekt użytkownika wraz z przypisanymi do niego zadaniami na podstawie podanego ID użytkownika.

---

### **12. Dodaj nowe funkcjonalności do aplikacji konsolowej.**

Rozszerz opcje aplikacji konsolowej o:
- wyświetlanie użytkowników,
- przypisywanie zadania do użytkownika.

Pamiętaj, aby wyświetlić nowe opcje w menu w interfejsie użytkownika.

#### **Rozszerz wyświetlanie szczegółów o zadaniu:**
- Wyświetl dodatkowe informacje dla szczegółów zadania o twórcę zadania i osobę przypisaną do zadania.

#### **Wyświetlanie listy użytkowników:**
- Rozszerz klasę `TaskManagerService` o metodę `GetAllUsersAsync`. Wykorzystaj istniejącą metodę repozytorium `GetAllUsersAsync`.
- W konsoli wywołaj nową metodę serwisu i wyświetl zwrócone dane.

#### **Przypisywanie użytkownika do zadania:**
- Rozszerz klasę `TaskManagerService` o metodę `AssignToAsync(taskId, userId)`, która zwróci informację czy udało się ustawić wykonawcę zadania. Niech metoda pobierze aktualną wersję zadania oraz użytkownika, następnie w modelu biznesowym zadania wywołaj metodę `AssignTo`, a na koniec zaktualizuj model zadania w bazie danych. ID użytkownika jest opcjonalne i może przyjąć wartość `null`, wówczas należy odsunąć użytkownika od zadania (i przekazać `null` do metody `AssignTo`). Metoda `AssignToAsync` powinna używać już istniejące metody serwisu i repozytorium, aby nie powielać kodu.
- W konsoli wywołaj nową metodę serwisu i wyświetl stosowny komunikat.

---

### **13. Dodatkowe testy jednostkowe.**

Dopisz testy jednostkowe sprawdzające `TaskItem` oraz `TaskManagerService`.

#### **`TaskItemTests`:**
- Dopisz dwa scenariusze testowe sprawdzający działanie metody `AssignTo`, w przypadku gdy:
   - podamy obiekt użytkownika,
   - podamy wartość `null`.

#### **`TaskManagerServiceTests`:**
- Dopisz cztery scenariusze testowe sprawdzające działanie metody `AssignToAsync`, w przypadku gdy:
   - podamy właściwe ID zadania i użytkownika (przypisze zadanie do użytkownika),
   - podamy właściwe ID zadania i pustego użytkownika (ustawi `null` przypisanemu użytkownikowi),
   - podamy właściwe ID zadania, ale nie właściwe ID użytkownika (nie ustawi użytkownika),
   - podamy niewłaściwe ID zadania (nie ustawi użytkownika).

---

### **14. GRATULACJE! Właśnie jesteś na końcu warsztatów.**

Gratulujemy i dziękujemy za aktywny udział w warsztatach. Twoje zaangażowanie i chęć do nauki są dla nas inspiracją. Życzymy powodzenia w dalszej części kursu!

---

### **Aplikację możesz dowolnie rozszerzyć o dodatkowe funkcjonalności.**

Pamiętaj, że ogranicza Cię tylko Twoja wyobraźnia. Możesz dalej rozwijać tę aplikację i rozszerzać ją o dodatkowe funkcjonalności, np.:
- wyświetlanie Twoich zadań,
- wyświetlanie zadań pogrupowanych po użytkownikach wykonujących je,
- wyświetlanie nieprzypisanych zadań,
- i wiele, wiele innych...


---
---
---
---

## Podsumowanie warsztatów TaskManager z Dapper

Po intensywnej pracy pełnej nauki i programowania, nadszedł czas na podsumowanie warsztatów dotyczących tworzenia aplikacji TaskManager w języku C#.

### Główne punkty warsztatu

1. **Podział aplikacji na warstwy**: Podzieliliśmy kod aplikacji na mniejsze bloczki:
- modele biznesowe (warstwa domeny),
- repozytorium (warstwa dostępu do danych),
- serwis (warstwa aplikacji),
- konsola (warstwa prezentacji).

2. **Rozbudowanie modeli**: Rozbudowaliśmy model zadania o nowe cechy i akcje. Utworzliśmy nowy model użytkownika.

3. **Utworzenie bazy danych**: Utworzyliśmy bazę danych `TaskManager` z tabelami zadań i użytkowników.

4. **Połączenie z bazą danych przy użyciu Dapper**: Zainstalowaliśmy i skonfigurowaliśmy paczkę NuGet Dapper. Utorzyliśmy interfejs i repozytorium, za pomocą którego łączymy się z bazą danych.

5. **Rozbudowanie testów**: Zmodyfikowaliśmy testy o wykorzystanie mock-a repozytorium. Dopisaliśmy nowe scenariusze testowe.

6. **Rozbudowanie aplikacji konsolowej**: Zmodyfikowaliśmy aplikację konsolową i wprowadziliśmy programowanie asynchroniczne. Dodaliśmy nowe funkcje do aplikacji.

### Dalsze kroki

Zachęcamy do rozwijania aplikacji TaskManager. Możliwe są takie rozszerzenia jak: dodawanie priorytetów dla zadań, dodanie śledzenia historii zmian w zadaniu, dodawanie komentarzy do zadań, tworzenie interfejsu programistycznego WebAPI i interfejsu graficznego dla użytkownika w formie strony WWW.

### Gratulacje!

Życzymy powodzenia w dalszej części kursu!
