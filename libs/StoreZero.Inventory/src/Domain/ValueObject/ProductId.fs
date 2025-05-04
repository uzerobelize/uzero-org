namespace StoreZero.Inventory.Domain.ValueObject

open System // Required for Guid

module ProductId =

    // Describes potential errors when creating or parsing a ProductId.
    type ErrorT =
        | EmptyGuidProvided         // An attempt was made to use Guid.Empty.
        | InvalidGuidString of string // The input string was not a valid format.

    // Represents the unique identifier for a Product Aggregate Root.
    // Defined inside the module, enforces creation via module functions.
    // Single-case DU ensures type safety. Private constructor (`private`).
    // Equality, HashCode, and Comparison rely on F#'s default structural behavior
    // for single-case DUs wrapping Guid. ToString() provides default representation.
    type T = private ProductId of Guid
        // No explicit overrides for ToString, Equals, GetHashCode, IComparable

    // Creates a new, unique ProductId instance with a generated GUID.
    // This operation is guaranteed to succeed.
    let create () : T =
        ProductId (Guid.NewGuid())

    // Attempts to create a ProductId from an existing Guid value.
    // Returns Error if the provided Guid is Guid.Empty.
    let parseGuid (guid: Guid) : Result<T, ErrorT> =
        if guid = Guid.Empty then
            Error EmptyGuidProvided
        else
            Ok (ProductId guid)

    // Attempts to parse a string into a ProductId.
    // Returns Error if the string is not a valid GUID format
    // or if the parsed GUID is Guid.Empty.
    let parseString (guidString: string) : Result<T, ErrorT> =
        match Guid.TryParse guidString with
        | true, guid when guid <> Guid.Empty ->
            Ok (ProductId guid) // Calls the private DU constructor
        | true, _ -> // Represents Guid.TryParse succeeded but yielded Guid.Empty
            Error EmptyGuidProvided
        | false, _ ->
            Error (InvalidGuidString guidString)

    // Extracts the underlying Guid value from a ProductId instance.
    let value (productId: T) : Guid =
        let (ProductId guid) = productId // Use pattern matching
        guid
