# VIA Assurance ERP

VIA Assurance ERP est une application métier construite pour centraliser la gestion opérationnelle d'une entreprise d'assurance : suivi des entreprises clientes, gestion des employés, souscriptions, paie, actifs assurables, sinistres et documents contractuels.

L'objectif du projet est simple : **regrouper dans une seule plateforme les opérations administratives, RH et assurance**, avec une séparation claire entre l'interface utilisateur, la logique métier et l'accès aux données.

## Ce que fait concrètement l'application

Le code source couvre plusieurs domaines fonctionnels :

- **Gestion des entreprises** : récupération des informations d'une entreprise cliente
- **Gestion des employés** : création, consultation, mise à jour, activation/désactivation et rattachement à des projets
- **Documents employés** : dépôt et consultation de documents liés aux collaborateurs
- **Souscriptions** : gestion des abonnements ou contrats d'assurance par employé et par entreprise
- **Paie** : paramétrage de paie, périodes de paie, brouillons de bulletins, recalculs et validation
- **Actifs assurables** :
  - flotte de véhicules
  - entrepôts et matériels stockés
  - transport de marchandises
- **Documents d'entreprise** : génération, signature et archivage de confirmations d'assurance
- **Sinistres** : déclaration, typologie, suivi des statuts, pièces jointes et traitement des dossiers
- **Services transverses** : authentification, envoi d'e-mails, localisation FR/EN et exposition d'outils MCP

En résumé, ce dépôt porte un **ERP orienté assurance**, avec une forte composante documentaire et métier.

## Stack technique

- **.NET 10**
- **Blazor Server** pour l'interface utilisateur
- **ASP.NET Core** pour l'hébergement, l'authentification et les endpoints
- **SQL Server** via `SqlConnection`
- **Dapper** pour les accès aux données
- **QuestPDF / IronPdf** pour la génération documentaire
- **ASP.NET Identity** pour la gestion des utilisateurs

## Architecture employée

Le projet suit une **Clean Architecture modulaire**.

L'idée directrice est la suivante :

1. **la couche UI affiche et orchestre les interactions**
2. **les modules encapsulent la logique métier**
3. **l'infrastructure réalise les accès techniques et la persistance**

Cette séparation permet de faire évoluer le métier sans mélanger l'interface, les règles métiers et le code d'accès à la base.

---

## Vue d'ensemble des couches

### 1. `ClientApp` — la couche présentation

`ClientApp` est l'application frontale construite avec Blazor.

Elle contient :

- les **composants Razor** dans `Components`
- les **contrôleurs UI** dans `Controllers`
- les **ViewModels** dans `Models`
- les **services applicatifs de présentation**
- la **configuration de l'application** et l'enregistrement des dépendances dans `Program.cs`

#### Rôle de cette couche

Cette couche est responsable de :

- afficher les écrans
- gérer la navigation
- déclencher les actions utilisateur
- mapper les objets métier vers des ViewModels destinés à l'interface
- orchestrer des cas d'usage via les modules métier

#### Ce qu'on y observe dans le code

- des pages comme :
  - `/list-employees`
  - `/employee-documents`
  - `/payroll`
  - `/company-documents`
  - `/fleet-list`
  - `/warehouse-list`
  - `/transportation-list`
  - `/list-sinisters`
- des contrôleurs UI spécialisés comme :
  - `EmployeeController`
  - `PayrollController`
  - `CompanyDocumentsController`
  - `FleetController`
  - `WarehouseController`
  - `TransportationController`
  - `SinisterListController`

#### Principe architectural

La couche UI **ne porte pas la logique métier profonde**.  
Elle s'appuie sur les interfaces des modules pour exécuter les cas d'usage et sur des ViewModels pour garder une interface claire.

---

### 2. Les `*.Module` — la couche métier

Chaque dossier `*.Module` représente un **module métier autonome**.  
Exemples :

- `Company.Module`
- `Employee.Module`
- `Subscription.Module`
- `CompanyPayroll.Module`
- `EmployeePayroll.Module`
- `PaySlip.Module`
- `Company.Fleet.Module`
- `Company.Warehouse.Module`
- `Company.Transportation.Module`
- `CompanyDocuments.Module`
- `Sinister.Module`
- `Company.Sinister.Module`
- `CompanySinisterDocument.Module`

#### Responsabilité

Les modules contiennent :

- les **règles métier**
- les **modèles métier** (`Business`)
- les **contrats d'accès aux données** (`Data/Providers`)
- les **points d'entrée métier** via des interfaces du type `IEmployeeModule`, `IPaySlipModule`, `ICompanyDocumentModule`, etc.

#### Organisation interne

Chaque module est découpé en général en :

- `Business` : objets et logique métier
- `Data/Models` : modèles de données
- `Data/Providers` : contrats lus par le métier et implémentés par l'infrastructure

#### Exemples de responsabilités métier

- `Employee.Module` : gestion du cycle de vie d'un employé
- `Subscription.Module` : gestion des souscriptions d'assurance
- `CompanyPayroll.Module` : paramètres de paie de l'entreprise
- `EmployeePayroll.Module` : périodes de paie et historique
- `PaySlip.Module` : génération, sauvegarde et recalcul des bulletins
- `CompanyDocuments.Module` : génération, signature et archivage des documents d'assurance
- `Company.Sinister.Module` : gestion des dossiers sinistres

