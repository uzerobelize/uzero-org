namespace StoreZero.Inventory.Domain.Tests

open FsUnit // Assuming FsUnit is available
open NUnit.Framework // Assuming NUnit is available
open StoreZero.Inventory.Domain.Entity
open StoreZero.Inventory.Domain.ValueObject // Open the ValueObject namespace
open System // Needed for Guid, DateTimeOffset, Collections.Generic, Decimal

[<TestFixture>]
module ProductEntityTests =

    // --- Unvalidated Tests ---

    [<Test>]
    let ``Unvalidated types with same TemporaryId are equal`` () =
        Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``Unvalidated types with different TemporaryId are not equal`` () =
        Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``Unvalidated GetHashCode is based on TemporaryId`` () =
        Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``Unvalidated GetHashCode is different for different TemporaryId`` () =
        Assert.Fail("Test not implemented yet.")


    // --- ValidationError Tests ---

    [<Test>]
    let ``ValidationError types with same fields are equal`` () =
        Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``ValidationError types with different FieldName are not equal`` () =
        Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``ValidationError types with different ErrorMessage are not equal`` () =
        Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``ValidationError GetHashCode is based on FieldName and ErrorMessage`` () =
        Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``ValidationError GetHashCode is different for different fields`` () =
        Assert.Fail("Test not implemented yet.")


    // --- InvalidProduct Tests ---

    [<Test>]
    let ``InvalidProduct types with same data and errors are equal`` () =
        Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``InvalidProduct types with different UnvalidatedData are not equal`` () =
        Assert.Fail("Test not implemented yet.")


    [<Test>]
    let ``InvalidProduct types with different ValidationErrors are not equal`` () =
        Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``InvalidProduct GetHashCode is based on UnvalidatedData and ValidationErrors`` () =
         Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``InvalidProduct GetHashCode is different for different data or errors`` () =
         Assert.Fail("Test not implemented yet.")


    // --- Validated Tests ---

    [<Test>]
    let ``Validated types with same ProductId are equal`` () =
        Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``Validated types with different ProductId are not equal`` () =
        Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``Validated GetHashCode is based on ProductId`` () =
        Assert.Fail("Test not implemented yet.")

    [<Test>]
    let ``Validated GetHashCode is different for different ProductId`` () =
        Assert.Fail("Test not implemented yet.")

