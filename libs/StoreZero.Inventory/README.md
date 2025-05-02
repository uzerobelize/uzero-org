# Online Grocery Store - Inventory Domain Model

This document outlines the key entities for the inventory domain of an online grocery store. The model is designed to track products, their stock levels across various locations, and their movement through processes like procurement, transfers, and returns.

## Entities

### Product

Represents a distinct item available for sale.

* **Attributes:**
    * `productId` (Unique Identifier)
    * `name`
    * `description`
    * `category` (e.g., Dairy, Produce, Bakery)
    * `unitOfMeasure` (e.g., kg, liter, unit, bunch)
    * `brand`
    * `barcodeUpc`
    * `standardRetailPrice`
* **Purpose:** Defines the core characteristics of a sellable item.

### Location

Defines a physical or logical place where inventory is stored or processed.

* **Attributes:**
    * `locationId` (Unique Identifier)
    * `name`
    * `code`
    * `address` (if applicable)
    * `type` (e.g., Warehouse, Fulfillment Center, Retail Store, Receiving Dock, Returns Processing Area)
    * `capacity` (optional)
* **Purpose:** Provides context for the physical location of inventory.

### InventoryItem / StockItem

Represents the quantity of a specific product held at a particular location.

* **Attributes:**
    * `inventoryItemId` (Unique Identifier)
    * `productId` (Link to Product)
    * `locationId` (Link to Location)
    * `quantityOnHand`
    * `minimumStockLevel`
    * `maximumStockLevel`
    * `reorderPoint`
    * `status` (e.g., Available, Held, QualityCheck, Expired)
* **Purpose:** Tracks the current stock level and status of a product at a specific location.

### Lot / Batch

Tracks a specific group of products, often with a common expiry date or origin.

* **Attributes:**
    * `lotId` (Unique Identifier)
    * `productId` (Link to Product)
    * `locationId` (Link to Location)
    * `batchNumber`
    * `expiryDate`
    * `manufacturingDate` (optional)
    * `quantityInLot`
    * `status` (e.g., Available, On Hold, Recalled)
* **Purpose:** Enables fine-grained tracking of inventory, particularly important for perishable goods and recalls.

### Supplier

Represents a vendor from whom products are procured.

* **Attributes:**
    * `supplierId` (Unique Identifier)
    * `name`
    * `contactPerson`
    * `contactInformation` (Phone, Email)
    * `address`
    * `paymentTerms`
* **Purpose:** Manages information about product sources.

### ProductSupplier

Links products to their suppliers, detailing supplier-specific product information.

* **Attributes:**
    * `productSupplierId` (Unique Identifier)
    * `productId` (Link to Product)
    * `supplierId` (Link to Supplier)
    * `supplierProductCode`
    * `unitCost`
    * `minimumOrderQuantity`
    * `leadTime`
* **Purpose:** Represents the many-to-many relationship between Products and Suppliers and stores relevant details for procurement.

### PurchaseOrder

Represents an order placed with a supplier to acquire inventory.

* **Attributes:**
    * `purchaseOrderId` (Unique Identifier)
    * `supplierId` (Link to Supplier)
    * `orderDate`
    * `expectedDispatchDate`
    * `expectedArrivalDate`
    * `destinationLocationId` (Link to Location)
    * `status` (e.g., Draft, Placed, Confirmed, Shipped, Received, Cancelled)
* **Purpose:** Records the intent to purchase inventory from a supplier.

### PurchaseOrderItem

A line item detailing a specific product and quantity within a Purchase Order.

* **Attributes:**
    * `purchaseOrderItemId` (Unique Identifier)
    * `purchaseOrderId` (Link to PurchaseOrder)
    * `productId` (Link to Product)
    * `orderedQuantity`
    * `unitCost`
    * `receivedQuantity`
    * `status` (e.g., Pending, Received Partially, Received Fully, Cancelled)
* **Purpose:** Specifies the items and quantities requested in a purchase order.

### Shipment

Represents the physical movement of goods between locations.

* **Attributes:**
    * `shipmentId` (Unique Identifier)
    * `originLocationId` (Link to Location - can be Supplier or internal)
    * `destinationLocationId` (Link to Location)
    * `shipmentDate`
    * `estimatedArrivalDate`
    * `actualArrivalDate`
    * `carrier`
    * `trackingNumber`
    * `status` (e.g., Manifested, In Transit, Delivered, Delayed)
* **Purpose:** Tracks the logistical process of moving inventory.

### ShipmentItem

A line item detailing the contents of a Shipment.

* **Attributes:**
    * `shipmentItemId` (Unique Identifier)
    * `shipmentId` (Link to Shipment)
    * `productId` (Link to Product)
    * `lotId` (Optional link to Lot)
    * `shippedQuantity`
    * `receivedQuantity`
* **Purpose:** Specifies the items and quantities included in a shipment.

### GoodsReceipt

Documents the formal receiving of inventory at a location.

* **Attributes:**
    * `goodsReceiptId` (Unique Identifier)
    * `receivingLocationId` (Link to Location)
    * `receiptDate`
    * `receivedBy` (Link to User/Employee entity)
    * `sourceDocument` (Link to PurchaseOrder or Shipment)
    * `status` (e.g., Pending Inspection, Received, Put Away)
* **Purpose:** Records the act of receiving incoming inventory.

