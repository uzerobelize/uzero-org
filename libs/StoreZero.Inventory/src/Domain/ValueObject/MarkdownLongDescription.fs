namespace StoreZero.Inventory.Domain.ValueObject

open System

//-------------------------------------------------------------------------
// Module Definition
//-------------------------------------------------------------------------

// Represents a validated, non-empty, Markdown-formatted long description
// for a product. Ensures minimum length and disallowed character constraints.
// It is designed to be used within the Product Aggregate.
[<RequireQualifiedAccess>]
module MarkdownLongDescription =

    //-------------------------------------------------------------------------
    // Error Type Definition
    //-------------------------------------------------------------------------

    // Defines the possible validation errors when creating a MarkdownLongDescription.
    type ErrorT =
        // The input string was null, empty, or consisted only of whitespace.
        | EmptyOrWhitespace
        // The input string, after normalization, was shorter than the minimum length.
        | TooShort of minLength: int * actualLength: int
        // The input string contained disallowed control characters.
        | DisallowedCharacters

    //-------------------------------------------------------------------------
    // Value Object Type Definition
    //-------------------------------------------------------------------------

    // The internal type representing the validated Markdown description string.
    // Constructor is private to enforce creation via the 'create' function.
    [<CustomEquality; CustomComparison>]
    type T = private MarkdownLongDescription of string
        with
            // --- IEquatable<T> Implementation ---
            interface IEquatable<T> with
                member this.Equals(other) =
                    // Use Ordinal comparison for Markdown content
                    let (MarkdownLongDescription v1) = this
                    let (MarkdownLongDescription v2) = other
                    StringComparer.Ordinal.Equals(v1, v2)

            override this.Equals(obj) =
                match obj with
                | :? T as other -> (this :> IEquatable<T>).Equals(other)
                | _             -> false

            // Hash code consistent with Ordinal comparison
            override this.GetHashCode() =
                let (MarkdownLongDescription v) = this
                StringComparer.Ordinal.GetHashCode(v)

            // --- IComparable<T> Implementation ---
            interface IComparable<T> with
                member this.CompareTo(other) =
                    let (MarkdownLongDescription v1) = this
                    let (MarkdownLongDescription v2) = other
                    // Use Ordinal comparison for sorting
                    StringComparer.Ordinal.Compare(v1, v2)

            // --- IComparable Implementation ---
            interface IComparable with
                member this.CompareTo(obj) =
                    match obj with
                    // If null, this instance is greater
                    | null -> 1
                    // If it's the same type, use the type-safe CompareTo
                    | :? T as other -> (this :> IComparable<T>).CompareTo(other)
                    // Otherwise, it's an invalid comparison
                    | _ ->
                        invalidArg
                            "obj"
                            (sprintf "Cannot compare values of type T and %O" (obj.GetType()))

            override this.ToString() =
                let (MarkdownLongDescription v) = this
                v

    //-------------------------------------------------------------------------
    // Private Constants & Configuration
    //-------------------------------------------------------------------------

    // Minimum required length for a valid description.
    let private minLength = 30

    //-------------------------------------------------------------------------
    // Private Helper Functions
    //-------------------------------------------------------------------------

    // Checks if a character is a disallowed control character (C0/C1 blocks),
    // excluding allowed whitespace (TAB, LF, CR).
    let private isDisallowedControlChar (c: char) : bool =
        match c with
        // Allowed whitespace characters
        | '\u0009' | '\u000A' | '\u000D' -> false
        // Check C0 control block (U+0000 to U+001F)
        | c when c >= '\u0000' && c <= '\u001F' -> true
        // Check C1 control block (U+007F to U+009F) - U+007F is DEL
        | c when c >= '\u007F' && c <= '\u009F' -> true
        // All other characters are allowed
        | _ -> false

    // Checks if the input string contains any disallowed control characters.
    let private containsDisallowedChars (input: string) : bool =
        input |> Seq.exists isDisallowedControlChar

    // Normalizes the input string by trimming whitespace and standardizing
    // line endings to Line Feed (LF, \n).
    let private normalize (input: string) : string =
        input.Trim().Replace("\r\n", "\n").Replace("\r", "\n")

    //-------------------------------------------------------------------------
    // Private ROP Validation Functions
    //-------------------------------------------------------------------------

    // Validation function for ROP: Checks for non-empty/whitespace.
    let private validateNotEmpty (normalizedInput: string) : Result<string, ErrorT> =
        if String.IsNullOrWhiteSpace(normalizedInput) then
            Error EmptyOrWhitespace
        else
            Ok normalizedInput

    // Validation function for ROP: Checks for minimum length.
    let private validateMinLength (input: string) : Result<string, ErrorT> =
        if input.Length < minLength then
            Error (TooShort(minLength = minLength, actualLength = input.Length))
        else
            Ok input

    // Validation function for ROP: Checks for disallowed characters.
    let private validateAllowedChars (input: string) : Result<string, ErrorT> =
        if containsDisallowedChars input then
            Error DisallowedCharacters
        else
            Ok input

    //-------------------------------------------------------------------------
    // Public Factory Function
    //-------------------------------------------------------------------------

    // Factory function to create a MarkdownLongDescription.T value.
    // Performs normalization and validation using Railway Oriented Programming.
    // Returns Ok T if valid, otherwise Error ErrorT.
    let create (rawInput: string) : Result<T, ErrorT> =
        // ROP Pipeline: Normalize -> Validate Not Empty -> Validate Min Length
        // -> Validate Allowed Chars -> Construct T
        Ok rawInput
        |> Result.map normalize // Normalization doesn't fail in ROP sense
        |> Result.bind validateNotEmpty
        |> Result.bind validateMinLength
        |> Result.bind validateAllowedChars
        |> Result.map MarkdownLongDescription // Construct the type

    //-------------------------------------------------------------------------
    // Public Accessor Function
    //-------------------------------------------------------------------------

    // Safely extracts the underlying string value from the T type.
    let value (MarkdownLongDescription internalValue) : string =
        internalValue
