# CONTRIBUTING

Project coding standards:
- Follow .editorconfig for formatting and naming.
- Use explicit attribute `[DatabaseGenerated(DatabaseGeneratedOption.Identity)]` for primary key Id properties in domain entities so SQLite autoincrement is explicit.
- Keep migrations in `Migrations` namespace and ensure they are compiled into the project.
