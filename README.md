# Nordic Ember API

A REST API for **Nordic Ember**, a web shop for coffee. Built as the project assignment in *Backend-utveckling 2* with ASP.NET Core Minimal API, Entity Framework Core and SQL Server.

Products are organised in **categories**, and categories are grouped in **facets** – the questions a customer filters on (roast level, origin, brew method, form, labelling). A product can belong to many categories.

## Tech stack

- .NET 10 / ASP.NET Core Minimal API
- Entity Framework Core 10 with SQL Server (LocalDB)
- OpenAPI with Scalar for documentation
- Built-in validation (`AddValidation`) with DataAnnotations
- ProblemDetails for error responses

## Getting started

### Prerequisites

- .NET 10 SDK
- SQL Server LocalDB (included with Visual Studio)
- Visual Studio 2026 (or another editor plus the `dotnet-ef` tool)

### Run the project

1. Open `NordicEmber.slnx` in Visual Studio.
2. Check the connection string in `NordicEmber.Api/appsettings.Development.json`. By default it points to `(localdb)\MSSQLLocalDB` and the database `NordicEmberDb`.
3. Create the database. In **Package Manager Console**:
   ```
   Update-Database
   ```
   This runs the migrations and seeds the database with 5 facets, 16 categories and 10 products.
4. Start the project with the **https** profile (F5).
5. Open the documentation at **https://localhost:8000/scalar**.

### Reset the database

```
Drop-Database
Update-Database
```

Seeding only runs on an empty database, so dropping it restores the original data.

## Endpoints

### Products

| Method | Route | Description | Responses |
|---|---|---|---|
| GET | `/api/products` | All products, sorted by name | 200 |
| GET | `/api/products?slug={slug}` | Product by URL slug, as a list with 0 or 1 item | 200 |
| GET | `/api/products/{id}` | Product by ID | 200, 404 |
| POST | `/api/products` | Create a product | 201, 400, 409 |
| DELETE | `/api/products/{id}` | Delete a product | 204, 404 |

### Categories

| Method | Route | Description | Responses |
|---|---|---|---|
| GET | `/api/categories` | All categories with their products and facet, ordered by facet and name | 200 |
| GET | `/api/categories?slug={slug}` | Category by URL slug, as a list with 0 or 1 item | 200 |
| GET | `/api/categories/{id}` | Category by ID | 200, 404 |
| POST | `/api/categories` | Create a category, optionally in a facet (`facetId`) | 201, 400, 409 |
| DELETE | `/api/categories/{id}` | Delete a category. Its products remain, only the links are removed. | 204, 404 |

### Example: create a product

```http
POST /api/products
Content-Type: application/json

{
  "name": "Rwanda Nyungwe",
  "description": "Saftig med toner av röda bär och mörk choklad.",
  "price": 155,
  "image": "/images/products/rwanda-nyungwe.png"
}
```

The URL slug is generated from the name (`rwanda-nyungwe`). The response is `201 Created` with a `Location` header pointing to the new product.

## Error handling

- **400 Bad Request** – validation errors, returned as ProblemDetails with the invalid field and a message.
- **409 Conflict** – a product or category with the same slug already exists. "Kenya AA" and "kenya-aa" count as the same.
- **404 Not Found** – the resource does not exist. Empty body.

Error messages in `errors` are in Swedish, since they are meant to be shown to customers. Everything else in the API is in English.

## Project structure

```
NordicEmber.Api/
  Common/           Shared code: SlugGenerator and the Result pattern
  Data/
    Entities/       Classes mapped to database tables
    Configurations/ EF Core configuration per entity (lengths, indexes, relations)
    Migrations/     EF Core migrations
    AppDbContext.cs
    DbSeeder.cs     Seed data
  Features/
    Products/       DTOs, mapping, service and endpoints for products
    Categories/     DTOs, mapping, service and endpoints for categories
    Facets/         DTOs for facets
  Program.cs        Wires everything together
```

The code is organised by feature: everything that belongs to products lives in one folder.

## Design decisions

- **Entities are never exposed.** The API only returns DTOs.
- **One source of truth.** Max lengths and price limits are constants on the entities, used by both the database configuration and the validation.
- **Unique slugs** are checked in the service and guaranteed by a unique index in the database.
- **Mapping as expressions**, so EF Core translates them to SQL and only fetches the columns that are needed.
- **Seed data in code** (`UseSeeding` / `UseAsyncSeeding`), so the database looks the same on every machine.
- **Facets** are an extension of the assignment. Each facet has a sort order and a rule (`AllowsMultiple`) for whether a product may have more than one category in it.
