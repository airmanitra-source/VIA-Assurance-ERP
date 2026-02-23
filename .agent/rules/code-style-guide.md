---
trigger: always_on
---

# Application Architecture

The application follows a Clean Architecture pattern. It is composed of modules for Business Logic, an Infrastructure layer (handling all database CRUD operations), and a ClientApp (the UI layer, built with .NET Blazor).

## General Guidelines and Naming Conventions

* Migration scripts are written using t-sql. You must follow syntax rules and check for errors.

* Interface and implementation must not be located on the same file.

* Asynchronous Methods: All async methods must be suffixed with Async.

* Property Ordering: Properties within classes must be sorted alphabetically, with the exception of Id fields which should come first.

## Infrastructure Project

The Infrastructure layer is located in the Filetable.Infrastructure folder. It contains all implementations for read-only and read-write data providers.

* Contracts: Interfaces for these providers must be defined within the Business Logic modules and prefixed with `Create`, `Read`, `Update`, or `Delete`.

* Entities: The Entities folder contains objects that are direct mirrors of the database tables (Schema: [documentdb].[dbo]). They are aware of Data Models and are responsible for mapping into them.

* Migrations: The Migrations folder stores all scripts for database schema changes. Scripts must follow this naming convention: {YYYYMMdd}_{alter/create/delete}_{TableName}_{optional_description}.

## Modules Project

Modules encapsulate the business logic. Each module is divided into 'Business' and 'Data' folders:

* Business: Contains all objects related to business logic.

* Data: Subdivided into Models and Providers. The Providers folder contains the contracts implemented by the Infrastructure layer. You must strictly separate Read contracts from Read/Write contracts.

Entry Points: Each module is accessed via a specific interface and its implementation. Module methods must be prefixed with `Get`, `Add`, `Set`, or `Remove`.

Mapping: Business Models are responsible for mapping Data Models into Business Models.

Dependency Rule: Data Models must remain "pure"; they should not depend on, or have knowledge of, Business Logic or Entities.

## ClientApp Project

This is the UI layer.

* Components: Razor UI components must exclusively use ViewModels located in the Models folder. They also must be suffixed by `Component`.

Separation of Concerns: Razor markup, C# code-behind, and CSS must be kept in separate files. Razor code should only handle state and events.

* Controllers: All other logic is handled by them. Each view is managed by a controller responsible for mapping Business Models into ViewModels and non business logic such as sorting, ordering, mapping back to business model when submitting from the UI.

Controllers must follow REST Conventions especially for the Urls and its methods must be named `Index`, `Show`, `Store`, or `Destroy`.