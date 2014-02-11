namespace HelloEventStore.Domain.Commands
open HelloEventStore.Infrastructure
open System

type PlaceOrder = 
    {UserId: Guid; ProductId: Guid; Quantity: int}
    with interface ICommand

type DeliverOrder =
    {Id: Guid}
    with interface ICommand

type CancelOrder = 
    {Id: Guid}
    with interface ICommand

type AddProductToInventory = 
    {ProductName: string; Quantity: int}
    with interface ICommand

type UpdateProductInventory =
    {Id: Guid; QuantityChange: int}
    with interface ICommand

type ChangeName = 
    {Id: Guid; NewName: string}
    with interface ICommand

type CreateUser = 
    {UserName: string; Name: string}
    with interface ICommand
