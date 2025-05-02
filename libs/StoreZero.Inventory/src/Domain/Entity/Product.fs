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

  // Represents a product with raw, unvalidated data.
  [<CLIMutable>]
  [<CustomEquality>]
  [<NoComparison>]
  type Unvalidated = {
    // A temporary identifier or external reference before validation.
    TemporaryId : System.Guid

    RawSku : string
    RawBarcodeUpc : string
    RawName : string
    RawShortDescription : string
    RawLongDescription : string
    RawBrand : string
    RawManufacturer : string
    RawCategory : string
    RawSubCategory : string
    RawTaxonomyPath : string
    RawUnitOfMeasure : string
    RawUnitValue : string
    RawIsVariableWeight : string
    RawAverageWeightPerUnit : string
    RawStorageRequirements : string
    RawIsPerishable : string
    RawTaxClass : string
    RawIsSellable : string
    RawIsActive : string
    RawImageUrl : string
    RawThumbnailUrl : string
    NutritionalInformation : string
    Ingredients : string
    Allergens : string
    RawCountryOfOrigin : string
    CreatedAt : System.DateTimeOffset
  }
  with
    override this.Equals(other: obj) : bool =
      match other with
      | :? Unvalidated as otherProduct ->
          // Unvalidated products are considered equal if their TemporaryId is the same
          this.TemporaryId = otherProduct.TemporaryId
      | _ -> false

    override this.GetHashCode() : int =
      // Hash code based on the field used for equality
      this.TemporaryId.GetHashCode()


  // Represents a specific validation error.
  [<CustomEquality>]
  [<NoComparison>]
  type ValidationError = {
    FieldName : string
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
  // All data here is considered clean and correctly formatted according to business rules.
  [<CLIMutable>]
  [<CustomEquality>]
  [<NoComparison>]
  type Validated = {
    // Unique Identifier for the product (internal system ID).
    ProductId : ProductId.T

    // Stock Keeping Unit - a unique code used for tracking inventory.
    Sku : Sku.T

    // Universal Product Code or EAN - standard external identifier found on packaging.
    BarcodeUpc : BarcodeUpc.T

    // Full product name as displayed to customers.
    Name : ProductName.T

    // Brief summary of the product.
    ShortDescription : ProductDescription.T

    // Detailed description, ingredients, usage instructions, etc.
    LongDescription : ProductDescription.T

    // The brand of the product.
    Brand : BrandName.T

    // The manufacturer of the product (could be different from the brand).
    Manufacturer : ManufacturerName.T

    // Primary category of the product (e.g., "Dairy", "Produce").
    Category : ProductCategory.T

    // More specific categorization (e.g., "Milk", "Fresh Vegetables").
    SubCategory : string

    // Hierarchical classification for navigation (e.g., "Groceries -> Dairy -> Milk").
    TaxonomyPath : string

    // Unit of Measure (e.g., "kg", "liter", "piece", "bunch", "pack").
    UnitOfMeasure : UnitOfMeasure.T

    // The quantity represented by the unit (e.g., 1 for 1kg, 500 for 500ml).
    UnitValue : UnitValue.T

    // Indicates if the final weight/price is determined at fulfillment (like loose produce).
    IsVariableWeight : bool

    // For variable weight items, an estimate for calculating approximate order totals.
    AverageWeightPerUnit : Weight.T

    // Storage requirements (e.g., "Refrigerated", "Frozen", "Pantry").
    StorageRequirements : string

    // Indicates if the product has an expiry date and requires Lot/Batch tracking.
    IsPerishable : bool

    // For calculating applicable taxes.
    TaxClass : string

    // Indicates if the product is currently available for sale online.
    IsSellable : bool

    // Indicates if the product is an active part of the catalog.
    IsActive : bool

    // URL for the primary product image.
    ImageUrl : ImageUrl.T

    // URL for a smaller image.
    ThumbnailUrl : ImageUrl.T

    // Nutritional information (can be structured data or text).
    NutritionalInformation : string

    // Raw ingredients string.
    Ingredients : string

    // List of common allergens present.
    Allergens : string

    // Country of origin for the product.
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
          // Validated products are considered equal if their ProductId is the same.
          this.ProductId.Equals(otherProduct.ProductId)
      | _ -> false

    override this.GetHashCode() : int =
      // Hash code based on the field used for equality
      this.ProductId.GetHashCode()
