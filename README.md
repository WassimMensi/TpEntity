# TpHopital - Système de Gestion Hospitalière

Application ASP.NET Core 10 avec Entity Framework Core et SQLite.

## Prérequis

- .NET 10 SDK
- Aucune installation de base de données nécessaire (SQLite inclus)

## Lancer le projet

```bash
# Après avoir cloner le dépôt
cd TpHopital

# Restaurer les dépendances
dotnet restore

# Appliquer les migrations et lancer l'API
cd api
dotnet run
```

La documentation Swagger est disponible en ajoutant `/swagger` à l'URL de l'API.

La base de données est créée automatiquement au premier démarrage. Les données de test (3 départements, 3 médecins, 5 patients, 8 consultations) sont injectées au démarrage via `DataSeeder`.

## Lancer les tests

```bash
cd HospitalManagement.Tests
dotnet test
```

## Architecture

Le projet suit une **Clean Architecture** en 3 couches :

```
TpHopital/
├── Domain/              # Entités, interfaces, DTOs, ValueObjects
├── Infrastructure/      # DbContext, migrations, repositories, services
├── api/                 # Contrôleurs REST, Program.cs
└── HospitalManagement.Tests/  # Tests unitaires (xUnit + Moq)
```

### Domain
Contient les interfaces et les entités métier. Aucune dépendance vers EF Core ou la couche infra.

### Infrastructure
Implémente les interfaces du Domain. Contient le `HospitalDbContext`, les repositories et les services métier.

### API
Expose les endpoints REST. Délègue toute la logique aux services via injection de dépendances.

## Entités et relations

| Entité | Rôle |
|--------|------|
| `Patient` | Informations personnelles + numéro de dossier unique |
| `Doctor` | Médecin rattaché à un département |
| `Department` | Département avec responsable et sous-départements |
| `Consultation` | Lien entre un patient et un médecin à une date |
| `Staff` | Classe de base (TPH) pour MedicalDoctor, Nurse, AdminStaff |
| `Address` | Value object intégré dans Patient et Department |

**Relations principales :**
- `Patient` → `Consultation` : One-to-Many
- `Doctor` → `Consultation` : One-to-Many
- `Doctor` → `Department` : Many-to-One (DeleteBehavior.Restrict)
- `Department` → `Department` : auto-référence (sous-départements)
- `Department` → `Doctor` : One-to-One optionnel (DeleteBehavior.SetNull)
- `Staff` : héritage TPH (MedicalDoctor, Nurse, AdminStaff)

## Migrations (5)

| Migration | Contenu |
|-----------|---------|
| `InitialCreate` | Tables Patient, Department, Doctor, Consultation |
| `AddDoctorAndDepartmentHead` | Relation chef de département, auto-référence |
| `AddConsultation` | Affinements de la table Consultation |
| `AdvancedModeling` | Héritage TPH Staff, value object Address |
| `PerformanceAndConcurrency` | Index composites, RowVersion (concurrence optimiste) |

## Endpoints principaux

**Patients**
- `GET /api/patients` — liste paginée
- `GET /api/patients/search?name=...` — recherche par nom
- `POST /api/patients` — créer un patient
- `PUT /api/patients/{id}` — modifier un patient
- `DELETE /api/patients/{id}` — supprimer (bloqué si consultations existantes)

**Consultations**
- `POST /api/consultations` — planifier
- `PUT /api/consultations/{id}/status` — changer le statut
- `PUT /api/consultations/{id}/cancel` — annuler
- `GET /api/consultations/patient/{id}/upcoming` — prochaines consultations d'un patient
- `GET /api/consultations/doctor/{id}/today` — planning du jour d'un médecin

**Tableau de bord**
- `GET /api/dashboard/patients/{id}` — fiche patient complète
- `GET /api/dashboard/doctors/{id}` — planning médecin
- `GET /api/dashboard/departments` — statistiques par département

## Choix techniques

- **SQLite** : simple à déployer, pas de serveur requis, adapté au TP.
- **DeleteBehavior.Restrict** sur Doctor→Department : on ne peut pas supprimer un département qui a des médecins, ce qui évite les orphelins.
- **DeleteBehavior.SetNull** sur HeadDoctor : le département peut exister sans responsable.
- **RowVersion** sur Patient : détection des conflits en cas de modification simultanée (concurrence optimiste).
- **AsNoTracking()** sur toutes les lectures : réduit la consommation mémoire pour les requêtes sans écriture.
- **Repository Pattern + Service Layer** : la logique métier est dans les services, les contrôleurs ne font que router les appels.
