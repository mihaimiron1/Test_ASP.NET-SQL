# MyApp — Monitorul Alegerilor

## 1. Numele proiectului

**MyApp** (Monitorul Alegerilor / Election Monitor)

---

## 2. Limbajele și tehnologiile folosite

| Componentă | Tehnologie | Versiune |
|---|---|---|
| **Backend** | C# | .NET 10.0 |
| **Framework web** | ASP.NET Core MVC | .NET 10.0 |
| **Frontend** | HTML, CSS, JavaScript | — |
| **CSS Framework** | Tailwind CSS | 4.1.17 |
| **Grafice interactive** | AmCharts 5 | 5.14.4 |
| **Grafice statistice** | ApexCharts | 3.46.0 |
| **Componente UI** | Flowbite | 4.0.1 |
| **ORM ușor** | Dapper | 2.1.66 |
| **ORM complet** | Entity Framework Core | 10.0.1 |
| **Baza de date** | SQL Server | (Windows Auth) |
| **Localizare** | ASP.NET Core Localization | built-in |

---

## 3. Ce face aplicația

**MyApp** este un sistem web de **monitorizare și vizualizare a rezultatelor alegerilor** din Republica Moldova (alegerile locale din 5 noiembrie 2023). Principalele funcționalități:

- **Hartă termică interactivă** — afișează rata de participare la vot pe regiuni (raioane), codificată color pe o hartă GeoJSON a Moldovei.
- **Statistici demografice** — analiza votanților pe **gen** (bărbați / femei) și pe **grupe de vârstă**, la nivel de raion sau localitate.
- **Rezultate alegeri** — clasamente cu primele 7 partide/candidați pentru:
  - Primari municipali și locali
  - Consilieri municipali, locali și raionali
- **Navigare ierarhică** — Raion → Localitate → Secție de votare.
- **Suport multilingv** — interfață disponibilă în **română** și **rusă**.
- **Performanță** — date cache-uite în memorie, actualizate automat în fundal la fiecare 10 minute.

---

## 4. Concepte importante folosite

### Arhitectură & Modele de proiectare
| Concept | Detalii |
|---|---|
| **MVC (Model-View-Controller)** | Separarea clară a logicii, datelor și prezentării în ASP.NET Core |
| **Repository Pattern** | Interfețe `IStatisticsRepository`, `IElectionResultRepository`, `IAssignedVoterRepository` abstractizează accesul la date |
| **Dependency Injection** | Toate serviciile și repository-urile sunt înregistrate și injectate prin containerul DI al ASP.NET Core |
| **Factory Pattern** | `DbConnectionFactory` / `IDbConnectionFactory` gestionează crearea conexiunilor la baza de date |
| **Service Layer** | Logica de business separată în servicii (`StatisticsCacheService`, `RegionMapIdMapper`) |
| **Background Services** | `StatisticsCacheService` moștenește `BackgroundService` și actualizează cache-ul periodic |
| **OOP** | Clase, interfețe, moștenire și polimorfism utilizate pe scară largă în toată aplicația |

### Baze de date
| Concept | Detalii |
|---|---|
| **Stored Procedures** | Toată logica de interogare este în proceduri stocate SQL Server (`sp_GetRaionVotingStatsForHeatMap`, `sp_GetRaionGender`, `sp_GetRaionAgeCategory` etc.) |
| **Dapper ORM** | Mapare ușoară a rezultatelor SQL la obiecte C# (`QueryAsync<T>`) |
| **Entity Framework Core** | Configurat ca ORM alternativ pentru operații de tip CRUD |
| **Connection Pooling** | Conexiunile sunt gestionate eficient prin factory și `await using` |

### Algoritmi & Procesarea datelor
| Concept | Detalii |
|---|---|
| **Sortare** | Rezultatele alegerilor sunt sortate descrescător după numărul de voturi (`OrderByDescending`) și limitate la top 7 (`Take(7)`) |
| **Calculul procentelor** | Calculat dinamic în repository: `Math.Round(count / total * 100, 2)` |
| **Mapare ID-uri geografice** | `Dictionary<int, string>` mapează ID-urile din baza de date la coduri GeoJSON (`MD-AN`, `MD-BS` etc.) |
| **Transformare date** | DTO-uri (Data Transfer Objects) pentru a transforma și expune datele din baza de date către frontend |

### Caching & Performanță
| Concept | Detalii |
|---|---|
| **In-Memory Cache** | `IMemoryCache` stochează statisticile regionale cu TTL de 10–15 minute |
| **Cache-Aside Pattern** | La cerere, se verifică cache-ul; dacă lipsesc datele (cache miss), se încarcă din DB și se salvează în cache |
| **Background Refresh** | Un serviciu hosted actualizează automat cache-ul la fiecare 10 minute fără intervenția utilizatorului |
| **Async/Await** | Toate operațiile I/O (DB, rețea) sunt asincrone pentru scalabilitate maximă |

### Frontend & Vizualizare
| Concept | Detalii |
|---|---|
| **GeoJSON Maps** | Harta interactivă a Moldovei redată cu AmCharts 5 pe baza datelor GeoJSON |
| **Grafice dinamice** | ApexCharts generează grafice de tip pie/bar pentru statistici de gen și vârstă |
| **Responsive Design** | Tailwind CSS asigură adaptabilitatea interfeței pe toate dispozitivele |
| **AJAX / Fetch API** | Datele statistice sunt încărcate asincron din controller fără reîncărcarea paginii |

### Localizare
| Concept | Detalii |
|---|---|
| **i18n (Internaționalizare)** | Fișiere `.resx` separate pentru română (`ro`) și rusă (`ru`) |
| **Culture Switching** | `LanguageController` permite utilizatorului să schimbe limba interfeței |

---

## Structura proiectului

```
MyApp/
├── Controllers/          # MVC Controllers (Rezultate, Statistics, Language)
├── Models/               # Modele de date și DTO-uri
│   ├── Statistics/       # RegionVotingStatistic, DetailedRegionStatistic etc.
│   └── ElectionResult/   # ElectionMunicipalityResult, DTO-uri
├── Repositories/         # Interfețe + implementări pentru accesul la date
├── Services/             # DbConnectionFactory, StatisticsCacheService, RegionMapIdMapper
├── Views/                # Template-uri Razor (.cshtml)
│   ├── Statistics/       # HeatMap.cshtml
│   └── Rezultate/        # PrimariMunicipali, ConsilieriLocali etc.
├── Resources/            # Fișiere de localizare (.resx) RO + RU
├── wwwroot/              # Fișiere statice (CSS, JS, imagini)
├── Docs/                 # Documentație internă
├── Program.cs            # Configurare DI, middleware, localizare
├── appsettings.json      # Configurare conexiune SQL Server
└── MyApp.csproj          # Fișier proiect .NET 10.0
```