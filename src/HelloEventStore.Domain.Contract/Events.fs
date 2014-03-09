namespace HelloEventStore.Domain.Events
open HelloEventStore.Infrastructure
open System

type OrderCreated = 
    {Id: Guid; UserId: Guid; ProductId: Guid; Quantity: int}
    with interface IEvent

type OrderDelivered =
    {Id: Guid}
    with interface IEvent

type OrderCancelled = 
    {Id: Guid; ProductId: Guid; Quantity: int}
    with interface IEvent

type ProductQuantityDecreased = 
    {Id: Guid; QuantityChange: int; InitialQuantity: int}
    with interface IEvent

type ProductQuantityIncreased = 
    {Id: Guid; QuantityChange: int; InitialQuantity: int}
    with interface IEvent

type ProductAddedToInventory =
    {Id: Guid; ProductName: string; Quantity: int}
    with interface IEvent

type OutOfProduct =
    {Id: Guid; Name: string}
    with interface IEvent

type NameChanged = 
    {Id: Guid; NewName: string}
    with interface IEvent

type UserCreated = 
    {Id: Guid; UserName: string; Name: string}
    with interface IEvent
