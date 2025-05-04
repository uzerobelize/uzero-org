namespace StoreZero.Inventory.Domain.Tests

open NUnit.Framework
open FsUnit
open StoreZero.Inventory.Domain.ValueObject
open System // Needed for Guid, Decimal

[<TestFixture>]
module ValueObjectTests =

    [<Test>]
    let ``Sku create returns Ok for valid string`` () =
        let validSku = "SKU12345"
        let result = Sku.create validSku
        match result with
        | Ok sku -> Sku.value sku |> should equal validSku
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``Sku create returns Error for empty string`` () =
        let invalidSku = ""
        let result = Sku.create invalidSku
        result |> should equal (Error "SKU cannot be empty.")

    [<Test>]
    let ``Sku create returns Error for whitespace string`` () =
        let invalidSku = "   "
        let result = Sku.create invalidSku
        result |> should equal (Error "SKU cannot be empty.")

    [<Test>]
    let ``Sku value unwraps the string`` () =
        let testSku = "TESTSKU"
        let sku = Sku.create testSku |> Result.toOption |> Option.get
        Sku.value sku |> should equal testSku

    [<Test>]
    let ``Sku ToString returns string representation`` () =
        let testSku = "TESTSKU"
        let sku = Sku.create testSku |> Result.toOption |> Option.get
        sku.ToString() |> should equal testSku

    [<Test>]
    let ``BarcodeUpc create returns Ok for valid string`` () =
        let validBarcode = "123456789012"
        let result = BarcodeUpc.create validBarcode
        match result with
        | Ok barcode -> BarcodeUpc.value barcode |> should equal validBarcode
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``BarcodeUpc create returns Error for empty string`` () =
        let invalidBarcode = ""
        let result = BarcodeUpc.create invalidBarcode
        result |> should equal (Error "Barcode/UPC cannot be empty.")

    [<Test>]
    let ``BarcodeUpc create returns Error for whitespace string`` () =
        let invalidBarcode = "   "
        let result = BarcodeUpc.create invalidBarcode
        result |> should equal (Error "Barcode/UPC cannot be empty.")

    [<Test>]
    let ``BarcodeUpc value unwraps the string`` () =
        let testBarcode = "987654321098"
        let barcode = BarcodeUpc.create testBarcode |> Result.toOption |> Option.get
        BarcodeUpc.value barcode |> should equal testBarcode

    [<Test>]
    let ``BarcodeUpc ToString returns string representation`` () =
        let testBarcode = "987654321098"
        let barcode = BarcodeUpc.create testBarcode |> Result.toOption |> Option.get
        barcode.ToString() |> should equal testBarcode

    [<Test>]
    let ``ProductName create returns Ok for valid string`` () =
        let validName = "Test Product Name"
        let result = ProductName.create validName
        match result with
        | Ok productName -> ProductName.value productName |> should equal validName
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``ProductName create returns Error for empty string`` () =
        let invalidName = ""
        let result = ProductName.create invalidName
        result |> should equal (Error "Product Name cannot be empty.")

    [<Test>]
    let ``ProductName create returns Error for whitespace string`` () =
        let invalidName = "   "
        let result = ProductName.create invalidName
        result |> should equal (Error "Product Name cannot be empty.")

    [<Test>]
    let ``ProductName value unwraps the string`` () =
        let testName = "Another Product"
        let productName = ProductName.create testName |> Result.toOption |> Option.get
        ProductName.value productName |> should equal testName

    [<Test>]
    let ``ProductName ToString returns string representation`` () =
        let testName = "Another Product"
        let productName = ProductName.create testName |> Result.toOption |> Option.get
        productName.ToString() |> should equal testName

    [<Test>]
    let ``ProductDescription create returns Ok for valid string`` () =
        let validDescription = "This is a description."
        let result = ProductDescription.create validDescription
        match result with
        | Ok description -> ProductDescription.value description |> should equal validDescription
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``ProductDescription create returns Ok for empty string`` () =
        let emptyDescription = ""
        let result = ProductDescription.create emptyDescription
        match result with
        | Ok description -> ProductDescription.value description |> should equal emptyDescription
        | Error _ -> failwith "Expected Ok result"


    [<Test>]
    let ``ProductDescription create returns Ok for whitespace string`` () =
        let whitespaceDescription = "   "
        let result = ProductDescription.create whitespaceDescription
        match result with
        | Ok description -> ProductDescription.value description |> should equal ""
        | Error _ -> failwith "Expected Ok result"


    [<Test>]
    let ``ProductDescription value unwraps the string`` () =
        let testDescription = "Test description."
        let productDescription = ProductDescription.create testDescription |> Result.toOption |> Option.get
        ProductDescription.value productDescription |> should equal testDescription

    [<Test>]
    let ``ProductDescription ToString returns string representation`` () =
        let testDescription = "Test description."
        let productDescription = ProductDescription.create testDescription |> Result.toOption |> Option.get
        productDescription.ToString() |> should equal testDescription

    [<Test>]
    let ``BrandName create returns Ok for valid string`` () =
        let validBrand = "BrandX"
        let result = BrandName.create validBrand
        match result with
        | Ok brandName -> BrandName.value brandName |> should equal validBrand
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``BrandName create returns Error for empty string`` () =
        let invalidBrand = ""
        let result = BrandName.create invalidBrand
        result |> should equal (Error "Brand Name cannot be empty.")

    [<Test>]
    let ``BrandName create returns Error for whitespace string`` () =
        let invalidBrand = "   "
        let result = BrandName.create invalidBrand
        result |> should equal (Error "Brand Name cannot be empty.")

    [<Test>]
    let ``BrandName value unwraps the string`` () =
        let testBrand = "BrandY"
        let brandName = BrandName.create testBrand |> Result.toOption |> Option.get
        BrandName.value brandName |> should equal testBrand

    [<Test>]
    let ``BrandName ToString returns string representation`` () =
        let testBrand = "BrandY"
        let brandName = BrandName.create testBrand |> Result.toOption |> Option.get
        brandName.ToString() |> should equal testBrand

    [<Test>]
    let ``ManufacturerName create returns Ok for valid string`` () =
        let validManufacturer = "ManuCorp"
        let result = ManufacturerName.create validManufacturer
        match result with
        | Ok manufacturerName -> ManufacturerName.value manufacturerName |> should equal validManufacturer
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``ManufacturerName create returns Error for empty string`` () =
        let invalidManufacturer = ""
        let result = ManufacturerName.create invalidManufacturer
        result |> should equal (Error "Manufacturer Name cannot be empty.")

    [<Test>]
    let ``ManufacturerName create returns Error for whitespace string`` () =
        let invalidManufacturer = "   "
        let result = ManufacturerName.create invalidManufacturer
        result |> should equal (Error "Manufacturer Name cannot be empty.")

    [<Test>]
    let ``ManufacturerName value unwraps the string`` () =
        let testManufacturer = "ManuInc"
        let manufacturerName = ManufacturerName.create testManufacturer |> Result.toOption |> Option.get
        ManufacturerName.value manufacturerName |> should equal testManufacturer

    [<Test>]
    let ``ManufacturerName ToString returns string representation`` () =
        let testManufacturer = "ManuInc"
        let manufacturerName = ManufacturerName.create testManufacturer |> Result.toOption |> Option.get
        manufacturerName.ToString() |> should equal testManufacturer

    [<Test>]
    let ``ProductCategory create returns Ok for valid string`` () =
        let validCategory = "Dairy"
        let result = ProductCategory.create validCategory
        match result with
        | Ok category -> ProductCategory.value category |> should equal validCategory
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``ProductCategory create returns Error for empty string`` () =
        let invalidCategory = ""
        let result = ProductCategory.create invalidCategory
        result |> should equal (Error "Category cannot be empty.")

    [<Test>]
    let ``ProductCategory create returns Error for whitespace string`` () =
        let invalidCategory = "   "
        let result = ProductCategory.create invalidCategory
        result |> should equal (Error "Category cannot be empty.")

    [<Test>]
    let ``ProductCategory value unwraps the string`` () =
        let testCategory = "Produce"
        let productCategory = ProductCategory.create testCategory |> Result.toOption |> Option.get
        ProductCategory.value productCategory |> should equal testCategory

    [<Test>]
    let ``ProductCategory ToString returns string representation`` () =
        let testCategory = "Produce"
        let productCategory = ProductCategory.create testCategory |> Result.toOption |> Option.get
        productCategory.ToString() |> should equal testCategory

    [<Test>]
    let ``UnitOfMeasure create returns Ok for valid string`` () =
        let validUnit = "kg"
        let result = UnitOfMeasure.create validUnit
        match result with
        | Ok unit -> UnitOfMeasure.value unit |> should equal validUnit
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``UnitOfMeasure create returns Error for empty string`` () =
        let invalidUnit = ""
        let result = UnitOfMeasure.create invalidUnit
        result |> should equal (Error "Unit of Measure cannot be empty.")

    [<Test>]
    let ``UnitOfMeasure create returns Error for whitespace string`` () =
        let invalidUnit = "   "
        let result = UnitOfMeasure.create invalidUnit
        result |> should equal (Error "Unit of Measure cannot be empty.")

    [<Test>]
    let ``UnitOfMeasure value unwraps the string`` () =
        let testUnit = "liter"
        let unitOfMeasure = UnitOfMeasure.create testUnit |> Result.toOption |> Option.get
        UnitOfMeasure.value unitOfMeasure |> should equal testUnit

    [<Test>]
    let ``UnitOfMeasure ToString returns string representation`` () =
        let testUnit = "liter"
        let unitOfMeasure = UnitOfMeasure.create testUnit |> Result.toOption |> Option.get
        unitOfMeasure.ToString() |> should equal testUnit

    [<Test>]
    let ``UnitValue create returns Ok for non-negative decimal`` () =
        let validValue = 1.5m
        let result = UnitValue.create validValue
        match result with
        | Ok unitValue -> UnitValue.value unitValue |> should equal validValue
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``UnitValue create returns Ok for zero decimal`` () =
        let validValue = 0m
        let result = UnitValue.create validValue
        match result with
        | Ok unitValue -> UnitValue.value unitValue |> should equal validValue
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``UnitValue create returns Error for negative decimal`` () =
        let invalidValue = -1.0m
        let result = UnitValue.create invalidValue
        result |> should equal (Error "Unit Value cannot be negative.")

    [<Test>]
    let ``UnitValue value unwraps the decimal`` () =
        let testValue = 10.99m
        let unitValue = UnitValue.create testValue |> Result.toOption |> Option.get
        UnitValue.value unitValue |> should equal testValue

    [<Test>]
    let ``UnitValue ToString returns decimal string representation`` () =
        let testValue = 5.0m
        let unitValue = UnitValue.create testValue |> Result.toOption |> Option.get
        unitValue.ToString() |> should equal (testValue.ToString())

    [<Test>]
    let ``Weight create returns Ok for non-negative decimal`` () =
        let validWeight = 0.75m
        let result = Weight.create validWeight
        match result with
        | Ok weight -> Weight.value weight |> should equal validWeight
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``Weight create returns Ok for zero decimal`` () =
        let validWeight = 0m
        let result = Weight.create validWeight
        match result with
        | Ok weight -> Weight.value weight |> should equal validWeight
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``Weight create returns Error for negative decimal`` () =
        let invalidWeight = -0.1m
        let result = Weight.create invalidWeight
        result |> should equal (Error "Weight cannot be negative.")

    [<Test>]
    let ``Weight value unwraps the decimal`` () =
        let testWeight = 2.5m
        let weight = Weight.create testWeight |> Result.toOption |> Option.get
        Weight.value weight |> should equal testWeight

    [<Test>]
    let ``Weight ToString returns decimal string representation`` () =
        let testWeight = 1.0m
        let weight = Weight.create testWeight |> Result.toOption |> Option.get
        weight.ToString() |> should equal (testWeight.ToString())

    [<Test>]
    let ``ImageUrl create returns Ok for valid URL`` () =
        let validUrl = "https://example.com/image.jpg"
        let result = ImageUrl.create validUrl
        match result with
        | Ok imageUrl -> ImageUrl.value imageUrl |> should equal validUrl
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``ImageUrl create returns Error for invalid URL format`` () =
        let invalidUrl = "not a url"
        let result = ImageUrl.create invalidUrl
        result |> should equal (Error "Invalid image URL format.")

    [<Test>]
    let ``ImageUrl value unwraps the string`` () =
        let testUrl = "http://test.com/img.png"
        let imageUrl = ImageUrl.create testUrl |> Result.toOption |> Option.get
        ImageUrl.value imageUrl |> should equal testUrl

    [<Test>]
    let ``ImageUrl ToString returns string representation`` () =
        let testUrl = "https://images.test.net/photo.gif"
        let imageUrl = ImageUrl.create testUrl |> Result.toOption |> Option.get
        imageUrl.ToString() |> should equal testUrl

    [<Test>]
    let ``CountryName create returns Ok for valid string`` () =
        let validCountry = "Canada"
        let result = CountryName.create validCountry
        match result with
        | Ok countryName -> CountryName.value countryName |> should equal validCountry
        | Error _ -> failwith "Expected Ok result"

    [<Test>]
    let ``CountryName create returns Error for empty string`` () =
        let invalidCountry = ""
        let result = CountryName.create invalidCountry
        result |> should equal (Error "Country Name cannot be empty.")

    [<Test>]
    let ``CountryName create returns Error for whitespace string`` () =
        let invalidCountry = "   "
        let result = CountryName.create invalidCountry
        result |> should equal (Error "Country Name cannot be empty.")

    [<Test>]
    let ``CountryName value unwraps the string`` () =
        let testCountry = "USA"
        let countryName = CountryName.create testCountry |> Result.toOption |> Option.get
        CountryName.value countryName |> should equal testCountry

    [<Test>]
    let ``CountryName ToString returns string representation`` () =
        let testCountry = "Mexico"
        let countryName = CountryName.create testCountry |> Result.toOption |> Option.get
        countryName.ToString() |> should equal testCountry
