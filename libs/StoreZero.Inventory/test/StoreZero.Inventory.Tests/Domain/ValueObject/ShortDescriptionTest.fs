namespace StoreZero.Inventory.Domain.ValueObject.Tests

open NUnit.Framework
open FsUnit
open StoreZero.Inventory.Domain.ValueObject
open System // Required for ArgumentException
open FSharp.Core // For Result.get

type ``ShortDescriptionTests`` () =

    // Helper to create a string of specific length
    let createString length char = String(Array.create length char)

    // Helper to create a valid instance or fail the test
    let createValid input =
        ShortDescription.create input
        // If creation fails, report error and fail the test immediately
        |> Result.mapError (fun e -> failwithf "Test setup failed: %A" e)
        // If Ok, extract the value (safe because error case already handled)
        |> Result.unsafeGetOk

    //-------------------------------------------------------------------------
    // Creation Success Tests (These remain largely the same)
    //-------------------------------------------------------------------------

    [<Test>]
    member _.``create returns Ok for valid simple description`` () =
        let input = "A simple, valid description."
        let result = ShortDescription.create input
        result |> Result.isOk |> should be True
        match result with
        | Ok desc -> ShortDescription.value desc |> should equal input
        | Error e -> Assert.Fail($"Expected Ok, got Error {e}")

    // ... other creation success tests remain the same ...

    [<Test>]
    member _.``create returns Ok for description with Unicode letters/digits`` () =
        let input = "Produkt αβγ 123 Prüfung"
        let result = ShortDescription.create input
        result |> Result.isOk |> should be True
        match result with
        | Ok desc -> ShortDescription.value desc |> should equal input
        | Error e -> Assert.Fail($"Expected Ok, got Error {e}")

    //-------------------------------------------------------------------------
    // Creation Failure Tests (These remain the same)
    //-------------------------------------------------------------------------

    [<Test>]
    member _.``create returns Error for empty string`` () =
        let input = ""
        let result = ShortDescription.create input
        result |> Result.isError |> should be True
        match result with
        | Ok _ -> Assert.Fail("Expected Error, got Ok")
        | Error e -> e |> should equal ShortDescription.ErrorT.IsEmptyOrWhitespace

    // ... other creation failure tests remain the same ...

    [<Test>]
    member _.``create returns Error for string with invalid symbol pipe`` () =
        let input = "No | pipes | allowed."
        let result = ShortDescription.create input
        result |> Result.isError |> should be True
        match result with
        | Ok _ -> Assert.Fail("Expected Error, got Ok")
        | Error (ShortDescription.ErrorT.ContainsInvalidChars chars) ->
            chars |> should equal ['|']
        | Error e -> Assert.Fail($"Expected ContainsInvalidChars, got {e}")

    //-------------------------------------------------------------------------
    // Equality Tests (Refactored to use createValid)
    //-------------------------------------------------------------------------

    [<Test>]
    member _.``Equals returns true for identical valid descriptions`` () =
        let input = "Test Description"
        let d1 = createValid input
        let d2 = createValid input
        d1 |> should equal d2 // Uses overridden Equals

    [<Test>]
    member _.``Equals returns true for descriptions normalizing to same value`` () =
        let input1 = "  normalize   me \n "
        let input2 = "normalize me"
        let d1 = createValid input1
        let d2 = createValid input2
        // Verify normalization worked (using helper and value function)
        (ShortDescription.value d1) |> should equal (ShortDescription.value d2)
        // Verify equality of the value objects
        d1 |> should equal d2

    [<Test>]
    member _.``Equals returns false for different valid descriptions`` () =
        let d1 = createValid "Description A"
        let d2 = createValid "Description B"
        d1 |> should not' (equal d2)

    [<Test>]
    member _.``Equals returns false for different case descriptions`` () =
        // Current implementation uses Ordinal comparison (case-sensitive)
        let d1 = createValid "Case Test"
        let d2 = createValid "case test"
        d1 |> should not' (equal d2)

    [<Test>]
    member _.``Equals returns false when compared to null`` () =
        let d1 = createValid "Valid"
        d1.Equals(null) |> should be False

    [<Test>]
    member _.``Equals returns false when compared to different type`` () =
        let d1 = createValid "Valid"
        d1.Equals(box "Some string") |> should be False

    [<Test>]
    member _.``GetHashCode returns same code for identical descriptions`` () =
        let input = "Test Hash"
        let d1 = createValid input
        let d2 = createValid input
        d1.GetHashCode() |> should equal (d2.GetHashCode())

    [<Test>]
    member _.``GetHashCode returns different codes for different cases`` () =
        // Consistent with case-sensitive Ordinal comparison
        let d1 = createValid "Case Hash"
        let d2 = createValid "case hash"
        d1.GetHashCode() |> should not' (equal (d2.GetHashCode()))

    //-------------------------------------------------------------------------
    // Comparison Tests (Refactored to use createValid)
    //-------------------------------------------------------------------------

    [<Test>]
    member _.``CompareTo returns 0 for identical descriptions`` () =
        let d1 = createValid "Compare A"
        let d2 = createValid "Compare A"
        (d1 :> IComparable<ShortDescription.T>).CompareTo(d2) |> should equal 0

    [<Test>]
    member _.``CompareTo returns less than 0 for A < B`` () =
        let d1 = createValid "Apple"
        let d2 = createValid "Banana"
        (d1 :> IComparable<ShortDescription.T>).CompareTo(d2) |> should be (lessThan 0)

    [<Test>]
    member _.``CompareTo returns greater than 0 for B > A`` () =
        let d1 = createValid "Banana"
        let d2 = createValid "Apple"
        (d1 :> IComparable<ShortDescription.T>).CompareTo(d2) |> should be (greaterThan 0)

    [<Test>]
    member _.``CompareTo is case-sensitive`` () =
        // Based on Ordinal comparer
        let d1 = createValid "apple" // Lowercase 'a' > Uppercase 'A'
        let d2 = createValid "Apple"
        (d1 :> IComparable<ShortDescription.T>).CompareTo(d2) |> should be (greaterThan 0)

    [<Test>]
    member _.``CompareTo obj returns greater than 0 when compared to null`` () =
        let d1 = createValid "Valid"
        let comparableD1 = (d1 :> IComparable) // Intermediate variable
        comparableD1.CompareTo(null) |> should be (greaterThan 0)

    [<Test>]
    member _.``CompareTo obj throws when compared to different type`` () =
        let d1 = createValid "Valid"
        let comparableD1 = (d1 :> IComparable) // Intermediate variable
        let action = fun () -> comparableD1.CompareTo("string") |> ignore
        action
        |> should (throwWithMessage
        "Cannot compare ShortDescription with a different type. (Parameter 'string')"
        ) typeof<ArgumentException>

    //-------------------------------------------------------------------------
    // ToString Tests (Refactored to use createValid)
    //-------------------------------------------------------------------------

    [<Test>]
    member _.``ToString returns the normalized value`` () =
        let input = "  Stringify \n me!  "
        let expected = "Stringify me!"
        let desc = createValid input // Create using helper
        // ToString is implicitly tested here by the `should equal` FsUnit helper
        // when comparing against a string derived from another valid instance
        let expectedDesc = createValid expected
        desc |> should equal expectedDesc
        // Or more explicitly test the ToString() method:
        desc.ToString() |> should equal expected

    //-------------------------------------------------------------------------
    // Value Accessor Tests (Refactored to use createValid)
    //-------------------------------------------------------------------------
    [<Test>]
    member _.``value function returns the underlying normalized string``() =
        let input = " Access me "
        let expected = "Access me"
        let desc = createValid input // Create using helper
        // Explicitly test the module's `value` function
        ShortDescription.value desc |> should equal expected
