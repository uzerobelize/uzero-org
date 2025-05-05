namespace StoreZero.Inventory.Domain.ValueObject

[<RequireQualifiedAccess>]
module Gtin =

    type GtinError =
        | NullOrEmptyInput
        | UnsupportedFormat of actualLength: int
        | NonNumericCharacters of value: string
        | InvalidCheckDigit of value: string * expected: int * actual: int

    type T =
        private
        | Gtin12 of string
        | Gtin13 of string

    // ----------------------------------------------------------------------
    // Internal Validation Helper Functions
    // ----------------------------------------------------------------------

    let private validateNotNullOrWhitespace
        (candidateValue: string) : Result<string, GtinError> =
        if System.String.IsNullOrWhiteSpace(candidateValue) then
            Error NullOrEmptyInput
        else
            Ok candidateValue

    let private validateLength
        (candidateValue: string) : Result<string, GtinError> =
        let len = candidateValue.Length
        if len <> 12 && len <> 13 then
            Error (UnsupportedFormat len)
        else
            Ok candidateValue

    let private validateNumeric
        (candidateValue: string) : Result<string, GtinError> =
        if String.forall System.Char.IsDigit candidateValue then
            Ok candidateValue
        else
            Error (NonNumericCharacters candidateValue)

    let private getWeight (totalDigitsBeforeCheck: int) (position: int) : int =
        if totalDigitsBeforeCheck = 12 then // GTIN-13 logic
            if position % 2 = 0 then 3 else 1
        else // GTIN-12 logic
            if position % 2 <> 0 then 3 else 1

    // Calculates the expected check digit using Seq.indexed |> Seq.fold.
    // Takes the digits *excluding* the check digit itself.
    let private calculateExpectedCheckDigit
        (digitsWithoutCheckDigit: string) : int =
        let n = digitsWithoutCheckDigit.Length

        let totalSum =
            digitsWithoutCheckDigit
            |> Seq.indexed
            |> Seq.fold (fun currentSum (index, digitChar) ->
                let digit = int digitChar - int '0'
                let position = index + 1 // Position is 1-based
                let weight = getWeight n position
                currentSum + (digit * weight)
            ) 0

        let remainder = totalSum % 10
        if remainder = 0 then 0 else 10 - remainder

    let private validateCheckDigit
        (candidateValue: string) : Result<string, GtinError> =
        let n = candidateValue.Length
        let digitsWithoutCheckDigit = candidateValue.[0 .. n-2]
        let actualCheckDigitChar = candidateValue.[n-1]
        let actualCheckDigit = int actualCheckDigitChar - int '0'

        let expectedCheckDigit =
            calculateExpectedCheckDigit digitsWithoutCheckDigit

        if actualCheckDigit <> expectedCheckDigit then
            Error (
                InvalidCheckDigit (
                    candidateValue, expectedCheckDigit, actualCheckDigit
                )
            )
        else
            Ok candidateValue

    let private createGtinT (validatedValue: string) : T =
        if validatedValue.Length = 12 then
            Gtin12 validatedValue
        else
            Gtin13 validatedValue

    // ----------------------------------------------------------------------
    // Public Factory Function
    // ----------------------------------------------------------------------
    let create (candidateValue: string) : Result<T, GtinError> =
        validateNotNullOrWhitespace candidateValue
        |> Result.bind validateLength
        |> Result.bind validateNumeric
        |> Result.bind validateCheckDigit
        |> Result.map createGtinT

    // ----------------------------------------------------------------------
    // Public Accessor Functions
    // ----------------------------------------------------------------------
    let value (gtin: T) : string =
        match gtin with
        | Gtin12 s -> s
        | Gtin13 s -> s

    type GtinFormat = Gtin12Format | Gtin13Format

    let format (gtin: T) : GtinFormat =
         match gtin with
         | Gtin12 _ -> Gtin12Format
         | Gtin13 _ -> Gtin13Format
