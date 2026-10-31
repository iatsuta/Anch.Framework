# IdentitySource и VisualIdentitySource

Два небольших родственных модуля. Отвечают на вопрос «как у **произвольного** доменного типа получить идентификатор / отображаемое имя, не зная его заранее». Оба строятся на `PropertyAccessors` (выражения).

## IdentitySource (`src/IdentitySource/`)

| Проект | Содержимое |
|---|---|
| `Anch.IdentitySource.Abstractions` | `IIdentityInfoSource`, `IIdentityInfo<TDomain,TIdent>`, `IdentityInfo` (record), `IIdentityPropertyExtractor` |
| `Anch.IdentitySource.Runtime` | `IdentityInfoSource`, `IdentityInfoProxy`, `IdentityPropertyExtractor`, `IdentityPropertySourceSettings` |
| `Anch.IdentitySource` | DI: `AddIdentitySource(setup?)`, `IIdentitySourceSetup` |
| `Anch.IdentitySource.Tests` | Тесты |

Ключевые типы:
- `IdentityInfo<TDomainObject,TIdent>(PropertyAccessors Id)`: путь к Id, `GetId(obj)`, `CreateFilter(ident)` / `CreateFilter(idents)` (готовые `Expression<Func<T,bool>>` для Where: равенство и `Contains`).
- `IIdentityInfoSource.GetIdentityInfo<T>()` / `TryGetIdentityInfo<T>()` / по `Type`.

Как находится свойство Id:
1. Если задано явно через `AddIdentitySource(b => b.SetId((T x) => x.Key))` берётся оно.
2. Иначе `IdentityPropertyExtractor` ищет свойство по имени из `IdentityPropertySourceSettings` (по умолчанию `"Id"`; поменять: `b.SetSettings(new IdentityPropertySourceSettings("MyId"))`).

```csharp
services.AddIdentitySource(b => b.SetId((Location l) => l.MyId));   // как в _Example
var info = sp.GetRequiredService<IIdentityInfoSource>().GetIdentityInfo<Location, Guid>();
```

Потребители: `HierarchicalExpand`, `SecuritySystem`, интеграционные тесты NHibernate.

## VisualIdentitySource (`src/VisualIdentitySource/`)

Структура зеркальна IdentitySource:

| Проект | Содержимое |
|---|---|
| `Anch.VisualIdentitySource.Abstractions` | `IVisualIdentityInfoSource`, `IVisualIdentityInfo<T>` (`Name` как `PropertyAccessors<T,string>`), `IDisplayObjectInfoSource`, `IDomainObjectDisplayService` (`Format(obj)`), `IVisualIdentityPropertyExtractor` |
| `Anch.VisualIdentitySource.Runtime` | `VisualIdentityInfoSource`, `DisplayObjectInfoSource`, `DomainObjectDisplayService`, `*Proxy`, `VisualIdentityPropertyExtractor`, `VisualIdentityPropertySourceSettings` |
| `Anch.VisualIdentitySource` | DI: `AddVisualIdentitySource(setup?)` |
| `Anch.VisualIdentitySource.Tests` | Тесты (`DomainObjectDisplayServiceTests`, …) |

Поиск «имени» по умолчанию: первое существующее свойство из списка `Login`, `Name`, `Code` (`VisualIdentityPropertySourceSettings.Default`). Переопределение: `AddVisualIdentitySource(b => b.SetName(x => x.MyName))`, или функция отображения целиком: `b.SetDisplay((T v) => v.MyName)`.

Подходит только строковое (`string`) публичное свойство. Берётся первое найденное по порядку списка.

`IDomainObjectDisplayService.Format(obj)` даёт строку для сообщений и UI. Потребители в SecuritySystem: `PlainTypedSecurityContextStorage`, `HierarchicalTypedSecurityContextStorage` (имена контекстов безопасности), `PermissionDelegationValidator` (GeneralPermission).
