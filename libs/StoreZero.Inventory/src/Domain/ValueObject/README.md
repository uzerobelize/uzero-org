
---

Inventory Domain: Value Objects
===============================

Introduction
------------

This document provides a reference for the Value Objects defined and utilized within the Inventory domain of the online retail store system. Following Domain-Driven Design (DDD) principles, these Value Objects are fundamental building blocks, encapsulating concepts that are defined by their attributes rather than a unique identity.

Value Objects play a critical role in ensuring the validity, consistency, and expressiveness of the Inventory domain model by:

1.  **Enforcing Invariants:** Containing validation logic to ensure they only represent valid states according to business rules (e.g., a non-negative Quantity, a correctly formatted Sku, a valid UnitOfMeasure from a predefined set).
2.  **Preventing Primitive Obsession:** Replacing primitive types (like string, int, decimal, bool) with domain-specific types that carry inherent meaning, context, and constraints (e.g., using an Sku type instead of just string).
3.  **Ensuring Immutability:** Guaranteeing that once a Value Object instance is created, its state cannot change, which simplifies reasoning about the system state and promotes predictability.
4.  **Improving Clarity:** Making the domain model more self-documenting and easier to understand by explicitly modeling concepts drawn directly from the Ubiquitous Language of inventory management.

Within the Inventory domain, Value Objects typically represent concepts such as:

*   Unique identifiers for sellable units (`Sku`)
*   Quantities and measurements (`Quantity`, `Weight`, `Dimensions`, `UnitOfMeasure`, `UnitValue`)
*   Specific status flags with domain meaning (`StorageRequirement`, `IsPerishable`, `TaxClass`)
*   Standardized external codes (`BarcodeUpc`)
*   Timestamp markers (`CreatedAt`, `UpdatedAt`)
*   Domain-specific codes or references (`CountryOfOrigin`, `BrandId`, `ManufacturerId`, `CategoryId`, `DistributorId`)

This README serves as a guide to these essential Value Objects, detailing their specific purpose, the attributes they encapsulate, and the constraints they enforce within the Inventory domain. Understanding these Value Objects is key to correctly interacting with and extending the domain model.

* * * * *

Attributes of ValidatedProduct:
-------------------------------

*(Note: Many attributes belong conceptually to the `ProductVariant` Aggregate Root (representing a specific SKU), while others belong to the parent `Product` Aggregate Root. This distinction should be clarified in the final domain model implementation.)*

### Identifiers (Internal & External)

*   **ProductId (`ProductId.T` Value Object):**
    *   **Description:** Unique Identifier for the core product concept/aggregate root (internal system ID). Serves as the anchor for the aggregate.
    *   **Purpose:** Aggregate Identity, Uniqueness, Referenceability, Data Integrity, System Operations, Lifecycle Tracking.
    *   **Placement:** Root identifier for the `Product` Aggregate. Referenced by `ProductVariant`s.
    *   **Constraints:** Must be a valid, non-empty `System.Guid`. Immutable. System-generated. Uniqueness across all products enforced by system/DB. Mandatory.
*   **Sku (`Sku.T` Value Object):**
    *   **Description:** Stock Keeping Unit - the unique internal identifier for a specific, sellable variant used for tracking inventory.
    *   **Purpose:** Granular Inventory Tracking, Accurate Order Fulfillment, Variant-Specific Pricing/Promotions, Sales Analysis, Disambiguation.
    *   **Placement:** `ProductVariant` Aggregate (identifies the specific variant).
    *   **Constraints:** Must not be null or whitespace. Must be between 8 and 15 characters long (configurable). Value is treated as case-insensitive (typically stored uppercase). Specific character format rules enforced by VO factory. Uniqueness across all active variants enforced by system/DB. Mandatory. Immutable.
*   **BarcodeUpc (`BarcodeUpc.T` Value Object):**
    *   **Description:** Standardized 12-digit Universal Product Code or EAN (external identifier, often found on packaging).
    *   **Purpose:** Retail POS scanning, Inventory Receiving, Data Integration (marketplaces), Supply Chain Tracking, Compliance.
    *   **Placement:** `ProductVariant` Aggregate (as variants can sometimes have unique UPCs).
    *   **Constraints:** Optional (nullable). If present: must be exactly 12 digits, numeric only, preserve leading zeros (stored as string). Check digit validation is highly recommended within the VO factory. Immutable. Should generally not identify two different base products.

### Descriptive Content

