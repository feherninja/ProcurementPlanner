# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

This is a **.NET MAUI Blazor Hybrid** app targeting **Windows only** (`net9.0-windows10.0.19041.0`, `win10-x64`). It is unpackaged (`WindowsPackageType=None`).

```bash
# Build
dotnet build DeliveryManager/DeliveryManager.csproj

# Run
dotnet run --project DeliveryManager/DeliveryManager.csproj

# Add a new EF Core migration (run from the DeliveryManager/ project directory)
dotnet ef migrations add <MigrationName> --project DeliveryManager.csproj

# Apply pending migrations manually (also happens automatically on startup)
dotnet ef database update --project DeliveryManager.csproj
```

The SQLite database file (`delivery.db`) is placed next to the executable at runtime. Migrations are applied automatically on startup via `context.Database.Migrate()` in `MauiProgram.cs`.

## Architecture

### Layers
| Layer | What lives there |
|---|---|
| **MAUI Shell** | `App.xaml`, `MauiProgram.cs`, platform bootstrapping |
| **UI** | Razor pages, dialog components, layout, `wwwroot/` |
| **Service** | Interfaces + implementations, document generators, `DeliveryPlanningService` |
| **Domain Model** | Entity classes (`Models/`), EF Fluent configs (`Db/Configuration/`) |
| **Data Access** | `DeliveryDbContext`, `DeliveryDbContextFactory`, `Migrations/` |
| **Platform Config** | `.csproj`, manifests, per-platform packaging descriptors |

### Stack
- **MAUI** shell — provides the native window (launched maximized on Windows)
- **Blazor WebView** — all UI is Razor components rendered inside the MAUI window
- **MudBlazor 8** — UI component library (tables, dialogs, snackbars, icons)
- **QuestPDF** (Community) — PDF generation for purchase order documents
- **EF Core 9 + SQLite** — persistence, code-first, migrations in `Migrations/`

### Data model
```
Company
  └── Item (many, via CompanyId)
        └── ItemComponent (join, with Quantity)
              └── Component (many, via CompanyId)
  └── Component (many, via CompanyId)

Delivery
  └── DeliveryItem → Item
```

Key fields to know:
- `Component.PartNumber` = internal company part number (Céges termékszám)
- `Component.ExternalNumber` = supplier part number (Schwarzmüller-Nr.)
- `Component.OrderPackageSize` = minimum order increment (ceiling-division used for order qty)
- `Component.Stock` / `Item.Price` / `Component.Price` are `int`

`DeliveryDbContext.UpdateTimestamps()` auto-sets `CreatedAt` on insert and `UpdatedAt` on update for any entity that has those properties — no manual assignment needed except in services that call `UpdateAsync`.

### Company scoping
All data is scoped to a `Company`. `CompanyContext` (scoped DI singleton) holds `ActiveCompanyId` and is shared across pages so switching companies on one page propagates everywhere. Default company is id=1.

### Service layer
| Interface | Implementation | Notes |
|---|---|---|
| `IComponentService` | `ComponentService` | Eager-loads `ItemComponents → Item`; `UpdateStockLevelsAsync` applies order quantities |
| `IItemService` | `ItemService` | Eager-loads `ItemComponents → Component` |
| `ICompanyService` | `CompanyService` | Simple CRUD |
| `IDeliveryDocumentService` | `DeliveryDocumentService` | Delivery note PDF (QuestPDF) |
| `IPurchaseOrderDocumentService` | `PurchaseOrderDocumentService` | Purchase order PDF (German column headers: Bezeichnung / Artikel-Nr. / Schwarzmüller-Nr.) |
| `DeliveryPlanningService` | _(no interface, scoped)_ | Holds transient planning state: selected items, required components, company, date, order number |

### Pages & routing
| Route | Page | Purpose |
|---|---|---|
| `/companies` | `Companies.razor` | CRUD for companies |
| `/components` | `Components.razor` | CRUD for components; drawer shows which items use each component |
| `/items` | `Items.razor` | CRUD for items; drawer shows component BOM per item |
| `/order-planner` | `OrderPlanner.razor` | Build an order: select items → calculates required components → generates purchase order PDF |

`OrderPlanner.razor` is split into `OrderPlanner.razor` (markup) and `OrderPlannerBase` in `OrderPlanner.razor.cs` (logic). The nested classes `OrderPlannerBase.SelectedItem` and `OrderPlannerBase.RequiredComponent` are reused by `DeliveryPlanningService` and `ComponentService`.

### Order planning logic (`RequiredComponent`)
- `CalculateRequiredOrderQuantity()` — returns number of **packages** to order (ceiling division of deficit by `OrderPackageSize`); returns 0 if stock is sufficient
- `CalculateNewStock()` — new stock after receiving ordered packages and shipping required quantity
- After PDF is saved successfully, `ApplyDelivery()` calls `ComponentService.UpdateStockLevelsAsync()` to commit the new stock levels

### Dialog pattern
All dialogs: Razor markup file + code-behind `.razor.cs`, opened via `IDialogService.ShowAsync<TDialog>(...)` with `DialogParameters`, receive `[CascadingParameter] IMudDialogInstance MudDialog`.

### Dialogs — two patterns
| Dialog | Purpose |
|---|---|
| `CompanyDialog`, `ComponentDialog`, `ItemDialog` | Create/edit a single entity; return the saved entity via `DialogResult.Ok(entity)` |
| `ItemSelectionDialog` | Multi-select items with quantities for the order planner; returns `List<SelectedItem>` |
| `ComponentSelectionDialog` | Add components to an item's BOM from the Items page; filters out already-linked components; persists immediately via `ItemService.AddComponentToItemAsync` |

### EF Configuration
Entity configurations live in `Db/Configuration/` (one file per entity, implementing `IEntityTypeConfiguration<T>`), applied via `modelBuilder.ApplyConfigurationsFromAssembly(...)`.

`DeliveryDbContextFactory` (`Db/DeliveryDbContextFactory.cs`) implements `IDesignTimeDbContextFactory` so `dotnet ef` CLI can create a context without the MAUI host. It has a **hardcoded dev path** (`f:\_databases\DeliveryManager\delivery.db`) that must be updated when switching machines.

### Dev tooling
`Db/export.ps1` and `Models/export.ps1` are PowerShell scripts that concatenate all `.cs` files in their respective folders into a single `dump.txt` — useful for pasting context into AI tools. Run from the folder containing the script:
```powershell
.\export.ps1              # outputs dump.txt
.\export.ps1 -OutputFile out.txt
```