#### Pourquoi cette couche est importante

C'est ici que le projet exprime sa vraie valeur :  
**les cas d'usage métiers sont isolés du stockage et de l'UI**.

---

### 3. `FileTable.Infrastructure` — la couche infrastructure

Cette couche contient les détails techniques nécessaires au fonctionnement de l'application.

On y trouve notamment :

- `FileTableDbContext` pour créer les connexions SQL
- `FileTableDb/DataProviders` pour les implémentations concrètes des contrats de lecture/écriture
- `FileTableDb/Entities` pour les entités miroir des tables
- `FileTableDb/Migrations` pour les scripts de migration
- `Identities` pour l'intégration avec ASP.NET Identity
- `Services` pour les services techniques comme l'e-mail

#### Rôle de cette couche

Elle est responsable de :

- exécuter les opérations CRUD
- dialoguer avec SQL Server
- mapper les données de la base vers les modèles manipulés par le domaine
- gérer la persistance documentaire
- encapsuler les détails techniques que le métier ne doit pas connaître

#### Data providers

Le découpage en providers distingue clairement :

- la **lecture seule** (`ReadOnly`)
- la **lecture/écriture** (`ReadWrite`)

Exemples présents dans le dépôt :

- `EmployeeReadOnly` / `EmployeeReadWrite`
- `CompanyDocumentReadOnly` / `CompanyDocumentReadWrite`
- `CompanySinisterReadOnly` / `CompanySinisterReadWrite`
- `PayrollPeriodReadOnly` / `PayrollPeriodReadWrite`

Ce choix rend l'intention du code explicite et colle bien au découpage Clean Architecture.

---

### 4. `FileTable.Infrastructure.Abstractions` — les abstractions techniques transverses

Ce projet contient les contrats techniques partagés, par exemple :

- `ITransactionHandler`
- `ITransactionDetector`

Ils permettent aux modules métier d'exprimer un besoin transactionnel **sans dépendre d'une implémentation infrastructure concrète**.

Concrètement, lorsqu'un cas d'usage doit enchaîner plusieurs écritures cohérentes, le module peut demander une exécution transactionnelle via ces abstractions. C'est ensuite l'infrastructure qui se charge du mécanisme réel côté base de données, par exemple pour la paie, les souscriptions ou les sinistres.

---

## Comment circulent les données

Le flux principal ressemble à ceci :

1. un utilisateur interagit avec une page Blazor
2. le composant appelle un contrôleur de `ClientApp`
3. le contrôleur invoque un module métier
4. le module utilise des interfaces de providers définies dans sa couche `Data`
5. l'infrastructure implémente ces interfaces et interroge SQL Server
6. les données remontent vers le module, puis vers le contrôleur, puis vers la vue

Ce flux garantit :

- une meilleure lisibilité
- une séparation nette des responsabilités
- une testabilité plus simple du métier
- une maintenance plus sereine quand les besoins évoluent

## Découpage fonctionnel du dépôt

### Gestion RH

- entreprises
- employés
- documents employés
- rattachement à des projets

### Assurance

- souscriptions
- police / confirmation d'assurance
- actifs assurables
- documents contractuels
- suivi des sinistres

### Paie

- paramètres de paie entreprise
- périodes de paie
- bulletins
- demandes de modification
- recalculs et historisation

### Services supports

- authentification et gestion utilisateurs
- e-mail
- localisation multilingue
- outils MCP

## Pourquoi cette architecture est pertinente ici

Ce type d'application mélange :

- des écrans nombreux
- plusieurs sous-domaines métier
- de la persistance relationnelle
- de la génération documentaire
- des workflows sensibles comme la paie ou les sinistres

Une architecture monolithique sans séparation aurait rapidement rendu le projet difficile à faire évoluer.  
Le choix d'une **Clean Architecture modulaire** apporte ici :

- une **meilleure isolation des responsabilités**
- une **évolutivité par domaine métier**
- une **infrastructure remplaçable**
- une **UI plus simple à maintenir**
- une **base saine pour faire grandir l'application**

## Structure du dépôt

```text
ClientApp/                           Interface utilisateur Blazor
FileTable.Infrastructure/            Accès SQL, providers, identité, services techniques
FileTable.Infrastructure.Abstractions/ Abstractions transverses
*.Module/                            Domaines métier et contrats de données
FileTableViewer.sln                  Solution .NET principale
```

## En bref

VIA Assurance ERP est un **ERP assurance modulaire** qui centralise :

- les opérations RH
- les souscriptions
- la paie
- les actifs assurés
- les sinistres
- les documents métier

Le dépôt est structuré autour d'une **Clean Architecture en couches**, dans laquelle :

- **`ClientApp`** pilote l'expérience utilisateur
- **les modules** portent le métier
- **`FileTable.Infrastructure`** exécute la technique et la persistance

Cette organisation rend le projet plus clair, plus maintenable et plus durable à long terme.
