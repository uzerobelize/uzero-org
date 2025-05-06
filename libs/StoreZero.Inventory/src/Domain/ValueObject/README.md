
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
*   Standardized external codes (`Gtin`)
*   Timestamp markers (`CreatedAt`, `UpdatedAt`)
*   Domain-specific codes or references (`CountryOfOrigin`, `BrandId`, `ManufacturerId`, `CategoryId`, `DistributorId`)

This README serves as a guide to these essential Value Objects, detailing their specific purpose, the attributes they encapsulate, and the constraints they enforce within the Inventory domain. Understanding these Value Objects is key to correctly interacting with and extending the domain model.

* * * * *

Attributes of ValidatedProduct:
-------------------------------

*(Note: Many attributes belong conceptually to the `ProductVariant` Aggregate Root (representing a specific SKU), while others belong to the parent `Product` Aggregate Root. This distinction should be clarified in the final domain model implementation.)*

### Identifiers (Internal & External)

*   **Conceptual Field Name:** ProductId (or ProductID)

    **Purpose (within the Domain Model):**
    Serves as the unique, immutable identity for the Product Aggregate Root. It distinguishes one core product concept (e.g., "BrandX Model Y Running Shoe") from all others within the system, regardless of its specific variations. This ID is the anchor for the aggregate and guarantees its continuity and referenceability throughout its lifecycle. It's a fundamental part of the Ubiquitous Language ("Which ProductId does this SKU belong to?", "Look up ProductId `xxxxxxxx-xxxx-...`"). Within DDD, the ProductId itself is implemented as a Value Object to ensure validity, type safety, and clear intent.

    **Importance (in DDD):**
    The ProductId is paramount for:
    *   **Aggregate Identity:** Provides the stable identity required for an Aggregate Root.
    *   **Uniqueness:** Guarantees each distinct product concept has one identifier within the Bounded Context.
    *   **Referenceability:** Allows other Aggregates (e.g., Order) or Entities (e.g., Sku, ProductVariant) to reliably reference the specific Product Aggregate.
    *   **Data Integrity:** Acts as the conceptual primary key.
    *   **System Operations:** Enables unambiguous lookup, retrieval, update, and deletion.
    *   **Lifecycle Tracking:** Provides a constant identifier despite changes in attributes or variants.

    **Placement within the Domain Model:**
    The `ProductId` is the root identifier for the `Product` Aggregate. Entities closely related to the Product, such as `ProductVariant` and `Sku`, will be modeled as separate Entities (potentially within the same Aggregate or a different one, depending on consistency boundaries) and will hold a reference to the `ProductId`. These Entities will have their own unique identifiers (e.g., `SkuId`).

    **Implementation Concept (DDD Value Object Pattern):**
    Conceptually, `ProductId` is implemented as a custom Value Object type. This type would:
    *   Encapsulate the actual identifier value, which will be a **GUID/UUID**. (Consider using UUIDv7 if feasible for database index performance benefits).
    *   Have its creation controlled via static factory methods:
        *   `ProductId.create()`: Generates a new, unique ProductId (using a UUID generator).
        *   `ProductId.parse(Guid existingGuid)`: Creates a ProductId instance from a pre-existing, validated GUID. Used in specific infrastructure scenarios (e.g., repository reconstitution, data migration).
        *   `ProductId.parse(string guidString)`: Creates a ProductId instance by parsing a string representation, ensuring it's a valid GUID format.
    *   These factory methods enforce validity constraints (e.g., ensuring a GUID is not the empty GUID, ensuring a parsed string is a valid GUID format and matches the canonical representation).
    *   Implement value-based equality (comparing the underlying GUIDs). Override `Equals()` and `GetHashCode()` (or use language features like C# records or Kotlin data classes).
    *   Ensure the Value Object instance itself is **immutable** (e.g., using `readonly`/`final` fields and no setters).
    *   Plan for serialization/deserialization, typically mapping to the underlying GUID string representation. Custom converters might be needed depending on the framework.
    *   Using a dedicated `ProductId` type prevents accidental misuse (e.g., passing a `CategoryId` where a `ProductId` is expected) and makes the domain model more expressive.

    **Constraints (Enforced Primarily by the Domain Model Type & Aggregate):**

    *   **Uniqueness (System-Wide):**
        *   *Constraint:* The value encapsulated by the `ProductId` Value Object must be unique across all Product aggregates. **Guaranteed primarily by the chosen GUID/UUID generation algorithm** (with negligible collision probability) and ultimately enforced by persistence layer constraints (e.g., database primary key).
        *   *Rationale:* Core requirement of identity. Prevents ambiguity.
        *   *Enforcement (DDD):* While the VO itself doesn't know about others, uniqueness is enforced during aggregate creation. The `ProductId.create()` method uses a reliable GUID generator. Database constraints provide the persistence-level guarantee. Application services handle uniqueness checks only if absolutely necessary (e.g., during imports where external IDs *might* clash, though unlikely with GUIDs).

    *   **Mandatory / Non-Null / Non-Empty:**
        *   *Constraint:* Every instance of a `Product` Aggregate must possess a `ProductId`. The identifier cannot be null or undefined, **nor can it be the 'empty' GUID (`00000000-0000-0000-0000-000000000000`)**.
        *   *Rationale:* An Aggregate Root cannot exist without a valid identity.
        *   *Enforcement (DDD):* The `ProductId` field within the `Product` Aggregate Root uses the non-nullable `ProductId` custom type. The Aggregate's creation logic ensures an ID is assigned immediately using a valid `ProductId` instance (typically via `ProductId.create()`). **The `ProductId` factory methods (`create`, `parse`) validate against the empty GUID.**

    *   **Immutability (CRITICAL Domain Invariant):**
        *   *Constraint:* Once a `ProductId` is assigned to a `Product` Aggregate instance upon its creation, it must **never change** for the entire lifetime of that conceptual product.
        *   *Rationale:* The identity *is* the product from the system's perspective. Changing it breaks continuity and references.
        *   *Enforcement (DDD):* Enforced by the `Product` Aggregate Root and the `ProductId` Value Object. The ID is set only during creation (via constructor or factory), and no methods allow its modification. The Value Object itself is immutable **through language constructs (e.g., `readonly`/`final` fields and lack of setter methods)**.

    *   **Data Type & Format Consistency:**
        *   *Constraint:* All `ProductId` values must encapsulate a valid **GUID/UUID according to RFC 4122**. **A consistent canonical string representation (e.g., lowercase with hyphens: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`) should be used internally and enforced during parsing from string.**
        *   *Rationale:* Ensures predictability, type safety, and proper functioning.
        *   *Enforcement (DDD):* Enforced by the `ProductId` Value Object's definition and its factory/parsing methods' validation logic.

    *   **System Generated (Primary Path):**
        *   *Constraint:* The identifier value **should normally be generated by the system** using the `ProductId.create()` factory method. **Creation from existing values using `parse()` methods should be reserved for specific, controlled scenarios** like data migration, repository reconstitution, or integration with systems that provide pre-existing IDs.
        *   *Rationale:* Guarantees uniqueness (for GUIDs), decouples the domain identity from potentially volatile external codes, reduces human error.
        *   *Enforcement (DDD):* The default `Product` Aggregate creation logic calls `ProductId.create()`. Infrastructure code (repositories, migrations, specific integrations) might use `ProductId.parse()` after validating the source ID.

    **Human Readability:**
    *   While `ProductId` (as a GUID) provides robust uniqueness, it's not inherently human-readable. If frequent human interaction (e.g., support, administration) requires a simpler identifier, consider adding a separate field (e.g., `ProductCode`, `InternalReference`) directly to the `Product` Aggregate. This field is for display or search purposes and is *not* the aggregate's primary identity (`ProductId`).

    **Relationship to Other Domain Concepts:**
    *   Identifies the `Product` Aggregate Root.
    *   Referenced by related **Entities like `ProductVariant` and `Sku`** (which have their own identities, e.g., `SkuId`) to link back to the parent Product concept.
    *   Referenced by other Aggregates (e.g., `OrderLine` references the `ProductId` of the item ordered) or external systems.
    *   **Distinct from `SkuId`**: `SkuId` (likely a string-based Value Object identifying the `Sku` Entity) identifies a specific, sellable variant/stock-keeping unit, while `ProductId` identifies the overall product concept that the SKU belongs to. There's typically a many-to-one relationship (many SKUs per ProductId).

*   **Sku (`Sku.T` Value Object):**
    *   **Description:** Stock Keeping Unit - the unique internal identifier for a specific, sellable variant used for tracking inventory.
    *   **Purpose:** Granular Inventory Tracking, Accurate Order Fulfillment, Variant-Specific Pricing/Promotions, Sales Analysis, Disambiguation.
    *   **Placement:** `ProductVariant` Aggregate (identifies the specific variant).
    *   **Constraints:** Must not be null or whitespace. Must be between 8 and 15 characters long (configurable). Value is treated as case-insensitive (typically stored uppercase). Specific character format rules enforced by VO factory. Uniqueness across all active variants enforced by system/DB. Mandatory. Immutable.
*   **Conceptual Module Name:** `Gtin`

    **2. Purpose (within the Domain Model):**
    This concept represents standardized Global Trade Item Numbers (GTINs), specifically supporting the 12-digit (GTIN-12 / UPC-A) and 13-digit (GTIN-13 / EAN-13) formats. These are assigned by manufacturers to identify specific trade items globally. It serves as a recognized identifier for retail point-of-sale scanning, inventory receiving, supply chain tracking, and integration with external marketplaces. It's part of the Ubiquitous Language related to external product identification ("Scan the GTIN/UPC/EAN," "Match products by GTIN," "Marketplace requires a valid GTIN"). Modeled as a Value Object encapsulating structure, validation rules, and specific format (GTIN-12 or GTIN-13).

    **3. Importance (in DDD):**
    Modeling `Gtin` as a distinct Value Object provides:
    *   **Validity & Standardization:** Ensures only syntactically valid GTIN-12 or GTIN-13 numeric strings, including mandatory check digit validation, are represented. Prevents storage of incorrectly formatted or nonsensical codes.
    *   **Clarity & Intent:** Using a dedicated `Gtin.T` type clearly signals its purpose and distinguishes it from internal identifiers (Sku, ProductId) or raw strings. The DU structure further clarifies the *specific* GTIN format.
    *   **Consistency:** Guarantees GTINs are handled and validated uniformly. Avoids issues like accidental truncation of leading zeros.
    *   **Encapsulation:** Groups the data (the numeric code string) and its specific validation logic (length, numeric, check digit algorithm) together within the `Gtin` module.
    *   **Reduced Primitive Obsession:** Avoids using generic strings for a concept with specific, globally defined structures and meanings.

    **4. Placement within the Domain Model:**
    *   The `Gtin.T` Value Object typically belongs on the `ProductVariant` (or equivalent Aggregate Root representing a specific SKU), marked as optional (nullable or `option` type). This accommodates the most granular scenario where different variants (colors, sizes, flavors) might have distinct GTINs.
    *   Storing it on the base `Product` is generally discouraged unless a strict, invariant business rule dictates all variants *always* share the same GTIN (rare and inflexible).

    **5. Implementation Concept (F# Functional Value Object):**
    *   A module named `Gtin` will contain all related definitions.
    *   **Core Type (`Gtin.T`):** A Discriminated Union (DU) defining the possible valid GTIN formats:
        ```fsharp
        type T =
            | Gtin12 of string // Holds the validated 12-digit string
            | Gtin13 of string // Holds the validated 13-digit string
        ```
        *   The DU cases implicitly make the type self-contained and prevent direct instantiation without validation.
    *   **Error Type (`Gtin.GtinError`):** A DU specifying possible validation failures:
        ```fsharp
        type GtinError =
            | NullOrEmptyInput
            | UnsupportedFormat of actualLength: int // Input length is not 12 or 13
            | NonNumericCharacters of value: string
            | InvalidCheckDigit of value: string * expected: int * actual: int
            // Note: IncorrectLength is covered by UnsupportedFormat now
        ```
    *   **Factory Functions (within `Gtin` module):**
        *   `create : string -> Result<T, GtinError>`: The primary factory. Accepts a candidate string. Checks for null/empty. Determines expected format based on length (12 or 13). Delegates to the appropriate specific internal validation logic. Returns `Ok Gtin.T` or `Error GtinError`. Rejects inputs with leading/trailing whitespace or non-standard lengths.
        *   (Potentially internal/private helper functions for `Gtin12` and `Gtin13` specific validation if logic becomes complex, but `create` can handle the branching).
    *   **Accessor Functions (within `Gtin` module):**
        *   `value : T -> string`: Extracts the underlying numeric string value from a `Gtin.T` instance.
        *   `format : T -> GtinFormat`: (Optional but recommended) Returns the specific format. Requires a simple `GtinFormat` DU: `type GtinFormat = Gtin12Format | Gtin13Format`.

    **6. Constraints (Enforced Primarily by `Gtin.create` function):**

    *   **Data Type (Encapsulated):** Internally represents a 12-digit or 13-digit numeric code, stored as a string within the appropriate DU case (`Gtin12` or `Gtin13`) to preserve leading zeros.
    *   **Optionality / Nullable (on `ProductVariant`):** The `Gtin.T` field on the `ProductVariant` aggregate must be optional (`option<Gtin.T>` or nullable).
        *   *Rationale:* Not all products sold have a manufacturer-assigned GTIN (custom goods, private label, bundles, etc.).
        *   *Enforcement:* Field definition on `ProductVariant`.
    *   **Input Format (Strict):** The input string passed to `Gtin.create` must not contain leading/trailing whitespace.
        *   *Rationale:* Ensures clean data, prevents ambiguous inputs.
        *   *Enforcement:* Check within `Gtin.create`.
    *   **Format: Fixed Length (If Present):** If a GTIN value is present (not null/none), it must consist of *exactly* 12 or 13 digits.
        *   *Rationale:* Adheres to the supported GTIN standards (GTIN-12/UPC-A, GTIN-13/EAN-13).
        *   *Enforcement:* Length check within `Gtin.create`. Failure results in `GtinError.UnsupportedFormat`.
    *   **Format: Digits Only (If Present):** If present, the value must contain only numeric digits (0-9).
        *   *Rationale:* GTINs are purely numeric codes.
        *   *Enforcement:* Character check (e.g., `String.forall Char.IsDigit`) within `Gtin.create`. Failure results in `GtinError.NonNumericCharacters`.
    *   **Format: Check Digit Validity (Mandatory Validation):** The last digit must correctly correspond to the calculated check digit based on the preceding 11 (for GTIN-12) or 12 (for GTIN-13) digits according to the standard GTIN algorithm.
        *   *Rationale:* Validates the integrity of the GTIN code itself, catching many common data entry errors. Ensures the code is mathematically sound. A code failing this is not a valid GTIN.
        *   *Enforcement:* Mandatory validation logic within `Gtin.create`. Creation fails with `GtinError.InvalidCheckDigit` if the check digit is invalid (providing expected/actual values).
    *   **Immutability:** `Gtin.T` instances are immutable. The GTIN associated with a product variant definition generally does not change.
        *   *Rationale:* Reflects the external, standardized nature. Value Objects are immutable.
        *   *Enforcement:* F# DU immutability by default.
    *   **Equality:** Structural equality is provided by the F# DU. Two `Gtin.T` instances are equal if they are the same case (`Gtin12` or `Gtin13`) and encapsulate the same string value.
    *   **Handling Invalid Source Data:** Raw, unvalidated strings received from external sources (e.g., suppliers) that *fail* `Gtin.create` should *not* be forced into a `Gtin.T` type. If they must be stored for auditing or correction, use a separate, optional `string` field on the `ProductVariant` (e.g., `RawSupplierGtin: option<string>`).

    **7. Uniqueness (System-Level Guideline):**
    *   *Constraint:* While multiple `ProductVariants` (SKUs) of the *same base product* might share the same GTIN, a single `Gtin.T` value should not typically identify two fundamentally *different active base products* within the retailer's system.
    *   *Rationale:* Ensures clarity when the GTIN is used as a lookup key.
    *   *Enforcement:* Data governance rule. Potentially checked via:
        *   Database unique index (allowing multiple nulls) on the persisted GTIN string column in the `ProductVariants` table.
        *   Checks within the Application Service layer before saving/updating `ProductVariant` aggregates.

    **8. Persistence:**
    *   The `Gtin.T` value should be persisted as a single string column in the database (e.g., `VARCHAR(13)`) to accommodate the longest format.
    *   The `Gtin.value` accessor function is used to get the string for storage.
    *   When reading from the database, the string is passed to `Gtin.create` to reconstruct the validated `Gtin.T` instance.

    **9. Relationship to Other Domain Concepts:**
    *   Associated with a `ProductVariant` (SKU).
    *   Distinct from internal `Sku` and `ProductId`.
    *   Used for Inventory Receiving, External Marketplace integration, POS integration, etc.

    This updated specification incorporates the move to a generic `Gtin` module, support for GTIN-12 and GTIN-13 via a DU, standalone factory functions within the module, and detailed error types, aligning with the F# functional approach requested.


### Descriptive Content

*   **Conceptual Field Name:** `ProductName`

    4.  **Purpose:**
        Represents the validated, canonical, and human-readable name of a Product concept. It serves as the primary textual identifier for products, ensuring consistency, validity, and adherence to specific business rules across the system. It encapsulates the name string along with its normalization and validation logic, preventing invalid states within the domain model.

    5.  **Key Characteristics:**
        *   **Immutability:** Once created, a `ProductName` instance cannot be altered. Modifications result in a new instance.
        *   **Validation:** Creation involves strict validation against defined constraints.
        *   **Normalization:** Applies specific whitespace handling rules during creation.
        *   **Equality:** Based on the *normalized, case-insensitive* string value.
        *   **Expressiveness:** Replaces primitive `string` types for product names, clarifying intent and enforcing rules.

    6.  **Constraints & Validation Rules (Applied During Creation):**
        *   **Non-Empty/Null:** Input cannot be null, empty, or consist only of whitespace *before* normalization.
        *   **Whitespace Normalization:**
            *   Leading and trailing whitespace characters are removed.
            *   Sequences of multiple internal whitespace characters are collapsed into a single space.
            *   *Example:* `"   Deluxe  Widget   Pro (v2)  "` becomes `"Deluxe Widget Pro (v2)"`.
        *   **Minimum Length:** The normalized string must have a minimum length of **4 characters**.
        *   **Maximum Length:** The normalized string must have a maximum length of **150 characters**.
        *   **Allowed Characters:** The normalized string must only contain:
            *   Unicode Letters (supporting international names)
            *   Numbers (0-9)
            *   Specific Symbols: space, hyphen (`-`), underscore (`_`), ampersand (`&`), apostrophe (`'`), period (`.`), comma (`,`), parentheses (`()`), forward slash (`/`).
        *   **Disallowed Characters:** Explicitly prohibits:
            *   Control characters (e.g., newline, tab - space is allowed post-normalization).
            *   Characters often problematic in web contexts or search: `<`, `>`, `|`, `\`.
            *   Other symbols not explicitly listed in the "Allowed Characters" section.
        *   **Semantic Filtering:** Does **not** perform checks for profanity, reserved words, or other semantic content rules. These are considered separate concerns, potentially handled by Application Services or other domain services/processes.

    7.  **Creation (Factory Function):**
        *   Creation is handled via a dedicated factory function, typically within a `ProductName` module (e.g., `ProductName.create`).
        *   **Signature (Conceptual F#):** `create : string -> Result<ProductName, ProductNameError>`
        *   **Input:** Takes a raw `string` as input.
        *   **Process:**
            1.  Checks for null/empty/whitespace-only input.
            2.  Applies whitespace normalization (trim ends, collapse internal).
            3.  Validates the normalized string against minimum/maximum length constraints.
            4.  Validates the normalized string against the allowed character set.
        *   **Output:**
            *   **Success:** Returns `Ok(ProductName)` containing the newly created, validated, and normalized `ProductName` instance.
            *   **Failure:** Returns `Error(ProductNameError)` detailing the *first* validation rule that failed.
        *   **`ProductNameError` Type (Conceptual F# Discriminated Union):**
            ```fsharp
            type ProductNameError =
                | IsNullOrWhitespace
                | TooShort of minLength: int * actualLength: int
                | TooLong of maxLength: int * actualLength: int
                | ContainsInvalidCharacters of invalidChars: char list
            ```
            *(This provides specific reasons for validation failure, suitable for Railway Oriented Programming).*

    8.  **Equality:**
        *   Two `ProductName` instances are considered equal if their internal, normalized string values are identical when compared in a **case-insensitive** manner.
        *   *Example:* A `ProductName` created from `"Widget Pro"` is equal to one created from `"widget pro"` or `"  Widget   PRO "`.

    9.  **Value Access:**
        *   The underlying normalized `string` value can be accessed via a dedicated function within the `ProductName` module.
        *   **Signature (Conceptual F#):** `value : ProductName -> string`
        *   This returns the canonical, normalized string representation stored within the Value Object.

    10. **Placement within Domain Model:**
        *   Primarily resides on the `Product` Aggregate Root, representing the core concept's name.
        *   Variant-specific naming details (like color or size names) are typically handled as separate attributes on the `ProductVariant` entity. Display names combining these are constructed outside this VO.

    11. **Ubiquitous Language Integration:**
        *   Use the term `ProductName` consistently in discussions, code, and requirements related to the primary name of a product.
        *   Refer to its constraints explicitly (e.g., "`ProductName` maximum length", "invalid characters for `ProductName`").

*   **Conceptual Name:** `ShortDescription`

    **2. Purpose & Domain Significance:**

    *   Represents a concise, engaging summary of the product, designed to quickly communicate its primary benefit, key feature, or core value proposition.
    *   Serves as a marketing "hook" or abstract, distinct from the primary `Name` and the comprehensive `LongDescription`.
    *   It is part of the Ubiquitous Language used by merchandising, content, UI/UX, and development teams (e.g., "Write the short description," "Display the summary near the price," "The hook text for the listing page").
    *   **Key Usage Context:** Often displayed in UI elements with limited space, such as:
        *   Product Listing Pages (PLPs) / Category Grids
        *   Search Result Snippets
        *   Product Detail Pages (PDPs) near the price/title
        *   Related/Recommended Product widgets
        *   Mobile views where brevity is paramount.

    **3. Placement within the Domain Model:**

    *   Belongs to the `Product` Aggregate Root.
    *   Typically summarizes the overall product concept, though specific marketing needs might occasionally warrant variant-level summaries handled differently.

    **4. Importance (Rationale for Value Object):**

    *   **Validity & Constraints:** Ensures the text adheres to specific business rules (brevity, format, presence if provided), preventing invalid data from entering the domain.
    *   **Clarity & Intent:** Makes the domain model explicit about the purpose and nature of this text, distinguishing it from generic strings (`Name`, `LongDescription`, attributes).
    *   **Encapsulation:** Bundles the descriptive text with its validation and normalization logic.
    *   **Reduced Primitive Obsession:** Avoids treating specifically constrained descriptive text as a generic `string`.
    *   **Consistency:** Promotes uniform handling, validation, and usage across the application.

    **5. Implementation Concept (DDD Value Object Pattern):**

    *   Implemented as an immutable custom Value Object type (`ShortDescription`).
    *   Internally encapsulates the normalized summary string.
    *   Creation is controlled exclusively via a static factory method (e.g., `ShortDescription.TryCreate(string rawValue)` or similar).
    *   The factory method performs validation and normalization on the input string.
    *   If the input is valid after normalization, the factory returns a `Success` result containing an immutable `ShortDescription` instance.
    *   If the input is invalid, the factory returns a `Failure` result containing details about the validation errors (see Error Handling).

    **6. Constraints (Enforced by Value Object Factory):**

    | Constraint                  | Rule / Value                                                                                                                                                                                                                                                                                                | Rationale & Notes                                                                                                |
    | :-------------------------- | :---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | :--------------------------------------------------------------------------------------------------------------- |
    | **Optionality on Entity** | The `ShortDescription` field on the `Product` Aggregate Root is **Optional** (nullable or `Option<ShortDescription>`).                                                                                                                                                                                      | Allows flexibility during product creation/import. Content enrichment may happen later.                          |
    |                             |                                                                                                                                                                                                                                                                                                             | *Note:* Lifecycle rules (e.g., "Required before Product can be Published") are enforced by Domain Services or Application Logic, not the VO itself. |
    | **Non-Empty / Whitespace**  | If a value is provided, it **must not** be empty or consist solely of whitespace **after** normalization. (Effectively `normalizedValue.Length > 0`).                                                                                                                                                      | An empty/whitespace summary provides no value.                                                                 |
    | **Maximum Length**          | **160 characters**.                                                                                                                                                                                                                                                                                         | Enforces brevity suitable for UI snippets and aligns with common SEO meta description limits. Confirmed limit.     |
    | **Format**                  | **Plain Text Only.**                                                                                                                                                                                                                                                                                          | Ensures consistency, security, and avoids complex rendering/sanitization logic for this concise field.            |
    |                             | **Must not** contain HTML, XML, Markdown tags, or other markup.                                                                                                                                                                                                                                           |                                                                                                                  |
    |                             | **Must not** contain control characters (ASCII 0-31), except for allowed standard whitespace (` `).                                                                                                                                                                                                       | Prevents rendering issues and potential injection vectors.                                                     |
    | **Allowed Characters**      | Must consist **only** of: <br/> - Unicode Letters <br/> - Numbers (0-9) <br/> - Standard Whitespace (` `) <br/> - **Allowed Punctuation & Symbols:** <br/>   - Essential: `.` `,` `?` `!` `'` `"` <br/>   - Connectors/Groupers: `-` `:` `;` `(` `)` `/` <br/>   - Branding Symbols: `&` `™` `®` `©` <br/>   - Common Symbols: `$` `€` `£` `%` `+` `*` `#` | Allows for readable, engaging marketing text across multiple languages, while preventing unwanted characters. The exact list must be strictly enforced by the validation logic. The list of symbols (`™`, currency, etc.) may be reviewed and expanded based on future business needs. |
    | **Immutability**            | The `ShortDescription` instance is **immutable**.                                                                                                                                                                                                                                                         | Standard Value Object characteristic. Changes require creating a new instance and updating the `Product`.         |
    | **Uniqueness**              | **Not required.**                                                                                                                                                                                                                                                                                         | It's descriptive marketing text; different products can share similar summaries.                               |

    **7. Normalization Rules (Applied by Factory Before Validation):**

    The following steps are applied sequentially to the raw input string during creation:

    1.  **Trim Whitespace:** Remove leading and trailing whitespace characters.
    2.  **Replace Newlines:** Replace all occurrences of newline characters (`\r\n`, `\n`, `\r`) with a single space character (` `).
    3.  **Collapse Internal Whitespace:** Replace sequences of multiple internal whitespace characters (including spaces resulting from newline replacement) with a single space character (` `).

    *Example:* `"  Product A \n   is great!  "` becomes `"Product A is great!"`

    **8. Error Handling (Factory Method):**

    *   The factory method (e.g., `TryCreate`) should return a `Result` object (e.g., `Result<ShortDescription, ErrorList>`) rather than throwing exceptions for validation failures.
    *   Potential validation errors include:
        *   `ValueIsEmptyOrWhitespace`: The input (after normalization) was empty.
        *   `ValueExceedsMaxLength`: The input (after normalization) exceeds the 160-character limit.
        *   `ValueContainsInvalidCharacters`: The input (after normalization) contains characters not present in the defined **Allowed Characters** list.

    **9. Equality Semantics:**

    *   Two `ShortDescription` instances are considered equal if their **normalized** internal string values are identical.
    *   Comparison is typically **case-sensitive** unless explicitly defined otherwise by business requirements.

    **10. Relationship to Other Domain Concepts:**

    *   Associated with the `Product` Aggregate Root.
    *   Provides a concise summary that expands slightly on the product `Name`.
    *   Acts as an introduction or teaser for the more detailed `LongDescription`.
    *   Consumed by the User Interface layer for display (PLP, PDP, etc.).
    *   Contributes moderately to SEO; it may serve as **input** or a default for meta descriptions, but a dedicated SEO service might override or refine this.

    **11. Localization:**

    *   The `ShortDescription` Value Object represents the text in a *single* language.
    *   If multi-language support is required, the `Product` entity is responsible for managing multiple `ShortDescription` instances, potentially using a structure like `Map<Locale, ShortDescription>`. The VO definition itself remains focused on the constraints of a single description string.

*   **Conceptual Name:** `MarkdownLongDescription.T`

    1. Summary

    Name: MarkdownLongDescription

    Type: Value Object (DDD)

    Purpose: Represents the comprehensive, detailed textual description of a Product. It is intended for the main content area of the Product Detail Page (PDP), providing rich, formatted information using Markdown syntax. Its goal is to enhance customer understanding, build purchase confidence, support comparison, and provide SEO-rich content.

    Domain: Inventory

    Aggregate: Product (Typically held as an optional field on the Product Aggregate Root)

    2. Domain Context & Intent

    This Value Object encapsulates the primary narrative content describing a product in detail. It contrasts with the concise ProductName and ShortDescription. By using a specific type (MarkdownLongDescription), we enforce structure, apply validation rules, clarify intent (this is formatted Markdown content), and avoid primitive obsession. It forms a key part of the Ubiquitous Language around product content management ("Update the markdown description," "Ensure the long description formatting is correct").

    3. Implementation Details

    Internal Representation: Encapsulates a single, immutable string value. This string contains the validated and normalized product description text formatted using Markdown syntax.

    Creation: Instances are created exclusively through a static function or a dedicated creation function within a module (e.g., MarkdownLongDescription.create : string -> Result<MarkdownLongDescription, string>). This function enforces all validation rules and normalization steps.

    Validation & Error Handling: The creation function returns a Result<MarkdownLongDescription, ErrorType> (e.g., Result<MarkdownLongDescription, string>) or Choice<MarkdownLongDescription, ErrorType> to indicate success or failure. Failure occurs if any constraint is violated, returning specific error information.

    Immutability: Once created, a MarkdownLongDescription instance cannot be changed. Updates to a product's description involve creating a new MarkdownLongDescription instance and assigning it to the Product. F# records are inherently immutable, making them a good fit.

    4. Constraints & Validation Rules

    These constraints are enforced by the create function before an instance is successfully created.

    Constraint	Rule / Value	Rationale	Enforcement Point
    Optionality on Product	The field holding this VO on the Product Aggregate is Optional (MarkdownLongDescription option).	Allows product creation before details are finalized. The VO itself represents a present value.	Product Record
    Format	Markdown. The internal string value must be treated as Markdown content.	Standardizes rich text format, balancing usability and features.	VO Definition
    Normalization	1. Trim leading/trailing whitespace. <br> 2. Normalize all line endings (\r\n, \r) to LF (\n).	Ensures data consistency and cleanliness.	VO create function
    Non-Empty/Whitespace	Required. After trimming, the input string must not be empty or consist solely of whitespace.	An empty/blank description provides no value.	VO create function
    Minimum Length	30 characters (after trimming and normalization).	Ensures the description has minimal substance to be considered "long" and meaningful.	VO create function
    Maximum Length	No specific domain limit. Limited only by the persistence layer's data type (e.g., NVARCHAR(MAX), TEXT).	Avoids arbitrary limits hindering comprehensive descriptions.	Database Schema
    Character Set	Unicode (UTF-8 recommended).	Supports global languages, symbols, and emojis.	VO create / DB
    Disallowed Chars	Disallow Unicode C0 (\u0000-\u001F) & C1 (\u007F-\u009F) control chars except TAB (\u0009), LF (\u000A), CR (\u000D).	Prevents invisible formatting or potentially problematic non-printable characters.	VO create function
    Markup Validation	Basic check only (non-empty). Does not perform full Markdown syntax validation.	Full validation is complex; relies on Markdown renderer tolerance & user input quality.	VO create function
    Immutability	VO instance is immutable (achieved via F# records or types).	Standard Value Object property. Ensures state consistency.	F# Type System
    Uniqueness	Not required.	Descriptive content, not an identifier. Can be shared or sourced externally.	No Constraint

    5. Normalization Steps

    The create function performs the following normalizations before validation checks (like length):

    Trimming: Removes all leading and trailing whitespace characters from the input string.

    Line Ending Conversion: Replaces all occurrences of Windows-style (\r\n) and old Mac-style (\r) line endings with Unix-style (\n) line endings for consistency.

    6. Security Considerations: Sanitization

    CRITICAL: While this VO stores Markdown, it does not perform HTML sanitization.

    Markdown is often rendered to HTML for display. The consuming layer (e.g., UI, API endpoint) is responsible for:

    Converting the Markdown string from MarkdownLongDescription.Value into HTML using a standard Markdown library.

    Sanitizing the resulting HTML using a robust, allow-list-based HTML sanitizer (e.g., OWASP Java HTML Sanitizer, DOMPurify, Bleach - potentially via JS interop or a .NET wrapper) to prevent Cross-Site Scripting (XSS) attacks before rendering it to the user.

    Failure to sanitize the rendered HTML output poses a significant security risk.

    7. Persistence

    Store the internal string value in the database. Use the Value property of the record/type.

    Recommended Database Types: NVARCHAR(MAX) (SQL Server), TEXT (PostgreSQL, MySQL), CLOB (Oracle). Ensure the database collation supports Unicode (e.g., UTF-8).

    When hydrating from the database, use the create function to reconstruct the VO, ensuring data validity.

    8. Relationship to Other Concepts

    Product: Holds the MarkdownLongDescription (as an option type).

    ProductName, ShortDescription: Provides concise identifiers/summaries, while MarkdownLongDescription provides the detail.

    Attributes / Specifications: Structured data (like Key/Value pairs) that the MarkdownLongDescription might elaborate upon in prose format.

    UI (PDP): Primary consumer of this data for display (after rendering and sanitization).

    SEO: A major source of searchable content for the product.


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