*   **Name (`ProductName.T` Value Object):**
    *   **Description:** Primary, human-readable title identifying the core product concept. Crucial for SEO and user identification.
    *   **Purpose:** Findability, Identification, User Experience, Communication, First Impression.
    *   **Placement:** `Product` Aggregate Root.
    *   **Constraints:** Must not be null or whitespace. Length between 40 - 80 characters (configurable for display/SEO, storage allows more). Allows standard characters, disallows control characters. Immutable VO (updates replace). Uniqueness is a guideline, not a strict constraint. Mandatory.
*   **ShortDescription (`ProductDescription.T` Value Object):**
    *   **Description:** Concise marketing "hook" or summary highlighting key benefits/features for listings and quick views.
    *   **Purpose:** Quick Value Proposition, Engagement, Highlight Key Features, Space Efficiency, Differentiation.
    *   **Placement:** `Product` Aggregate Root.
    *   **Constraints:** Optional (can be empty/null). If present, must not be whitespace. Max length significantly shorter than LongDescription (e.g., effective display 100-250 chars, storage 250-500 chars). Immutable VO.
*   **LongDescription (`ProductDescription.T` Value Object):**
    *   **Description:** Comprehensive, detailed description for the Product Detail Page (PDP), including features, benefits, specs, usage, etc.
    *   **Purpose:** Informed Purchase Decisions, Reduces Returns/Support, Builds Trust, SEO Content, Brand Storytelling, Comparison.
    *   **Placement:** `Product` Aggregate Root.
    *   **Constraints:** Optional (can be empty/null). If present, must not be whitespace. Max length limited by database large text type (`NVARCHAR(MAX)`/`TEXT`). Structure/readability (headings, lists) crucial for usability. Immutable VO.

### Classification & Sourcing

*   **Brand (`BrandName.T` Value Object / `BrandId` Reference):**
    *   **Description:** The marketing identity/brand under which the product is sold. Typically references a separate Brand entity via `BrandId`.
    *   **Purpose:** Product Identification/Recognition, Filtering/Navigation, Data Consistency, Reporting, Merchandising, Trust & Loyalty.
    *   **Placement:** `Product` Aggregate Root (holds `BrandId` reference).
    *   **Constraints:** Reference (`BrandId`) is Optional (nullable). If present, must point to a valid Brand entity (referential integrity via FK). `BrandName` itself (in Brand entity) has constraints (Unique, Not Null, Length). `BrandId` VO is immutable.
*   **Manufacturer (`ManufacturerName.T` Value Object / `ManufacturerId` Reference):**
    *   **Description:** The company that physically produced the goods. Can differ from the Brand. Typically references a separate Manufacturer entity via `ManufacturerId`.
    *   **Purpose:** Supply Chain Traceability, Warranty/Support, Regulatory Compliance, Internal Reporting, Authenticity.
    *   **Placement:** `ProductVariant` Aggregate (holds `ManufacturerId` reference, as it can vary).
    *   **Constraints:** Reference (`ManufacturerId`) is Optional (nullable). If present, must point to a valid Manufacturer entity (referential integrity via FK). `ManufacturerName` itself has constraints. `ManufacturerId` VO is immutable.
*   **Category (`ProductCategory.T` Value Object / `CategoryId` Reference Collection):**
    *   **Description:** Primary classification(s) of the product within a hierarchical structure. Managed via references to Category entities.
    *   **Purpose:** Navigation/Discovery, Organization, Targeted Marketing, Reporting, Attribute Management, SEO.
    *   **Placement:** `Product` Aggregate Root (holds collection of `CategoryId` references).
    *   **Constraints:** Collection can be empty. Each `CategoryId` must reference a valid Category entity (referential integrity via FK in mapping table). No duplicate `CategoryId`s per product. `CategoryId` VO is immutable.
*   **SubCategory (`string` / Handled by Category Hierarchy):**
    *   **Description:** More specific categorization within the main Category structure. Represented by assigning a Product to a `Category` entity that has a `ParentCategoryID`. Not a separate field.
    *   **Constraints:** Governed by the constraints of the `Category` entity hierarchy (referential integrity, acyclicity).
*   **Distributors (`DistributorId` Reference Collection / `SourcingInfo` VO):**
    *   **Description:** Set of external suppliers/distributors from whom the variant is sourced. Handled via references, often with associated sourcing details (cost, lead time, supplier SKU).
    *   **Purpose:** Procurement, Cost Management, Supply Chain Management, Alternative Sourcing.
    *   **Placement:** `ProductVariant` Aggregate (holds collection of `DistributorId` references or `SourcingInfo` VOs/Entities).
    *   **Constraints:** Collection can be empty. Each `DistributorId` must reference a valid Distributor entity (FK integrity). No duplicate links to the same distributor per variant. Associated sourcing details (CostPrice, LeadTime) must be valid (e.g., non-negative). IDs/VOs are immutable.
