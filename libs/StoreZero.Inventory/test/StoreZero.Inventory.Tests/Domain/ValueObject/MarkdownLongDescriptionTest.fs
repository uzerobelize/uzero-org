namespace StoreZero.Inventory.Domain.ValueObject.Tests

open System
open NUnit.Framework
open FsUnit
open StoreZero.Inventory.Domain.ValueObject

[<TestFixture>]
type MarkdownLongDescriptionTests() =

    // Helper to extract value from Result, failing test if Error
    let getValueOrFail (result: Result<MarkdownLongDescription.T, MarkdownLongDescription.ErrorT>) : MarkdownLongDescription.T =
        match result with
        | Ok v -> v
        | Error e ->
            let msg = $"Expected Ok but got Error: {e}"
            Assert.Fail(msg)
            failwith msg

    // Helper to extract error from Result, failing test if Ok
    let getErrorOrFail (result: Result<MarkdownLongDescription.T, MarkdownLongDescription.ErrorT>) : MarkdownLongDescription.ErrorT =
        match result with
        | Ok v ->
            let msg = $"Expected Error but got Ok: {v}"
            Assert.Fail(msg)
            failwith msg
        | Error e -> e

    [<Test>]
    member _.``Create succeeds for valid simple description``() =
        let input = "This is a perfectly valid markdown description, long enough."
        let result = MarkdownLongDescription.create input
        result |> Result.isOk |> should be True
        let valueObject = getValueOrFail result
        MarkdownLongDescription.value valueObject |> should equal input

    [<Test>]
    member _.``Create succeeds for valid description with markdown formatting``() =
        let input = "## Title\n\n* Item 1\n* Item 2\n\nSome **bold** text."
        let result = MarkdownLongDescription.create input
        result |> Result.isOk |> should be True
        let valueObject = getValueOrFail result
        MarkdownLongDescription.value valueObject |> should equal input

    [<Test>]
    member _.``Create succeeds for input with exactly minimum length``() =
        let input = System.String('a', 30)
        let result = MarkdownLongDescription.create input
        result |> Result.isOk |> should be True
        MarkdownLongDescription.value (getValueOrFail result) |> should equal input

    [<Test>]
    member _.``Create succeeds and normalizes input with surrounding whitespace``() =
        let input = "  \t Starts and ends with whitespace and tabs. \t \n "
        let expected = "Starts and ends with whitespace and tabs."
        let result = MarkdownLongDescription.create input
        result |> Result.isOk |> should be True
        let valueObject = getValueOrFail result
        MarkdownLongDescription.value valueObject |> should equal expected

    [<Test>]
    member _.``Create succeeds and normalizes mixed line endings to LF``() =
        let input = "Line 1\r\nLine 2\rLine 3\nLong enough line ending test."
        let expected = "Line 1\nLine 2\nLine 3\nLong enough line ending test."
        let result = MarkdownLongDescription.create input
        result |> Result.isOk |> should be True
        let valueObject = getValueOrFail result
        MarkdownLongDescription.value valueObject |> should equal expected

    [<TestCase("", TestName = "Create fails for empty string")>]
    [<TestCase("   ", TestName = "Create fails for string with only spaces")>]
    [<TestCase("\t\n ", TestName = "Create fails for string with only whitespace chars")>]
    member _.``Create fails for empty or whitespace input``(input: string) =
        let result = MarkdownLongDescription.create input
        result |> Result.isError |> should be True
        let error = getErrorOrFail result
        error |> should equal MarkdownLongDescription.ErrorT.EmptyOrWhitespace

    [<Test>]
    member _.``Create fails for input shorter than minimum length``() =
        let input = "Too short"
        let result = MarkdownLongDescription.create input
        result |> Result.isError |> should be True
        let error = getErrorOrFail result
        match error with
        | MarkdownLongDescription.ErrorT.TooShort (min, actual) ->
            min |> should equal 30
            actual |> should equal (input.Length)
        | _ -> Assert.Fail($"Expected TooShort error but got {error}")

    [<Test>]
    member _.``Create fails for input shorter than minimum length after normalization``() =
        let input = "              Short text               "
        let result = MarkdownLongDescription.create input
        result |> Result.isError |> should be True
        let error = getErrorOrFail result
        match error with
        | MarkdownLongDescription.ErrorT.TooShort (min, actual) ->
            min |> should equal 30
            actual |> should equal (input.Trim().Length)
        | _ -> Assert.Fail($"Expected TooShort error but got {error}")

    [<TestCase("Valid but with \u0000 null char an just long enough.", TestName = "Create fails for input with null char (U+0000)")>]
    [<TestCase("Valid but with \u001F unit separator", TestName = "Create fails for input with unit separator (U+001F)")>]
    [<TestCase("Valid but with \u007F delete character.", TestName = "Create fails for input with delete char (U+007F)")>]
    [<TestCase("Valid but with \u0080 control char.", TestName = "Create fails for input with C1 control char (U+0080)")>]
    member _.``Create fails for input containing disallowed control characters``(input: string) =
        let longInput = input + System.String(' ', 30)
        let result = MarkdownLongDescription.create longInput
        result |> Result.isError |> should be True
        let error = getErrorOrFail result
        error |> should equal MarkdownLongDescription.ErrorT.DisallowedCharacters

    [<Test>]
    member _.``Create succeeds for input containing allowed control characters``() =
        let input = "Line with\ttab.\nLine with\nnewline.\rCarriage return.\nLong enough."
        let result = MarkdownLongDescription.create input
        result |> Result.isOk |> should be True
        let expected = "Line with\ttab.\nLine with\nnewline.\nCarriage return.\nLong enough."
        MarkdownLongDescription.value (getValueOrFail result) |> should equal expected

    [<Test>]
    member _.``Value accessor returns the normalized string``() =
        let input = "   Needs normalization for testing value accessor. Min length ok.   "
        let expected = "Needs normalization for testing value accessor. Min length ok."
        let result = MarkdownLongDescription.create input
        let valueObject = getValueOrFail result
        let actual = MarkdownLongDescription.value valueObject
        actual |> should equal expected

    [<Test>]
    member _.``Equality returns true for instances created with same valid input``() =
        let input = "Identical input string, long enough for validation."
        let vo1 = MarkdownLongDescription.create input |> getValueOrFail
        let vo2 = MarkdownLongDescription.create input |> getValueOrFail
        vo1 |> should equal vo2
        (vo1.GetHashCode()) |> should equal (vo2.GetHashCode())

    [<Test>]
    member _.``Equality returns false for instances created with different valid input``() =
        let input1 = "First valid input string, long enough for validation."
        let input2 = "Second valid input string, also long enough."
        let vo1 = MarkdownLongDescription.create input1 |> getValueOrFail
        let vo2 = MarkdownLongDescription.create input2 |> getValueOrFail
        vo1 |> should not' (equal vo2)

    [<Test>]
    member _.``Equality is case-sensitive due to Ordinal comparison``() =
        let input1 = "Case Difference Test String, long enough."
        let input2 = "case difference test string, long enough."
        let vo1 = MarkdownLongDescription.create input1 |> getValueOrFail
        let vo2 = MarkdownLongDescription.create input2 |> getValueOrFail
        vo1 |> should not' (equal vo2)

    [<Test>]
    member _.``Equality returns false when comparing with null``() =
        let input = "Valid input string, long enough for validation."
        let vo = MarkdownLongDescription.create input |> getValueOrFail
        vo.Equals(null) |> should be False

    [<Test>]
    member _.``Equality returns false when comparing with a different type``() =
        let input = "Valid input string, long enough for validation."
        let vo = MarkdownLongDescription.create input |> getValueOrFail
        let other = 123
        vo.Equals(other) |> should be False

    [<Test>]
    member _.``Comparison behaves correctly based on Ordinal string comparison``() =
        let appleLower = "apple description long enough to pass validation"
        let appleUpper = "Apple description long enough to pass validation"
        let banana = "banana description long enough to pass validation"

        let voAppleLower = MarkdownLongDescription.create appleLower |> getValueOrFail
        let voAppleUpper = MarkdownLongDescription.create appleUpper |> getValueOrFail
        let voBanana = MarkdownLongDescription.create banana |> getValueOrFail
        let voAppleLower2 = MarkdownLongDescription.create appleLower |> getValueOrFail

        // Ordinal comparison: "A" < "a" < "b"
        voAppleUpper |> should be (lessThan voAppleLower)
        voAppleLower |> should be (lessThan voBanana)
        voAppleUpper |> should be (lessThan voBanana)

        voAppleLower |> should be (greaterThan voAppleUpper)
        voBanana |> should be (greaterThan voAppleLower)
        voBanana |> should be (greaterThan voAppleUpper)

        voAppleLower |> should equal voAppleLower2

        // Explicit CompareTo requires casting to the interface
        (voAppleUpper :> IComparable<MarkdownLongDescription.T>).CompareTo(voAppleLower)
        |> should be (lessThan 0)
        (voAppleLower :> IComparable<MarkdownLongDescription.T>).CompareTo(voAppleUpper)
        |> should be (greaterThan 0)
        (voAppleLower :> IComparable<MarkdownLongDescription.T>).CompareTo(voAppleLower2)
        |> should equal 0

    [<Test>]
    member _.``Comparison with null using IComparable returns 1``() =
        let input = "Valid input string, long enough for validation."
        let vo : MarkdownLongDescription.T = MarkdownLongDescription.create input |> getValueOrFail
        let comparable = vo :> System.IComparable

        (fun () -> comparable.CompareTo(null) |> ignore)
        |> should not' (throw typeof<System.Exception>)

        comparable.CompareTo(null) |> should equal 1

    [<Test>]
    member _.``Comparison with a different type using IComparable throws ArgumentException``() =
        let input = "Valid input string, long enough for validation."
        let vo : MarkdownLongDescription.T = MarkdownLongDescription.create input |> getValueOrFail
        let comparable = vo :> System.IComparable
        let other = box 42
        let expectedExactMsg = "Cannot compare values of type T and System.Int32 (Parameter 'obj')"

        (fun () -> comparable.CompareTo(other) |> ignore)
        |> should (throwWithMessage expectedExactMsg) typeof<System.ArgumentException>
