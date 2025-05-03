namespace StoreZero.Inventory.Domain.Entity

open System // Needed for Guid, DateTimeOffset, Collections.Generic
open StoreZero.Inventory.Domain.ValueObject
// Needed for ProductId, Sku, BarcodeUpc, ProductName, ProductDescription
// BrandName, ManufacturerName, ProductCategory, UnitOfMeasure, UnitValue
// Weight, ImageUrl, CountryName


// --- Helper module for combining hash codes ---
module HashHelpers =
  let combineHashCodes (hashCodes: int array) : int =
    let seed = 48271 // A prime number
    let mutable hash = seed
    for h in hashCodes do
        hash <- (hash * 397) + h // Another prime number
    hash

  // Helper to hash a list of items that support GetHashCode
  let hashList (items: System.Collections.Generic.List<'T>) : int =
    let seed = 23 // Another prime number
    let mutable hash = seed
    for item in items do
        hash <- (hash * 31) + item.GetHashCode() // Another prime number
    hash


// Module encapsulating the Product domain types and validation logic.
module Product =

  // Represents a product with raw, unvalidated data, typically from an
  // external source. Fields here are strings or other basic types as the data
  // hasn't been confirmed yet.
  [<CLIMutable>]
  [<CustomEquality>]
  [<NoComparison>]
  type Unvalidated = {
    // A temporary identifier or external reference used during the
    // import/validation process.
    TemporaryId : System.Guid

    // Raw string value for the Stock Keeping Unit.
    RawSku : string
    // Raw string value for the Barcode or UPC.
    RawBarcodeUpc : string
    // Raw string value for the product's name.
    RawName : string
    // Raw string value for the product's short description.
    RawShortDescription : string
    // Raw string value for the product's long description.
    RawLongDescription : string
    // Raw string value for the product's brand name.
    RawBrand : string
    // Raw string value for the product's manufacturer name.
    RawManufacturer : string
    // Raw string value for the product's primary category.
    RawCategory : string
    // Raw string value for the product's subcategory.
    RawSubCategory : string
    // Raw string value representing the hierarchical path in the product
    // taxonomy.
    RawTaxonomyPath : string
    // Raw string value for the unit of measure (e.g., "kg", "liter").
    RawUnitOfMeasure : string
    // Raw string value for the quantity represented by the unit (e.g., "1",
    // "500").
    RawUnitValue : string
    // Raw string indicating if the product has variable weight (e.g., "true",
    // "false", "yes", "no").
    RawIsVariableWeight : string
    // Raw string value for the average weight per unit, for variable weight
    // items.
    RawAverageWeightPerUnit : string
    // Raw string value describing the storage requirements (e.g.,
    // "Refrigerated").
    RawStorageRequirements : string
    // Raw string indicating if the product is perishable (e.g., "true",
    // "false").
    RawIsPerishable : string
    // Raw string value for the tax classification.
    RawTaxClass : string
    // Raw string indicating if the product is currently sellable online (e.g.,
    // "true", "false").
    RawIsSellable : string
    // Raw string indicating if the product is active in the catalog (e.g.,
    // "true", "false").
    RawIsActive : string
    // Raw string value for the primary image URL.
    RawImageUrl : string
    // Raw string value for the thumbnail image URL.
    RawThumbnailUrl : string
    // Raw string value for nutritional information.
    NutritionalInformation : string
    // Raw string value for the list of ingredients.
    Ingredients : string
    // Raw string value for the list of common allergens.
    Allergens : string
    // Raw string value for the country of origin.
    RawCountryOfOrigin : string
    // Timestamp when the unvalidated product record was created.
    CreatedAt : System.DateTimeOffset
  }
  with
    override this.Equals(other: obj) : bool =
      match other with
      | :? Unvalidated as otherProduct ->
          this.TemporaryId = otherProduct.TemporaryId
      | _ -> false

    override this.GetHashCode() : int =
      this.TemporaryId.GetHashCode()


  // Represents a specific validation error encountered during the validation
  // workflow.
  [<CustomEquality>]
  [<NoComparison>]
  type ValidationError = {
    // The name of the field that failed validation.
    FieldName : string
    // A descriptive message explaining the validation error.
    ErrorMessage : string
  }
  with
    override this.Equals(other: obj) : bool =
      match other with
      | :? ValidationError as otherError ->
          this.FieldName = otherError.FieldName &&
          this.ErrorMessage = otherError.ErrorMessage
      | _ -> false

    override this.GetHashCode() : int =
      HashHelpers.combineHashCodes [|
          this.FieldName.GetHashCode();
          this.ErrorMessage.GetHashCode()
      |]

  // Represents a product that has successfully passed the validation workflow.
  // All data here is considered clean and correctly formatted according to
  // business rules, using Value Objects where appropriate to ensure data
  // integrity.
  [<CLIMutable>]
  [<CustomEquality>]
  [<NoComparison>]
  type Validated = {
    // Unique Identifier for the product (internal system ID), represented by a
    // ProductId Value Object.
    ProductId : ProductId.T
    // Stock Keeping Unit, represented by a Sku Value Object.
    Sku : Sku.T
    // Universal Product Code or EAN, represented by a BarcodeUpc Value Object.
    BarcodeUpc : BarcodeUpc.T
    // Full product name, represented by a ProductName Value Object.
    Name : ProductName.T
    // Brief summary of the product, represented by a ProductDescription Value
    // Object.
    ShortDescription : ProductDescription.T
    // Detailed description, ingredients, usage instructions, etc., represented
    // by a ProductDescription Value Object.
    LongDescription : ProductDescription.T
    // The brand of the product, represented by a BrandName Value Object.
    Brand : BrandName.T
    // The manufacturer of the product, represented by a ManufacturerName Value
    // Object.
    Manufacturer : ManufacturerName.T
    // Primary category of the product, represented by a ProductCategory Value
    // Object.
    Category : ProductCategory.T
    // More specific categorization (e.g., "Milk", "Fresh Vegetables").
    SubCategory : string
    // Hierarchical classification for navigation (e.g., "Groceries -> Dairy
    // -> Milk").
    TaxonomyPath : string
    // Unit of Measure, represented by a UnitOfMeasure Value Object.
    UnitOfMeasure : UnitOfMeasure.T
    // The quantity represented by the unit, represented by a UnitValue Value
    // Object.
    UnitValue : UnitValue.T
    // Indicates if the final weight/price is determined at fulfillment (like
    // loose produce).
    IsVariableWeight : bool
    // For variable weight items, an estimate for calculating approximate order
    // totals, represented by a Weight Value Object.
    AverageWeightPerUnit : Weight.T
    // Storage requirements (e.g., "Refrigerated", "Frozen", "Pantry").
    StorageRequirements : string
    // Indicates if the product has an expiry date and requires Lot/Batch
    // tracking.
    IsPerishable : bool
    // For calculating applicable taxes.
    TaxClass : string
    // Indicates if the product is currently available for sale online.
    IsSellable : bool
    // Indicates if the product is an active part of the catalog.
    IsActive : bool
    // URL for the primary product image, represented by an ImageUrl Value
    // Object.
    ImageUrl : ImageUrl.T
    // URL for a smaller image, represented by an ImageUrl Value Object.
    ThumbnailUrl : ImageUrl.T
    // Nutritional information (can be structured data or text).
    NutritionalInformation : string
    // List of ingredients.
    Ingredients : string
    // List of common allergens present.
    Allergens : string
    // Country of origin for the product, represented by a CountryName Value
    // Object.
    CountryOfOrigin : CountryName.T
    // Timestamp when the product record was created.
    CreatedAt : System.DateTimeOffset
    // Timestamp when the product record was last updated.
    UpdatedAt : System.DateTimeOffset
  }
  with
    override this.Equals(other: obj) : bool =
      match other with
      | :? Validated as otherProduct ->
          this.ProductId.Equals(otherProduct.ProductId)
      | _ -> false

    override this.GetHashCode() : int =
      this.ProductId.GetHashCode()
