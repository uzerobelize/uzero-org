// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
namespace StoreZero.Inventory.Domain.ValueObject

open System
open System.Text.RegularExpressions // Explicit open for Regex

// Encapsulates validation rules and behavior for a Product's
// Short Description.
// Ensures descriptions are non-empty, within length limits, contain
// allowed characters, and are normalized consistently.
[<RequireQualifiedAccess>]
module ShortDescription =
    //-------------------------------------------------------------------------
    // Constants
    //-------------------------------------------------------------------------
    let maxLength = 160

    // Define the set of allowed punctuation and symbols.
    // This makes character validation efficient.
    let private allowedPunctuationAndSymbols =
        Set.ofList [
            // Essential Punctuation
            '.'; ','; '?'; '!'; '\''; '"';
            // Connectors & Groupers
            '-'; ':'; ';'; '('; ')'; '/';
            // Branding Symbols
            '&'; '™'; '®'; '©';
            // Common Symbols
            '$'; '€'; '£'; '%'; '+'; '*'; '#'
            // Note: Space ' ' is handled separately in isAllowedChar
        ]

    //-------------------------------------------------------------------------
    // Error Type Definition
    //-------------------------------------------------------------------------
    // Represents the specific reasons why a ShortDescription
    // could not be created.
    type ErrorT =
        | IsEmptyOrWhitespace
        | ExceedsMaxLength of maxLength: int * actualLength: int
        | ContainsInvalidChars of invalidChars: char list

    //-------------------------------------------------------------------------
    // Value Object Type Definition
    //-------------------------------------------------------------------------
    // Represents a validated, normalized Product Short Description.
    // Use the 'create' function for construction.
    [<CustomEquality; CustomComparison>]
    type T = private ShortDescription of string // Constructor name is ShortDescription
        with
            // --- Equality & Comparison ---
            interface IEquatable<T> with
                member this.Equals(other) =
                    let (ShortDescription v1) = this
                    let (ShortDescription v2) = other
                    // Compare normalized strings using Ordinal (case-sensitive)
                    StringComparer.Ordinal.Equals(v1, v2)

            override this.Equals(obj) =
                match obj with
                | :? T as other -> (this :> IEquatable<T>).Equals(other)
                | _ -> false

            // Hash code consistent with Ordinal comparison
            override this.GetHashCode() =
                let (ShortDescription v) = this
                StringComparer.Ordinal.GetHashCode(v)

            // --- Comparison Implementation ---
            interface IComparable<T> with
                member this.CompareTo(other) =
                    let (ShortDescription v1) = this
                    let (ShortDescription v2) = other
                    // Use Ordinal comparison consistent with Equals/GetHashCode
                    StringComparer.Ordinal.Compare(v1, v2)

            interface IComparable with
                member this.CompareTo(obj) =
                    match obj with
                    | :? T as other -> (this :> IComparable<T>).CompareTo(other)
                    // Follow standard comparison: non-null instance > null instance
                    | null -> 1
                    // Cannot compare with unrelated types
                    | _ ->
                        invalidArg
                            (obj.ToString())
                            "Cannot compare ShortDescription with a different type."

            // Provides a string representation (useful for debugging/logging)
            override this.ToString() =
                let (ShortDescription v) = this
                v

    //-------------------------------------------------------------------------
    // Public Accessor Function
    //-------------------------------------------------------------------------
    // Provides access to the validated and normalized string value.
    let value (desc: T) : string =
        let (ShortDescription v) = desc // Pattern match to extract
        v

    //-------------------------------------------------------------------------
    // Private Helper Functions
    //-------------------------------------------------------------------------

    // Normalizes the input string according to defined rules:
    // 1. Trim leading/trailing whitespace.
    // 2. Replace various newline sequences with a single space.
    // 3. Collapse multiple internal whitespace chars into a single space.
    let private normalize (input: string) =
        let trimmedAndNewlinesReplaced =
            input.Trim()
                 .Replace("\r\n", " ") // Chain Replace calls directly
                 .Replace("\n", " ")
                 .Replace("\r", " ")
        // Collapse multiple whitespace chars (incl. space) into one space
        // Use the static Regex.Replace method explicitly
        Regex.Replace(trimmedAndNewlinesReplaced, @"\s+", " ")


    // Checks if a character is permitted in the short description.
    let private isAllowedChar (c: char) =
        Char.IsLetterOrDigit(c) // Unicode letters and standard digits
        || c = ' ' // Allow space explicitly
        || Set.contains c allowedPunctuationAndSymbols

    // Validation function: Ensure string is not null, empty, or whitespace.
    // Operates on the already normalized string.
    let private validateNotEmpty (s: string) =
        // Post-normalization, IsNullOrEmpty is sufficient check
        if String.IsNullOrEmpty s then
            Error ErrorT.IsEmptyOrWhitespace
        else
            Ok s

    // Validation function: Ensure string does not exceed max length.
    // Curried function to easily apply the maxLength constant.
    let private validateLength (maxLen: int) (s: string) =
        if s.Length > maxLen then
            Error (ErrorT.ExceedsMaxLength (maxLen, s.Length))
        else
            Ok s

    // Validation function: Ensure string contains only allowed characters.
    let private validateCharacters (s: string) =
        let invalidChars =
            s
            |> Seq.filter (fun c -> not (isAllowedChar c))
            |> Seq.distinct // Report each invalid char only once
            |> Seq.toList

        if List.isEmpty invalidChars then
            Ok s
        else
            Error (ErrorT.ContainsInvalidChars invalidChars)


    //-------------------------------------------------------------------------
    // Public Factory Function
    //-------------------------------------------------------------------------

    // Attempts to create a valid ShortDescription instance.
    // Applies normalization and validation rules.
    // Follows Railway Oriented Programming using Result<'T, 'Error>.
    // Note: Expects a non-null input string. Optionality should be
    // handled by the calling code/entity holding this value object
    // (e.g., using `Option<ShortDescription.T>`).
    let create (input: string) : Result<T, ErrorT> =
        // Apply normalization first
        let normalized = normalize input

        // Chain validation functions using Result.bind
        Ok normalized // Start the railway with the normalized value
        |> Result.bind validateNotEmpty
        |> Result.bind (validateLength maxLength) // Apply max length constraint
        |> Result.bind validateCharacters
        // If all validations pass, wrap the result in the private constructor
        |> Result.map ShortDescription // CORRECTED: Use private constructor name
