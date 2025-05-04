namespace StoreZero.Inventory.Domain.ValueObject.Tests

open NUnit.Framework
open FsUnit // Import FsUnit assertions
open StoreZero.Inventory.Domain.ValueObject // Module with T, ProductId.ErrorT
open System // For Guid

[<TestFixture>]
module ProductIdTests =

    // Helper to fail test if result is Error, otherwise return Ok value
    let private getOkValue (result: Result<ProductId.T, ProductId.ErrorT>) =
        match result with
        | Ok value -> value
        | Error e ->
            Assert.Fail(sprintf "Expected Ok, but got Error: %A" e)
            Unchecked.defaultof<ProductId.T>

    // Helper to fail test if result is Ok, otherwise return Error value
    let private getErrorValue (result: Result<ProductId.T, ProductId.ErrorT>) =
        match result with
        | Ok v ->
            Assert.Fail(sprintf "Expected Error, but got Ok: %A" v)
            Unchecked.defaultof<ProductId.ErrorT>
        | Error e -> e

    // --- Creation Tests ---

    [<Test>]
    let ``create generates a non-empty ProductId`` () =
        let productIdT = ProductId.create()
        let guidVal = ProductId.value productIdT
        guidVal |> should not' (equal Guid.Empty)

    // --- parseGuid Tests ---

    [<Test>]
    let ``parseGuid with valid non-empty Guid succeeds`` () =
        let validGuid = Guid.NewGuid()
        let result = ProductId.parseGuid validGuid
        result |> Result.isOk |> should be True
        let productIdT = getOkValue result
        ProductId.value productIdT |> should equal validGuid

    [<Test>]
    let ``parseGuid with empty Guid fails`` () =
        let emptyGuid = Guid.Empty
        let result = ProductId.parseGuid emptyGuid
        result |> Result.isError |> should be True
        let error = getErrorValue result
        error |> should equal ProductId.ErrorT.EmptyGuidProvided

    // --- parseString Tests ---

    [<Test>]
    let ``parseString with valid non-empty Guid string succeeds`` () =
        let validGuid = Guid.NewGuid()
        let validGuidString = validGuid.ToString("D") // Canonical format
        let result = ProductId.parseString validGuidString
        result |> Result.isOk |> should be True
        let productIdT = getOkValue result
        ProductId.value productIdT |> should equal validGuid

    [<Test>]
    let ``parseString with different valid formats succeeds`` () =
        let validGuid = Guid.NewGuid()
        let formats = [ "D"; "N"; "B"; "P" ]
        for fmt in formats do
            let guidString = validGuid.ToString(fmt)
            let result = ProductId.parseString guidString
            result |> Result.isOk |> should be True
            let productIdT = getOkValue result
            ProductId.value productIdT |> should equal validGuid

    [<Test>]
    let ``parseString with empty Guid string fails`` () =
        let emptyGuidString = Guid.Empty.ToString("D")
        let result = ProductId.parseString emptyGuidString
        result |> Result.isError |> should be True
        let error = getErrorValue result
        error |> should equal ProductId.ErrorT.EmptyGuidProvided

    [<Test>]
    let ``parseString with invalid string fails`` () =
        let invalidString = "this-is-not-a-valid-guid"
        let result = ProductId.parseString invalidString
        result |> Result.isError |> should be True
        let error = getErrorValue result
        error |> should equal (ProductId.ErrorT.InvalidGuidString invalidString)

    [<Test>]
    let ``parseString with null string fails`` () =
        let result = ProductId.parseString null
        result |> Result.isError |> should be True
        let error = getErrorValue result
        error |> should equal (ProductId.ErrorT.InvalidGuidString null)

    // --- Value Accessor Tests ---

    [<Test>]
    let ``value function returns correct Guid`` () =
        let guid = Guid.NewGuid()
        let productIdT = ProductId.parseGuid guid |> getOkValue
        ProductId.value productIdT |> should equal guid

    // --- Equality Tests (Relying on default structural equality) ---

    [<Test>]
    let ``ProductIds with same Guid are equal`` () =
        let guid = Guid.NewGuid()
        let id1 = ProductId.parseGuid guid |> getOkValue
        let id2 = ProductId.parseGuid guid |> getOkValue
        id1 |> should equal id2 // Default equality should compare wrapped Guid
        (id1 = id2) |> should be True
        (id1 <> id2) |> should be False

    [<Test>]
    let ``ProductIds with different Guids are not equal`` () =
        let id1 = ProductId.create()
        let id2 = ProductId.create()
        id1 |> should not' (equal id2)
        (id1 = id2) |> should be False
        (id1 <> id2) |> should be True

    [<Test>]
    let ``ProductId T is not equal to other types (using default Equals)`` () =
        let id1 = ProductId.create()
        // Default obj.Equals implementation for DUs should handle type checks
        id1.Equals("some string") |> should be False
        id1.Equals(box 123) |> should be False
        id1.Equals(null) |> should be False

    // --- Comparison Tests (Using standard F# 'compare' function) ---

    [<Test>]
    let ``Standard compare function works correctly based on underlying Guid`` () =
        let bytes1 = Array.zeroCreate<byte> 16
        bytes1.[15] <- byte 1 // Ensure guid2 > guid1
        let bytes2 = Array.zeroCreate<byte> 16
        bytes2.[15] <- byte 2 // Ensure guid2 > guid1
        let bytes3 = Array.zeroCreate<byte> 16
        bytes3.[15] <- byte 3 // Ensure guid3 > guid2

        let guid1 = Guid(bytes1)
        let guid2 = Guid(bytes2)
        let guid3 = Guid(bytes3)

        let id1 = ProductId.parseGuid guid1 |> getOkValue
        let id2 = ProductId.parseGuid guid2 |> getOkValue
        let id3 = ProductId.parseGuid guid3 |> getOkValue

        // Use F# compare function - structural comparison delegates to Guid
        (compare id1 id2) |> should be (lessThan 0) // i.e., < 0
        (compare id2 id1) |> should be (greaterThan 0) // i.e., > 0
        (compare id1 id1) |> should equal 0    // i.e., = 0
        (compare id2 id3) |> should be (lessThan 0) // i.e., < 0

    // --- GetHashCode Tests (Relying on default structural hashing) ---

    [<Test>]
    let ``Equal ProductIds have same HashCode`` () =
        let guid = Guid.NewGuid()
        let id1 = ProductId.parseGuid guid |> getOkValue
        let id2 = ProductId.parseGuid guid |> getOkValue
        // Default hash code should be based on wrapped Guid's hash code
        id1.GetHashCode() |> should equal (id2.GetHashCode())
