// tests/StoreZero.Inventory.Domain.Tests/ValueObject/GtinTests.fs
namespace StoreZero.Inventory.Domain.ValueObject.Tests

open NUnit.Framework
open FsUnit

open StoreZero.Inventory.Domain.ValueObject


[<TestFixture>]
type GtinTests () =

    static member private getValueOrFail (result: Result<Gtin.T, Gtin.GtinError>) : Gtin.T =
        match result with
        | Ok gtin -> gtin
        | Error e -> failwith $"getValueOrFail: Expected Ok but got Error: %A{e}"

    // --- Success Cases ---
    [<Test>]
    member this.``Should create Gtin12 for valid 12-digit input`` () =
        let input = "036000291452"
        let result = Gtin.tryCreate input
        result |> Result.isOk |> should be True
        let gtin = GtinTests.getValueOrFail result
        Gtin.value gtin |> should equal input
        Gtin.format gtin |> should equal Gtin.Gtin12Format

    [<Test>]
    member this.``Should create Gtin12 for another valid 12-digit input`` () =
        let input = "012345678905"
        let result = Gtin.tryCreate input
        result |> Result.isOk |> should be True
        let gtin = GtinTests.getValueOrFail result
        Gtin.value gtin |> should equal input
        Gtin.format gtin |> should equal Gtin.Gtin12Format

    [<Test>]
    member this.``Should create Gtin13 for valid 13-digit input`` () =
        let input = "6291041500213"
        let result = Gtin.tryCreate input
        result |> Result.isOk |> should be True
        let gtin = GtinTests.getValueOrFail result
        Gtin.value gtin |> should equal input
        Gtin.format gtin |> should equal Gtin.Gtin13Format

    [<Test>]
    member this.``Should create Gtin13 for ISBN-13 as GTIN-13`` () =
        let input = "9780201379624"
        let result = Gtin.tryCreate input
        result |> Result.isOk |> should be True
        let gtin = GtinTests.getValueOrFail result
        Gtin.value gtin |> should equal input
        Gtin.format gtin |> should equal Gtin.Gtin13Format

    // --- Error Cases ---
    [<TestCase(null, TestName = "Null Input")>]
    [<TestCase("", TestName = "Empty String")>]
    [<TestCase("   ", TestName = "Whitespace Only")>]
    [<TestCase("\t", TestName = "Tab Character")>]
    [<TestCase("\n", TestName = "Newline Character")>]
    member _.``Should return NullOrEmptyInput for invalid inputs`` (input: string) =
        let result = Gtin.tryCreate input
        result |> Result.isError |> should be True
        match result with // Explicitly check the error case
        | Error Gtin.NullOrEmptyInput -> () // Correct error type
        | Error other -> Assert.Fail($"Expected NullOrEmptyInput, got {other}")
        | Ok _ -> Assert.Fail("Expected Error, got Ok")

    [<TestCase("1234567890", 10, TestName = "Length 10")>]
    [<TestCase("12345678901", 11, TestName = "Length 11")>]
    [<TestCase("12345678901234", 14, TestName = "Length 14")>]
    [<TestCase("123456789012345", 15, TestName = "Length 15")>]
    member _.``Should return UnsupportedFormat for non 12 or 13 lengths``
        (input: string, expectedLength: int) =
        let result = Gtin.tryCreate input
        result |> Result.isError |> should be True
        match result with // Explicitly check error case and data
        | Error (Gtin.UnsupportedFormat actualLength) ->
            actualLength |> should equal expectedLength
        | Error other -> Assert.Fail($"Expected UnsupportedFormat({expectedLength}), got {other}")
        | Ok _ -> Assert.Fail("Expected Error, got Ok")


    [<TestCase("03600029145A", TestName = "Letter in GTIN-12")>]
    [<TestCase("629104150021X", TestName = "Letter in GTIN-13")>]
    [<TestCase("03600029145?", TestName = "Symbol")>]
    [<TestCase("03600 0291452", TestName = "Internal Space")>]
    [<TestCase("ABCDEFGHIJKL", TestName = "Letters Only Length 12")>]
    [<TestCase("ABCDEFGHIJKLM", TestName = "Letters Only Length 13")>]
    member _.``Should return NonNumericCharacters for invalid characters``
        (input: string) =
        let result = Gtin.tryCreate input
        result |> Result.isError |> should be True // Check line 82 area
        match result with // Explicitly check error case and data
        | Error (Gtin.NonNumericCharacters actualValue) ->
            actualValue |> should equal input // Check the payload string
        | Error other -> Assert.Fail($"Expected NonNumericCharacters({input}), got {other}")
        | Ok _ -> Assert.Fail("Expected Error, got Ok")


    [<Test>]
    member this.``Should return InvalidCheckDigit for wrong GTIN-12 digit`` () =
        let input = "036000291453" // Valid check digit is 2
        let expectedCheck = 2
        let actualCheck = 3
        let result = Gtin.tryCreate input

        result |> Result.isError |> should be True
        match result with // Explicitly check error case and data
        | Error (Gtin.InvalidCheckDigit (v, exp, act)) ->
            v |> should equal input
            exp |> should equal expectedCheck
            act |> should equal actualCheck
        | Error other -> Assert.Fail($"Expected InvalidCheckDigit, got {other}")
        | Ok _ -> Assert.Fail("Expected Error, got Ok")


    [<Test>]
    member this.``Should return InvalidCheckDigit for wrong GTIN-13 digit`` () =
        let input = "6291041500214" // Valid check digit is 3
        let expectedCheck = 3
        let actualCheck = 4
        let result = Gtin.tryCreate input

        result |> Result.isError |> should be True
        match result with // Explicitly check error case and data
        | Error (Gtin.InvalidCheckDigit (v, exp, act)) ->
            v |> should equal input
            exp |> should equal expectedCheck
            act |> should equal actualCheck
        | Error other -> Assert.Fail($"Expected InvalidCheckDigit, got {other}")
        | Ok _ -> Assert.Fail("Expected Error, got Ok")


    [<Test>]
    member this.``Should return InvalidCheckDigit for GTIN-12 ending 0 wrong`` () =
        let input = "012345678901" // Check digit should be 5
        let expectedCheck = 5
        let actualCheck = 1
        let result = Gtin.tryCreate input

        result |> Result.isError |> should be True
        match result with // Explicitly check error case and data
        | Error (Gtin.InvalidCheckDigit (v, exp, act)) ->
            v |> should equal input
            exp |> should equal expectedCheck
            act |> should equal actualCheck
        | Error other -> Assert.Fail($"Expected InvalidCheckDigit, got {other}")
        | Ok _ -> Assert.Fail("Expected Error, got Ok")


    [<Test>]
    member this.``Should return InvalidCheckDigit for GTIN-13 ending 0 wrong`` () =
        let input = "9780201379625" // Check digit should be 4
        let expectedCheck = 4
        let actualCheck = 5
        let result = Gtin.tryCreate input

        result |> Result.isError |> should be True
        match result with // Explicitly check error case and data
        | Error (Gtin.InvalidCheckDigit (v, exp, act)) ->
            v |> should equal input
            exp |> should equal expectedCheck
            act |> should equal actualCheck
        | Error other -> Assert.Fail($"Expected InvalidCheckDigit, got {other}")
        | Ok _ -> Assert.Fail("Expected Error, got Ok")