*   **TaxonomyPath (`string`):**
    *   **Description:** Denormalized, human-readable string showing the primary category hierarchy (e.g., "Electronics > Audio > Headphones"). Primarily for display/feeds.
    *   **Placement:** Stored denormalized on `Product` (or read model), *derived* from Category relationships.
    *   **Constraints:** Optional (nullable/empty). Format requires consistent delimiter and hierarchy order. CRITICAL: Must be kept synchronized with the canonical Category data via background jobs or triggers; otherwise becomes stale. Not unique.

### Measurement & Handling

*   **UnitOfMeasure (`UnitOfMeasure.T` Value Object):**
    *   **Description:** Standard unit by which the variant is priced and sold (e.g., "Each", "kg", "Liter", "Pack").
    *   **Purpose:** Customer Clarity, Accurate Pricing, Order Fulfillment, Inventory Link, Price Comparison.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Mandatory. Must be a value from a predefined, controlled list/type (enum/DU) defined in the domain. Immutable. Factory validates input codes.
*   **UnitValue (`UnitValue.T` Value Object):**
    *   **Description:** The numeric quantity associated with the `UnitOfMeasure` (e.g., `1` for 1kg, `0.5` for 500ml, `6` for Pack of 6).
    *   **Purpose:** Clarifies exact quantity per sellable unit. Defines price context. Impacts inventory conversion.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Must be > 0. Data type (decimal/int) depends on need for fractions. Optionality/applicability depends contextually on the `UnitOfMeasure` (often implicit '1' for 'Each', mandatory for 'kg', 'Pack'). Enforced by Aggregate/Service. Immutable. Precision/scale constraints apply if decimal.
*   **IsVariableWeight (`bool`):**
    *   **Description:** Indicates if the final price is determined by actual weight at fulfillment (e.g., loose produce, deli items).
    *   **Purpose:** Dictates pricing calculation, ordering process, fulfillment workflow.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Mandatory (non-null). Defaults to `False`. If `True`, `UnitOfMeasure` must be a weight unit (Aggregate invariant). If `True`, `Price` represents price *per weight unit*. Immutable conceptually (changing this implies a new variant).
*   **AverageWeightPerUnit (`Weight.T` Value Object):**
    *   **Description:** Estimated/typical weight of one sellable unit, used for shipping estimates and approximate pricing for variable weight items. Encapsulates value and weight unit (kg, lb, etc.).
    *   **Purpose:** Shipping Cost Estimation, Customer Weight Indication.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Optional (nullable). If present, value must be > 0, unit must be a valid weight unit. Weight VO itself is immutable, but the average can be updated on the variant by replacing the VO instance. Required by logic if `IsVariableWeight` is `True`.
*   **StorageRequirements (`string` / Structured VO):**
    *   **Description:** Necessary conditions for storing/handling the product (e.g., "Refrigerated", "Frozen", "Fragile", "Keep Dry"). Ideally a set of predefined tags/indicators (Value Object) rather than free text.
    *   **Purpose:** Logistics (WMS, Shipping), Fulfillment Handling, Customer Information, Safety.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Optional (defaults to ambient/standard). If using indicators, must be from a predefined list. Business rules may prevent invalid combinations (e.g., Frozen and Refrigerated simultaneously) - enforced by Aggregate.
*   **IsPerishable (`bool`):**
    *   **Description:** Indicates if the product has a limited shelf life and is subject to decay, requiring special handling/tracking.
    *   **Purpose:** Defines Handling, Inventory Policies (FIFO/FEFO), Shipping Rules, Returns Policy.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Mandatory (non-null). Defaults to `False`. If `True`, specific `StorageRequirements` must be present (Aggregate invariant). If `True`, implies need for expiry tracking (shelf life/lot tracking). Immutable conceptually (changing implies new variant).

### Compliance & Tax

*   **TaxClass (`string` / `TaxClass.T` Value Object):**
    *   **Description:** Classification code determining applicable tax rules/rates. Input for tax calculation engine.
    *   **Purpose:** Accurate Tax Calculation, Compliance.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Usually Mandatory (defaults to a standard taxable class). Code must be from a predefined list recognized by the tax system/engine (enforced by VO factory if used). Immutable VO.
*   **CountryOfOrigin (`CountryName.T` / `CountryCode.T` Value Object):**
    *   **Description:** Country where the product was manufactured/grown/substantially transformed (e.g., ISO 3166-1 alpha-2 code like "US", "CN").
    *   **Purpose:** Trade Compliance, Customs, Customer Information.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Optional (nullable). If present, must be a valid, standardized code (e.g., ISO alpha-2). Immutable VO.

