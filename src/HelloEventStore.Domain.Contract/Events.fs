namespace HelloEventStore.Domain.Events
open HelloEventStore.Infrastructure
open System

type OrderCreated = 
    {Id: Guid; UserId: Guid; ProductId: Guid; Quantity: int}

type OrderDelivered =
    {Id: Guid}

type OrderCancelled = 
    {Id: Guid; ProductId: Guid; Quantity: int}

type ProductQuantityDecreased = 
    {Id: Guid; QuantityChange: int; InitialQuantity: int}

type ProductQuantityIncreased = 
    {Id: Guid; QuantityChange: int; InitialQuantity: int}

type ProductAddedToInventory =
    {Id: Guid; ProductName: string; Quantity: int}

type OutOfProduct =
    {Id: Guid; Name: string}

type NameChanged = 
    {Id: Guid; NewName: string}

type UserCreated = 
    {Id: Guid; UserName: string; Name: string}

type NeedsApproval = 
    {Id: Guid; UserId: Guid; ProductId: Guid; Quantity: int}

type OrderApproved = 
    { Id: Guid }