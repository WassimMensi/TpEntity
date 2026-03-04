## Quels sont les avantages et inconvénients de votre modèle ?

### Avantages

**Modélisation fidèle au métier.** Chaque concept du cahier des charges (patient, médecin, département, consultation) est une entité distincte. Les relations reflètent exactement la réalité : un médecin appartient à un département, une consultation lie un patient à un médecin.

**Intégrité des données garantie.**
- Le numéro de dossier et l'email sont uniques via des index de base de données.
- `DeleteBehavior.Restrict` empêche de supprimer un département qui a encore des médecins.
- Une contrainte unique composite sur `(PatientId, DoctorId, Date)` empêche les doublons de consultation.

**Value Object Address.** L'adresse est modélisée comme un type intégré (Owned Type EF Core), partagé entre `Patient` et `Department`. Cela évite la duplication de colonnes sans créer de table supplémentaire.

**Héritage TPH pour le personnel.** Toutes les catégories de personnel (médecin, infirmier, administratif) partagent une seule table `Staff` avec un discriminateur. Simple à requêter, pas de jointures supplémentaires.

**Concurrence gérée.** Le `RowVersion` sur `Patient` détecte les modifications simultanées et renvoie une erreur claire plutôt que d'écraser silencieusement des données.

**Architecture testable.** La séparation Domain / Infrastructure / API et l'utilisation d'interfaces permettent de tester la logique métier avec une base de données en mémoire sans modifier le code de production.

### Inconvénients

**TPH peut devenir inefficace.** Si le nombre de types de personnel augmente, la table `Staff` accumulera des colonnes nullable pour chaque type. Une stratégie TPT (table par type) serait plus propre pour plus de 5 types.

**Utilisation de SQLite.** SQLite convient au développement et aux tests mais ne supporte pas les accès concurrents élevés ni certaines fonctionnalités avancées (ex. : procédures stockées, types JSON natifs). Il faudrait migrer vers PostgreSQL ou SQL Server en production.

**Pas d'historique des modifications.** Si un médecin change de département, l'ancienne affectation est perdue. Un système de suivi médical réel aurait besoin d'un audit trail.

---

## Quelles optimisations feriez-vous si l'hôpital avait 100 000 patients ?

**Migrer vers PostgreSQL ou SQL Server.** SQLite ne tient pas la charge avec de nombreux accès simultanés. PostgreSQL gère nativement la concurrence et offre de meilleures performances sur les gros volumes.

**Pagination systématique.** Toutes les listes doivent retourner des pages de 20-50 éléments maximum. C'est déjà en place mais il faudrait l'imposer côté API sans possibilité de tout charger.

**Cache applicatif.** Les données peu changeantes (liste des départements, médecins) peuvent être mises en cache pour éviter des requêtes répétées sur des données stables.

**Archivage des vieilles consultations.** Les consultations de plus de 5 ans pourraient être déplacées dans une table d'archive. Cela réduit la taille de la table active et accélère les requêtes courantes.

---

## Comment implémenteriez-vous un système de rendez-vous en ligne ?

**Nouvelle entité `Appointment`** (distinct de `Consultation`).
Un rendez-vous est une demande en attente de confirmation. Une fois confirmé par le secrétariat, il devient une `Consultation`. L'entité aurait : `PatientId`, `DoctorId`, `RequestedDate`, `Status` (Pending / Confirmed / Rejected), `CreatedAt`.

**Créneaux disponibles.** Ajouter une entité `TimeSlot` liée au médecin : `DoctorId`, `StartTime`, `EndTime`, `IsAvailable`. Le patient choisit un créneau libre. À la confirmation, le créneau passe à `IsAvailable = false`.

**Authentification patient.** Les patients doivent s'authentifier pour prendre un rendez-vous. Ajouter une entité `PatientAccount` avec email/mot de passe hashé, liée à `Patient` en One-to-One.

---

## Quel impact sur le modèle si on ajoutait la facturation ?

**Nouvelle entité `Invoice` (Facture).** Liée à une `Consultation` en One-to-One. Propriétés : `ConsultationId`, `Amount`, `IssuedAt`, `Status` (Draft / Sent / Paid / Cancelled), `DueDate`.

**Nouvelle entité `InvoiceLine`.** Une facture peut contenir plusieurs lignes (consultation + actes médicaux + médicaments). Propriétés : `InvoiceId`, `Description`, `Quantity`, `UnitPrice`.

**Nouveau module `Act` (acte médical).** Une consultation peut générer plusieurs actes facturables (radio, analyse, consultation spécialisée). `Consultation` → `Act` : One-to-Many. Chaque acte a un tarif de référence.

**Lien avec le patient.** Le patient doit avoir des informations de facturation : numéro de sécurité sociale, mutuelle, taux de remboursement. Ces données peuvent être ajoutées directement sur `Patient` ou dans une entité `PatientInsurance` séparée.

**Impact sur les suppressions.** Si une facture est émise, la consultation et le patient ne doivent plus pouvoir être supprimés. Il faudrait ajouter des vérifications dans les services (comme pour les consultations existantes lors de la suppression d'un patient).

**Impact sur les migrations.** Il faudrait 1 nouvelles migrations : une pour les tables `Invoice` et `InvoiceLine`, une pour les colonnes de mutuelle sur `Patient`.