### Operational Status

*   **IsSellable (`bool`):**
    *   **Description:** Master switch indicating if the variant is *intended* to be available for customer purchase (visibility/purchasability on storefront).
    *   **Purpose:** Lifecycle Management (active, seasonal, discontinued), Storefront Control, Separation from stock status.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Mandatory (non-null). Defaults to `False` (safer). Operates independently of inventory quantity. Mutable (via Aggregate methods). Must be `False` if `IsActive` is `False` (Aggregate invariant).
*   **IsActive (`bool`):**
    *   **Description:** High-level status indicating if the variant record is operational/relevant within backend systems (distinguished from draft, archived, logically deleted).
    *   **Purpose:** Data Management Lifecycle, System Performance (filtering), Workflow Status, Integration Filtering.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Mandatory (non-null). Default depends on workflow (often `True` if no draft state, `False` if explicit activation needed). Mutable (via Aggregate methods). If `False`, then `IsSellable` must be `False` (Aggregate invariant). Distinct from physical deletion.

### Media & Supplemental Data

*   **ImageUrl (`ImageUrl.T` Value Object):**
    *   **Description:** URL for the primary/default product image.
    *   **Purpose:** Main visual representation (PDP hero, listings).
    *   **Placement:** `ProductVariant` Aggregate (as variants like color often have different images).
    *   **Constraints:** Optional (nullable), but likely required by logic before `IsSellable`=True. If present, must be a valid, HTTPS absolute URL (VO factory validation). Immutable VO. Does not guarantee image exists/is reachable.
*   **ThumbnailUrl (`ImageUrl.T` Value Object):**
    *   **Description:** URL for a smaller, optimized version of the image for listings, grids, carts.
    *   **Purpose:** Performance Optimization, Quick Visual ID in lists.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Optional (nullable), but likely required by logic before `IsSellable`=True. If present, must be a valid, HTTPS absolute URL (VO factory validation). Immutable VO. Should visually represent the correct variant and ideally be derived from `ImageUrl`. Does not guarantee image exists/is reachable.
*   **NutritionalInformation (`string` / Structured VO):**
    *   **Description:** Detailed nutritional content (serving size, calories, macros, micros) for applicable products (food, supplements). Ideally a complex Value Object encapsulating structured data.
    *   **Purpose:** Customer Health Info, Dietary Needs, Regulatory Compliance, Comparison.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Optional (nullable). If present, internal components (serving size, calories, ingredients) must be valid and non-negative (enforced by VO factory). Immutable VO.
*   **Ingredients (`string` / Structured VO):**
    *   **Description:** Complete list of components/ingredients for applicable products (food, cosmetics, etc.). Often a formatted string adhering to regulations.
    *   **Purpose:** Customer Information, Safety, Compliance, Allergy/Preference Checks.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Optional (nullable). If present, must not be empty/whitespace. May have minimal format validation. Immutable VO. Source for Allergen info but distinct.
*   **Allergens (`string` / Structured VO):**
    *   **Description:** Explicit declaration of major allergens present or potentially present due to cross-contamination. Ideally a set of predefined allergen identifiers or a structured statement VO.
    *   **Purpose:** Customer Safety, Regulatory Compliance, Filtering.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Optional (nullable). If present using identifiers, must use predefined list of recognized allergens. Must be consistent with/derived from Ingredients/manufacturing info (process constraint). Immutable VO.

### Audit Timestamps

*   **CreatedAt (`System.DateTimeOffset`):**
    *   **Description:** Timestamp (UTC recommended) when the product variant record was first created in the system.
    *   **Purpose:** Auditing, Lifecycle Tracking, Data Analysis, Troubleshooting.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Mandatory (non-null). Immutable (set only on creation). System-generated. Must be timezone-aware.
*   **UpdatedAt (`System.DateTimeOffset`):**
    *   **Description:** Timestamp (UTC recommended) when the product variant record was last modified in the system.
    *   **Purpose:** Auditing, Data Synchronization, Optimistic Concurrency, Cache Invalidation.
    *   **Placement:** `ProductVariant` Aggregate.
    *   **Constraints:** Mandatory (non-null). Initialized to `CreatedAt` value. Mutable (MUST be updated automatically by Aggregate/system on every successful state change). System-generated. Must be timezone-aware. Must be >= `CreatedAt`.

---

This README provides an overview of the Value Objects used and the attributes comprising the ValidatedProduct state within the Inventory domain.
