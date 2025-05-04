namespace StoreZero.Inventory.Domain.ValueObject

open System // Needed for Guid, Decimal, DateTimeOffset, Uri

// Contains the Value Object definitions for the Product domain.

// Represents a validated Stock Keeping Unit.
module Sku =
  type T = private Sku of string
    with
      override this.ToString() =
        match this with
        | Sku stringValue -> stringValue

  // Creates a validated Sku from a string.
  // TODO: Define validation and creation logic for Sku Value Object.
  let create (value: string) : Result<T, string> =
    if System.String.IsNullOrWhiteSpace(value) then
      Error "SKU cannot be empty."
    else
      Ok (Sku value)

  // Returns the unwrapped string value of the Sku.
  let value (instance: T) : string =
    match instance with
    | Sku innerValue -> innerValue

// Represents a validated Product Name.
module ProductName =
  type T = private ProductName of string
    with
      override this.ToString() =
        match this with
        | ProductName stringValue -> stringValue

  // Creates a validated Product Name from a string.
  // TODO: Define validation and creation logic for ProductName Value Object.
  let create (value: string) : Result<T, string> =
    if System.String.IsNullOrWhiteSpace(value) then
      Error "Product Name cannot be empty."
    else
      Ok (ProductName value)

  // Returns the unwrapped string value of the Product Name.
  let value (instance: T) : string =
    match instance with
    | ProductName innerValue -> innerValue

// Represents a validated Description (Short or Long).
module ProductDescription =
  type T = private ProductDescription of string
    with
      override this.ToString() =
        match this with
        | ProductDescription stringValue -> stringValue

  // Creates a validated Product Description from a string.
  // TODO: Define validation and creation logic for ProductDescription Value Object.
  let create (value: string) : Result<T, string> =
    Ok (ProductDescription (value |> System.String.IsNullOrWhiteSpace |> not |> fun x -> if x then value else ""))

  // Returns the unwrapped string value of the Product Description.
  let value (instance: T) : string =
    match instance with
    | ProductDescription innerValue -> innerValue

// Returns the unwrapped string value of the Product Name.
module BrandName =
  type T = private BrandName of string
    with
      override this.ToString() =
        match this with
        | BrandName stringValue -> stringValue

  // Creates a validated Brand Name from a string.
  // TODO: Define validation and creation logic for BrandName Value Object.
  let create (value: string) : Result<T, string> =
    if System.String.IsNullOrWhiteSpace(value) then
      Error "Brand Name cannot be empty."
    else
      Ok (BrandName value)

  // Returns the unwrapped string value of the Brand Name.
  let value (instance: T) : string =
    match instance with
    | BrandName innerValue -> innerValue

// Represents a validated Manufacturer Name.
module ManufacturerName =
  type T = private ManufacturerName of string
    with
      override this.ToString() =
        match this with
        | ManufacturerName stringValue -> stringValue

  // Creates a validated Manufacturer Name from a string.
  // TODO: Define validation and creation logic for ManufacturerName Value Object.
  let create (value: string) : Result<T, string> =
    if System.String.IsNullOrWhiteSpace(value) then
      Error "Manufacturer Name cannot be empty."
    else
      Ok (ManufacturerName value)

  // Returns the unwrapped string value of the Manufacturer Name.
  let value (instance: T) : string =
    match instance with
    | ManufacturerName innerValue -> innerValue

// Represents a validated Category.
module ProductCategory =
  type T = private ProductCategory of string
    with
      override this.ToString() =
        match this with
        | ProductCategory stringValue -> stringValue

  // Creates a validated Category from a string.
  // TODO: Define validation and creation logic for ProductCategory Value Object.
  let create (value: string) : Result<T, string> =
       if System.String.IsNullOrWhiteSpace(value) then
        Error "Category cannot be empty."
       else
        Ok (ProductCategory value)

    // Returns the unwrapped string value of the Category.
  let value (instance: T) : string =
    match instance with
    | ProductCategory innerValue -> innerValue

// Represents a validated Unit of Measure.
module UnitOfMeasure =
  type T = private UnitOfMeasure of string
    with
      override this.ToString() =
        match this with
        | UnitOfMeasure stringValue -> stringValue

  // Creates a validated Unit of Measure from a string.
  // TODO: Define validation and creation logic for UnitOfMeasure Value Object.
  let create (value: string) : Result<T, string> =
       if System.String.IsNullOrWhiteSpace(value) then
        Error "Unit of Measure cannot be empty."
       else
        Ok (UnitOfMeasure value)

    // Returns the unwrapped string value of the Unit of Measure.
  let value (instance: T) : string =
    match instance with
    | UnitOfMeasure innerValue -> innerValue

// Represents a validated Unit Value (the quantity within a unit).
module UnitValue =
  type T = private UnitValue of decimal
    with
      override this.ToString() =
        match this with
        | UnitValue decimalValue -> decimalValue.ToString()

  // Creates a validated Unit Value from a decimal.
  // TODO: Define validation and creation logic for UnitValue Value Object.
  let create (value: decimal) : Result<T, string> =
    if value < 0m then
      Error "Unit Value cannot be negative."
    else
      Ok (UnitValue value)

  // Returns the unwrapped decimal value of the Unit Value.
  let value (instance: T) : decimal =
    match instance with
    | UnitValue innerValue -> innerValue

// Represents a validated Weight (e.g., AverageWeightPerUnit).
module Weight =
  type T = private Weight of decimal
    with
      override this.ToString() =
        match this with
        | Weight decimalValue -> decimalValue.ToString()

  // Creates a validated Weight from a decimal.
  // TODO: Define validation and creation logic for Weight Value Object.
  let create (value: decimal) : Result<T, string> =
    if value < 0m then
      Error "Weight cannot be negative."
    else
      Ok (Weight value)

  // Returns the unwrapped decimal value of the Weight.
  let value (instance: T) : decimal =
    match instance with
    | Weight innerValue -> innerValue

// Represents a validated URL for an image.
module ImageUrl =
  type T = private ImageUrl of string
    with
      override this.ToString() =
        match this with
        | ImageUrl stringValue -> stringValue

  // Creates a validated Image URL from a string.
  // TODO: Define validation and creation logic for ImageUrl Value Object.
  let create (value: string) : Result<T, string> =
    let mutable uriResult = Unchecked.defaultof<System.Uri> // Declare mutable variable for out parameter
    if not (System.Uri.TryCreate(value, System.UriKind.Absolute, &uriResult)) then // Pass by reference
         Error "Invalid image URL format."
    else
      Ok (ImageUrl value)

  // Returns the unwrapped string value of the Image URL.
  let value (instance: T) : string =
    match instance with
    | ImageUrl innerValue -> innerValue

// Represents a validated Country Name.
module CountryName =
  type T = private CountryName of string
    with
      override this.ToString() =
        match this with
        | CountryName stringValue -> stringValue

  // Creates a validated Country Name from a string.
  // TODO: Define validation and creation logic for CountryName Value Object.
  let create (value: string) : Result<T, string> =
       if System.String.IsNullOrWhiteSpace(value) then
        Error "Country Name cannot be empty."
       else
        Ok (CountryName value)

    // Returns the unwrapped string value of the Country Name.
  let value (instance: T) : string =
    match instance with
    | CountryName innerValue -> innerValue

