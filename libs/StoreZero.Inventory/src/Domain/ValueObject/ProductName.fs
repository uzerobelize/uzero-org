namespace StoreZero.Inventory.Domain.ValueObject

open System // For String, Char, IEquatable, IComparable, StringComparer
open System.Text.RegularExpressions // For whitespace normalization

[<RequireQualifiedAccess>]
module ProductName =

    let invalidCompareArgMessage (typeName: string): string =
        $"Cannot compare ProductName with '{typeName}'"

    //-------------------------------------------------------------------------
    // Type Definitions
    //-------------------------------------------------------------------------

    // Represents the validated, canonical product name.
    // Use 'ProductName.create' to construct instances.
    [<CustomEquality; CustomComparison>] // Attributes already present
    type T = private ProductName of string
        with
            // --- IEquatable Implementation (Existing) ---
            interface System.IEquatable<T> with
                member this.Equals(other) =
                    // Case-insensitive comparison using the normalized value
                    let (ProductName v1) = this
                    let (ProductName v2) = other
                    System.StringComparer.OrdinalIgnoreCase.Equals(v1, v2)

            // --- Overrides (Existing) ---
            override this.Equals(obj) =
                match obj with
                | :? T as other -> (this :> System.IEquatable<T>).Equals(other)
                | _ -> false

            override this.GetHashCode() =
                let (ProductName v) = this
                System.StringComparer.OrdinalIgnoreCase.GetHashCode(v)

            // --- IComparable<T> Implementation (NEW) ---
            interface System.IComparable<T> with
                member this.CompareTo(other) =
                    let (ProductName v1) = this
                    let (ProductName v2) = other
                    // Case-insensitive comparison for ordering, consistent with Equals
                    System.StringComparer.OrdinalIgnoreCase.Compare(v1, v2)

            // --- IComparable Implementation (NEW) ---
            interface System.IComparable with
                member this.CompareTo(obj) =
                    match obj with
                    | :? T as other ->
                        // Delegate to the generic version
                        (this :> System.IComparable<T>).CompareTo(other)
                    | null -> 1 // Standard convention: instances are greater than null
                    | _ ->
                        // Cannot compare with unrelated types
                        let typeName = obj.GetType().Name
                        invalidArg (obj.ToString()) (invalidCompareArgMessage typeName)

    // Describes errors that can occur during ProductName creation.
    type ErrorT =
        | IsNullOrWhitespace
        | TooShort of minLength: int * actualLength: int
        | TooLong of maxLength: int * actualLength: int
        | ContainsInvalidCharacters of invalidChars: char list

    //-------------------------------------------------------------------------
    // Constants and Configuration
    //-------------------------------------------------------------------------

    let private minLength = 4
    let private maxLength = 150

    // Allowed characters: Unicode Letters, Digits, and specific symbols
    // Note: Space is handled separately after normalization.
    let private isAllowedSymbol (c: char) : bool =
        match c with
        | '-' | '_' | '&' | '\'' | '.' | ',' | '(' | ')' | '/' | ' ' -> true
        | _ -> false

    let private isAllowedCharacter (c: char) : bool =
        Char.IsLetterOrDigit(c) || isAllowedSymbol(c)

    //-------------------------------------------------------------------------
    // Private Helper Functions
    //-------------------------------------------------------------------------

    // Normalizes whitespace: trims ends and collapses internal spaces.
    let private normalize (input: string) : string =
        let trimmed = input.Trim()
        // Replace multiple whitespace chars (including space, tab, newline etc)
        // with a single space.
        Regex.Replace(trimmed, @"\s+", " ")

    // Validates the length of the normalized string.
    let private validateLength (name: string) : Result<string, ErrorT> =
        let len = name.Length
        if len < minLength then
            Error (TooShort (minLength, len))
        elif len > maxLength then
            Error (TooLong (maxLength, len))
        else
            Ok name

    // Validates that all characters in the string are allowed.
    let private validateCharacters (name: string)
        : Result<string, ErrorT> =
        let invalidChars =
            name
            |> Seq.filter (fun c -> not (isAllowedCharacter c))
            |> Seq.distinct
            |> Seq.toList

        if List.isEmpty invalidChars then
            Ok name
        else
            Error (ContainsInvalidCharacters invalidChars)

    //-------------------------------------------------------------------------
    // Public API Functions
    //-------------------------------------------------------------------------

    // Creates a new ProductName instance from a raw string.
    // Performs normalization and validation.
    // Returns Result.Ok with the ProductName on success,
    // or Result.Error with a ErrorT on failure.
    let create (rawName: string) : Result<T, ErrorT> =
        if String.IsNullOrWhiteSpace rawName then
            Error IsNullOrWhitespace
        else
            Ok rawName
            |> Result.map normalize
            |> Result.bind validateLength
            |> Result.bind validateCharacters
            |> Result.map ProductName

    // Extracts the underlying normalized string value from a ProductName.
    let value (ProductName name) : string =
        name
