# Сборка, тесты, CI, публикация

## Требования

- .NET SDK **10.0.x** (`TargetFramework=net10.0`, `LangVersion=latest`, используются C# 14 `extension`-блоки).
- Node.js (в CI 18) + npm: только для сборки Angular-UI конфигуратора (`Anch.SecuritySystem.Configurator`). Результат `configurator-ui/dist/**` встраивается в сборку через `EmbeddedResource`. Без `dist` C#-проект всё равно соберётся, но UI админки будет отсутствовать (пакет для публикации надо собирать только после сборки UI, как в `publish.yml`).
- `global.json` включает **Microsoft.Testing.Platform** как test runner → тесты запускаются `dotnet test --project <папка>` (не `dotnet test <sln>`).

## Сборка

Все команды из корня репозитория.

```bash
# UI (нужен только если важна админка Configurator; в CI делается всегда)
cd src/SecuritySystem/Anch.SecuritySystem.Configurator/configurator-ui && npm ci && npm run ng build && cd -

dotnet restore src/Anch.Framework.slnx
dotnet build src/Anch.Framework.slnx -c Release
```

Только нужный модуль (без UI), например: `dotnet build src/Core/Anch.Core/Anch.Core.csproj`.

Версии пакетов NuGet централизованы в `src/Directory.Packages.props` (`ManagePackageVersionsCentrally`; версия `$(DotNet)` для всех пакетов Microsoft). В `csproj` у `PackageReference` версии **не указываются**. Новые пакеты добавляйте через `PackageVersion` в `Directory.Packages.props`.

## Тестовые проекты

Запуск: `dotnet test --project <путь к проекту> -c Release` (так делает CI).

| Модуль | Проект | Что покрывает |
|---|---|---|
| Core | `src/Core/Anch.Core.Tests` | Maybe, выражения, Dictionary, async-enumerable, **`CsprojValidationTests`** (проверка конвенции `PackageId == имя проекта` для всех csproj) |
| DI | `src/DependencyInjection/Anch.DependencyInjection.Tests` | Прокси-фабрика, валидаторы |
| IdentitySource | `src/IdentitySource/Anch.IdentitySource.Tests` | Поиск Id |
| VisualIdentitySource | `src/VisualIdentitySource/Anch.VisualIdentitySource.Tests` | Отображаемые имена |
| GenericQueryable | `Anch.GenericQueryable.UnitTests`, `…IntegrationTests.NHibernate`, `…IntegrationTests.EntityFramework` | (`…IntegrationTests` = общая библиотека тестов, не запускается сама) |
| HierarchicalExpand | `Anch.HierarchicalExpand.Tests`, `…IntegrationTests.NHibernate`, `…IntegrationTests.EntityFramework` | то же |
| SecuritySystem | `Anch.SecuritySystem.DiTests`, `_Example/ExampleApp.IntegrationTests.NHibernate`, `_Example/ExampleApp.IntegrationTests.EntityFramework` | `ExampleApp.IntegrationTests` общая библиотека |
| OData | `src/OData/Anch.OData.Tests` | Парсинг/конвертация |
| Testing | `src/Testing/Anch.Testing.Tests` | Инфраструктура тестов |
| Caching, Parsing, RelativePath, Threading, GenericRepository, Configurator | *(отдельных тестов нет)* | Parsing/Threading/RelativePath частично через другие модули |

Пример: `dotnet test --project src/HierarchicalExpand/Anch.HierarchicalExpand.Tests -c Release`.

Интеграционные тесты используют SQLite-файлы (создаются автоматически, см. [testing.md](modules/testing.md)); внешняя БД не нужна.

### Как устроены тесты

Окружение (`TestEnvironment`) и пул DI-контейнеров, `[AnchFact]`, `[assembly: AnchTestFramework<…>]`: см. [modules/testing.md](modules/testing.md). Тесты, как правило, принимают сервисы через конструктор.

## CI (GitHub Actions)

`.github/workflows/build.yml` (на push/PR в `main`): Node 18 → `npm ci && npm run ng build` → .NET 10 → `dotnet restore` → `dotnet build -c Release` → по одному шагу `dotnet test --project … --no-build -c Release` на каждый тестовый проект из таблицы выше. **Новый тестовый проект нужно добавить в `build.yml` вручную.**

`.github/workflows/publish.yml` (на публикацию GitHub Release): сборка UI → `dotnet pack src/Anch.Framework.slnx /p:PackageVersion=<tag релиза> /p:InformationalVersion=<sha>` → `dotnet nuget push` на nuget.org (секрет `NUGET_Anch`). Версия пакетов = **тег релиза**; значение `AssemblyVersion` в `src/__RootSolutionItems/RootCommonAssemblyInfo.cs` (сейчас `3.9.3.0`) задано вручную и с тегом не связано.

## Добавление нового проекта (чек-лист)

1. Каталог `src/<Модуль>/Anch.<Модуль>[.<Слой>]/`, csproj с `<PackageId>` равным имени проекта (проверяется тестом `CsprojValidationTests`) и при необходимости `<RootNamespace>`.
2. Добавить проект в `src/Anch.Framework.slnx` (в нужную `<Folder>`).
3. Ссылки на другие проекты через `ProjectReference`, пакеты через `PackageReference` без версии (+ запись в `Directory.Packages.props`).
4. Для тестового проекта: `<IsPackable>false</IsPackable>`, `<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>`, ссылка на `Anch.Testing.Xunit`, шаг в `build.yml`.
5. Общие настройки (nullable, LangVersion, NoWarn, `CS4014/CS1998/CA2016` как ошибки) приходят из `src/Directory.Build.props` автоматически. Туда же линкуется `__RootSolutionItems/RootCommonAssemblyInfo.cs`.

## Граф знаний (graphify)

В git хранится только `graphify-out/graph.json`; отчёт, HTML и кеш создаются локально и игнорируются (`.gitignore`).

- Обновить граф: `graphify update .` (только код, без LLM), затем `git add graphify-out/graph.json`.
- Обновляйте граф **отдельным коммитом** после крупных изменений кода. Кто перегенерировал граф, тот его и коммитит.
- Граф не мёржится. Конфликт в `graph.json` решается не правкой, а пересборкой: `graphify update .` и `git add graphify-out/graph.json`.
- Слияние настроено как обычное (merge-драйвер graphify не используется); дифф файла скрыт (`-diff`, `linguist-generated`).

## Частые проблемы

| Симптом | Причина / решение |
|---|---|
| Страница `/admin/configurator` отдаёт 404 / нет статики | Не собран Angular-UI перед сборкой C#: выполните `npm ci && npm run ng build` в `configurator-ui`, затем пересоберите проект |
| `dotnet test` на `.slnx` ничего не запускает | Используйте `--project <csproj или папка>` (Microsoft.Testing.Platform) |
| Тест падает на «has been registered many times» | Сработал `DuplicateServiceUsageValidator`, см. [dependency-injection.md](modules/dependency-injection.md) |
| Предупреждения как ошибки `CS4014/CS1998/CA2016` | Это настройка репозитория: не забытый `await`, `async` без `await`, не переданный `CancellationToken` |
| В каталогах `src/**/Anch.X` и рядом `src/**/X` (без префикса) | Второй это мусор после переименования (`bin/obj`), не трогать |
