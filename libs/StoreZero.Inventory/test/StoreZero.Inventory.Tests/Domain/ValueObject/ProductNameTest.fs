namespace StoreZero.Inventory.Domain.ValueObject.Tests

open NUnit.Framework
open FsUnit // Provides the 'should' syntax
open StoreZero.Inventory.Domain.ValueObject // Access ProductName module
open System // For IComparable tests

// Helper to extract Ok value when sure it's Ok (use after assertion)
module Result =
    let unsafeGetOk = function | Ok v -> v | Error e -> failwithf "%A" e
    let unsafeGetError = function | Error e -> e | Ok _ -> failwith "Expected Error"

[<TestFixture>]
type ``ProductNameTests``() =

    let minLen = 4 // Match the implementation detail for tests
    let maxLen = 150 // Match the implementation detail for tests

    // Helper to create a valid name for comparison/equality tests
    let createValid name =
        ProductName.create name
        |> Result.mapError (fun e -> failwithf "Test setup failed: %A" e)
        |> Result.unsafeGetOk

    //-------------------------------------------------------------------------
    // Success Cases
    //-------------------------------------------------------------------------

    [<Test>]
    member _.``Create succeeds for valid typical name``() =
        let input = "Standard Widget Pro"
        let result = ProductName.create input
        result |> Result.isOk |> should be True
        let name = result |> Result.unsafeGetOk
        ProductName.value name |> should equal "Standard Widget Pro"

    [<Test>]
    member _.``Create succeeds for minimum length name``() =
        let input = String.replicate minLen "a" // e.g., "aaaa"
        let result = ProductName.create input
        result |> Result.isOk |> should be True
        ProductName.value (result |> Result.unsafeGetOk) |> should equal input

    [<Test>]
    member _.``Create succeeds for maximum length name``() =
        let input = String.replicate maxLen "a"
        let result = ProductName.create input
        result |> Result.isOk |> should be True
        ProductName.value (result |> Result.unsafeGetOk) |> should equal input

    [<Test>]
    member _.``Create succeeds with allowed symbols``() =
        let input = "Widget-Pro_v2 & Co. (Ltd.) / 'Special'"
        let result = ProductName.create input
        result |> Result.isOk |> should be True
        ProductName.value (result |> Result.unsafeGetOk) |> should equal input

    [<Test>]
    member _.``Create succeeds with numbers``() =
        let input = "Product Model 12345"
        let result = ProductName.create input
        result |> Result.isOk |> should be True
        ProductName.value (result |> Result.unsafeGetOk) |> should equal input

    [<Test>]
    member _.``Create succeeds with unicode letters``() =
        let input = "Produit Étonnant Vörstellung" // Example unicode
        let result = ProductName.create input
        result |> Result.isOk |> should be True
        ProductName.value (result |> Result.unsafeGetOk) |> should equal input

    //-------------------------------------------------------------------------
    // Normalization Cases
    //-------------------------------------------------------------------------

    [<Test>]
    member _.``Create normalizes leading and trailing whitespace``() =
        let input = "  Spaces Around  "
        let expected = "Spaces Around"
        let result = ProductName.create input
        result |> Result.isOk |> should be True
        ProductName.value (result |> Result.unsafeGetOk) |> should equal expected

    [<Test>]
    member _.``Create normalizes collapsed internal whitespace``() =
        let input = "Multiple   Spaces  Inside"
        let expected = "Multiple Spaces Inside"
        let result = ProductName.create input
        result |> Result.isOk |> should be True
        ProductName.value (result |> Result.unsafeGetOk) |> should equal expected

    [<Test>]
    member _.``Create normalizes mixed whitespace``() =
        let input = " \t Mixed\r\nWhitespace\t Test  "
        let expected = "Mixed Whitespace Test" // Collapses \t, \r\n etc. to space
        let result = ProductName.create input
        result |> Result.isOk |> should be True
        ProductName.value (result |> Result.unsafeGetOk) |> should equal expected

    //-------------------------------------------------------------------------
    // Failure Cases: IsNullOrWhitespace
    //-------------------------------------------------------------------------

    [<TestCase(null, TestName = "Create fails for null input")>]
    [<TestCase("", TestName = "Create fails for empty string")>]
    [<TestCase("   ", TestName = "Create fails for whitespace only")>]
    [<TestCase("\t\n ", TestName = "Create fails for mixed whitespace only")>]
    member _.``Create fails for null or whitespace input``(input: string) =
        let result = ProductName.create input
        result |> Result.isError |> should be True
        result |> Result.unsafeGetError
        |> should equal ProductName.ErrorT.IsNullOrWhitespace

    //-------------------------------------------------------------------------
    // Failure Cases: TooShort
    //-------------------------------------------------------------------------

    [<TestCase("abc", 3, TestName = "Create fails for length 3")>]
    [<TestCase(" a ", 1, TestName = "Create fails length 1 after trim")>]
    [<TestCase("a b", 3, TestName = "Create fails length 3 after collapse")>]
    member _.``Create fails for names shorter than min length``
        (input: string) (expectedLength: int) =
        let result = ProductName.create input
        result |> Result.isError |> should be True
        result |> Result.unsafeGetError
        |> should equal (ProductName.ErrorT.TooShort (minLen, expectedLength))

    //-------------------------------------------------------------------------
    // Failure Cases: TooLong
    //-------------------------------------------------------------------------

    [<Test>]
    member _.``Create fails for names longer than max length``() =
        let input = String.replicate (maxLen + 1) "a"
        let result = ProductName.create input
        result |> Result.isError |> should be True
        result |> Result.unsafeGetError
        |> should equal (ProductName.ErrorT.TooLong (maxLen, maxLen + 1))

    //-------------------------------------------------------------------------
    // Failure Cases: ContainsInvalidCharacters
    //-------------------------------------------------------------------------

    [<TestCase("Name <Tag>", [|'<'; '>'|], TestName = "Invalid Char Angle Brackets")>]
    [<TestCase("Name | Pipe", [|'|'|], TestName = "Invalid Char Pipe")>]
    [<TestCase("Name \\ Backslash", [|'\\'|], TestName = "Invalid Char Backslash")>]
    [<TestCase("Name;", [|';'|], TestName = "Invalid Symbol Semicolon")>]
    [<TestCase("Name ^ Caret", [|'^'|], TestName = "Invalid Symbol Caret")>]
    [<TestCase("!Exclaim", [|'!'|], TestName = "Invalid Symbol Exclamation")>]
    [<TestCase("Multi<|>Invalid", [|'<'; '|'; '>'|], TestName = "Multiple Invalid Chars")>]
    member _.``Create fails for names with invalid characters``
        (input: string) (expectedInvalid: char[]) = // Parameter remains char[]
        let result = ProductName.create input
        result |> Result.isError |> should be True
        let error = result |> Result.unsafeGetError
        match error with
        | ProductName.ErrorT.ContainsInvalidCharacters chars -> // chars is list
            // Convert both actual list and expected array to Sets for comparison
            let actualSet = Set.ofList chars
            let expectedSet = Set.ofArray expectedInvalid
            actualSet |> should equal expectedSet // Compare sets
        | other ->
            failwithf "Expected ContainsInvalidCharacters, but got %A" other

    //-------------------------------------------------------------------------
    // Equality Cases (Case-Insensitive)
    //-------------------------------------------------------------------------

    [<Test>]
    member _.``Equality holds for identical normalized values``() =
        let name1 = createValid "Test Name"
        let name2 = createValid "Test Name"
        name1 = name2 |> should be True
        name1.Equals(name2) |> should be True // Explicit IEquatable
        (box name1).Equals(box name2) |> should be True // object.Equals

    [<Test>]
    member _.``Equality holds for different casing``() =
        let name1 = createValid "Test Name"
        let name2 = createValid "test name"
        name1 = name2 |> should be True
        name1.GetHashCode() = name2.GetHashCode() |> should be True

    [<Test>]
    member _.``Equality holds for names differing only by whitespace``() =
        let name1 = createValid "  Test   Name  "
        let name2 = createValid "Test Name"
        name1 = name2 |> should be True
        name1.GetHashCode() = name2.GetHashCode() |> should be True

    [<Test>]
    member _.``Equality is false for different normalized values``() =
        let name1 = createValid "Test Name One"
        let name2 = createValid "Test Name Two"
        name1 = name2 |> should be False

    [<Test>]
    member _.``Equality is false when compared to other types or null``() =
        let name1 = createValid "Test Name"
        (box name1).Equals(null) |> should be False
        (box name1).Equals("Test Name") |> should be False // Compare T to string
        (box name1).Equals(123) |> should be False // Compare T to int

    //-------------------------------------------------------------------------
    // Comparison Cases (Case-Insensitive, IComparable<T>)
    //-------------------------------------------------------------------------

    [<Test>]
    member _.``CompareTo T returns 0 for identical normalized values``() =
        let name1 = createValid "Test Name"
        let name2 = createValid "Test Name"
        (name1 :> IComparable<ProductName.T>).CompareTo(name2)
        |> should equal 0

    [<Test>]
    member _.``CompareTo T returns 0 for different casing``() =
        let name1 = createValid "Test Name"
        let name2 = createValid "test name"
        (name1 :> IComparable<ProductName.T>).CompareTo(name2)
        |> should equal 0

    [<Test>]
    member _.``CompareTo T returns 0 for different whitespace``() =
        let name1 = createValid "  Test   Name  "
        let name2 = createValid "Test Name"
        (name1 :> IComparable<ProductName.T>).CompareTo(name2)
        |> should equal 0

    [<Test>]
    member _.``CompareTo T returns less than 0 for name1 < name2``() =
        let name1 = createValid "Apple Pie"
        let name2 = createValid "Banana Bread"
        (name1 :> IComparable<ProductName.T>).CompareTo(name2)
        |> should be (lessThan 0)

    [<Test>]
    member _.``CompareTo T returns greater than 0 for name1 > name2``() =
        let name1 = createValid "Orange Juice"
        let name2 = createValid "Mango Lassi"
        (name1 :> IComparable<ProductName.T>).CompareTo(name2)
        |> should be (greaterThan 0)

    [<Test>]
    member _.``CompareTo T is case insensitive for less than``() =
        let name1 = createValid "apple Pie" // Lower 'a'
        let name2 = createValid "Banana Bread" // Upper 'B'
        (name1 :> IComparable<ProductName.T>).CompareTo(name2)
        |> should be (lessThan 0)

    [<Test>]
    member _.``CompareTo T is case insensitive for greater than``() =
        let name1 = createValid "ORANGE Juice" // Upper 'O'
        let name2 = createValid "mango Lassi" // Lower 'm'
        (name1 :> IComparable<ProductName.T>).CompareTo(name2)
        |> should be (greaterThan 0)

    //-------------------------------------------------------------------------
    // Comparison Cases (Case-Insensitive, IComparable)
    //-------------------------------------------------------------------------

    [<Test>]
    member _.``CompareTo obj returns 0 for same logical instance``() =
        let name1 = createValid "Test Name"
        let name2 = box (createValid "test name") // Boxed, different case
        (name1 :> IComparable).CompareTo(name2)
        |> should equal 0

    [<Test>]
    member _.``CompareTo obj returns less than 0 for name1 < name2``() =
        let name1 = createValid "Apple Pie"
        let name2 = box (createValid "Banana Bread")
        (name1 :> IComparable).CompareTo(name2)
        |> should be (lessThan 0)

    [<Test>]
    member _.``CompareTo obj returns greater than 0 for name1 > name2``() =
        let name1 = createValid "Orange Juice"
        let name2 = box (createValid "Mango Lassi")
        (name1 :> IComparable).CompareTo(name2)
        |> should be (greaterThan 0)

    [<Test>]
    member _.``CompareTo obj returns greater than 0 when compared to null``() =
        let name1 = createValid "Test Name"
        (name1 :> IComparable).CompareTo(null)
        |> should be (greaterThan 0)

    [<Test>]
    member _.``CompareTo obj throws ArgumentException for incompatible type``() =
        let name1 = createValid "Test Name"
        let invalidInput = "a string"
        let expectedType = "String"
        let expectedMessage =
            $"{ProductName.invalidCompareArgMessage expectedType} (Parameter '{invalidInput}')"
        let action () = (name1 :> IComparable).CompareTo(invalidInput) |> ignore
        // Use infix should with (throwWithMessage "substring") and typeof<ExceptionType>
        action
        |> should (throwWithMessage expectedMessage) typeof<System.ArgumentException>

    [<Test>]
    member _.``CompareTo obj throws ArgumentException for another incompatible type``() =
        let name1 = createValid "Test Name"
        let invalidInput = 123
        let expectedType = "Int32"
        let expectedMessage = $"{ProductName.invalidCompareArgMessage expectedType} (Parameter '{invalidInput}')"
        let action () = (name1 :> IComparable).CompareTo(invalidInput) |> ignore
        // Use infix should with (throwWithMessage "substring") and typeof<ExceptionType>
        action
        |> should (throwWithMessage expectedMessage) typeof<System.ArgumentException>