### GoodsReceiptItem

A line item detailing the specific products and quantities received in a Goods Receipt.

* **Attributes:**
    * `goodsReceiptItemId` (Unique Identifier)
    * `goodsReceiptId` (Link to GoodsReceipt)
    * `productId` (Link to Product)
    * `lotId` (Link to the specific Lot created or updated)
    * `receivedQuantity`
    * `putAwayLocation` (Specific bin/shelf within the Location)
* **Purpose:** Details the items received and their initial placement.

### InventoryTransaction

Records every individual change in the quantity of an InventoryItem.

* **Attributes:**
    * `inventoryTransactionId` (Unique Identifier)
    * `inventoryItemId` (Link to InventoryItem)
    * `lotId` (Link to Lot if applicable)
    * `transactionDate`
    * `transactionTime`
    * `quantityChange` (Positive for increase, Negative for decrease)
    * `transactionType` (e.g., Received, Sold, Adjusted, Transferred Out, Transferred In, Returned, Spoiled, Damaged)
    * `sourceDocument` (Link to the relevant document like GoodsReceiptItem, SalesOrderItem, StockAdjustment, ShipmentItem, ReturnItem)
    * `relatedLocationId` (Link to another Location if movement is involved)
* **Purpose:** Provides a detailed, historical audit trail of all inventory changes.

### StockTransfer

Represents the movement of inventory between your own internal locations.

* **Attributes:**
    * `stockTransferId` (Unique Identifier)
    * `originLocationId` (Link to Location)
    * `destinationLocationId` (Link to Location)
    * `transferDate`
    * `initiatedBy` (Link to User/Employee entity)
    * `status` (e.g., Requested, Packed, Shipped, Received)
* **Purpose:** Manages the process of moving inventory internally between locations.

### StockTransferItem

A line item detailing the specific products and quantities included in a Stock Transfer.

* **Attributes:**
    * `stockTransferItemId` (Unique Identifier)
    * `stockTransferId` (Link to StockTransfer)
    * `productId` (Link to Product)
    * `lotId` (Optional link to Lot)
    * `transferQuantity`
    * `shippedQuantity`
    * `receivedQuantity`
* **Purpose:** Specifies the items and quantities being moved in a stock transfer.

### Return

Represents a customer return that might impact inventory levels. (Often linked to Sales/Order domain).

* **Attributes:**
    * `returnId` (Unique Identifier)
    * `returnDate`
    * `originalOrderId` (Link to Order entity in Sales domain)
    * `customerId` (Link to Customer entity)
    * `status` (e.g., Initiated, Received, Inspected, Restocked, Disposed)
    * `receivingLocationId` (Link to Location where the return is processed)
* **Purpose:** Manages the process of customer returns and their potential impact on inventory.

### ReturnItem

A line item detailing a specific product and quantity included in a Return.

* **Attributes:**
    * `returnItemId` (Unique Identifier)
    * `returnId` (Link to Return)
    * `productId` (Link to Product)
    * `returnedQuantity`
    * `condition` (e.g., Sellable, Damaged, Defective)
    * `actionTaken` (e.g., Restocked, Disposed, Returned to Supplier)
    * `inventoryItemId` (Link to the specific InventoryItem if restocked)
    * `lotId` (Link to the specific Lot if restocked and lot-specific)
* **Purpose:** Details the items returned and their disposition, influencing inventory updates.

### StockAdjustment

Records manual or system-initiated changes to inventory levels for reasons other than standard movements (sales, receipts, transfers, returns).

* **Attributes:**
    * `stockAdjustmentId` (Unique Identifier)
    * `inventoryItemId` (Link to InventoryItem)
    * `lotId` (Link to Lot if applicable)
    * `adjustmentDate`
    * `quantityChange` (Positive or negative)
    * `reason` (e.g., Spoilage, Damage, Loss, Physical Count, Initial Stock)
    * `recordedBy` (Link to User/Employee entity)
* **Purpose:** Provides a record of adjustments to inventory levels for tracking and auditing.

## Relationships

The entities are related to each other to represent the flow and status of inventory. Key relationships include:

* A `Product` can exist as many `InventoryItem`s across different `Location`s.
* An `InventoryItem` at a location can be composed of multiple `Lot`s.
* `PurchaseOrder`s are placed with `Supplier`s and contain `PurchaseOrderItem`s.
* `Shipment`s track the movement of goods, containing `ShipmentItem`s.
* `GoodsReceipt`s document the receiving of `Shipment`s or `PurchaseOrder`s and update `InventoryItem` and `Lot` quantities via `GoodsReceiptItem`s.
* Every change in inventory is recorded as an `InventoryTransaction`, linked to the relevant `InventoryItem`, `Lot`, and source document.
* `StockTransfer`s manage internal movements between `Location`s and consist of `StockTransferItem`s.
* `Return`s from customers, detailed in `ReturnItem`s, can lead to inventory being restocked and recorded via `InventoryTransaction`s.
* `StockAdjustment`s directly modify `InventoryItem` and `Lot` quantities, recorded as `InventoryTransaction`s.
* `ProductSupplier` links `Product` and `Supplier` for procurement details.

This model provides a comprehensive view of the inventory domain, allowing for tracking stock levels, movements, and changes throughout the online grocery store's operations.