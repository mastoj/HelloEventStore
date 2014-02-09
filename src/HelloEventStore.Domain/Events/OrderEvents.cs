﻿using System;

namespace HelloEventStore.Domain.Events
{
    public class OrderCreated
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public OrderCreated(Guid id, Guid userId, Guid productId, int quantity)
        {
            Id = id;
            UserId = userId;
            ProductId = productId;
            Quantity = quantity;
        }

        protected bool Equals(OrderCreated other)
        {
            return Id.Equals(other.Id) && UserId.Equals(other.UserId) && ProductId.Equals(other.ProductId) && Quantity == other.Quantity;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OrderCreated)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ UserId.GetHashCode();
                hashCode = (hashCode * 397) ^ ProductId.GetHashCode();
                hashCode = (hashCode * 397) ^ Quantity;
                return hashCode;
            }
        }
    }

    public class OrderDelivered
    {
        public Guid Id { get; private set; }

        public OrderDelivered(Guid id)
        {
            Id = id;
        }

        protected bool Equals(OrderDelivered other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OrderDelivered) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class OrderCancelled
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public OrderCancelled(Guid id, Guid productId, int quantity)
        {
            Id = id;
            ProductId = productId;
            Quantity = quantity;
        }

        protected bool Equals(OrderCancelled other)
        {
            return Id.Equals(other.Id) && ProductId.Equals(other.ProductId) && Quantity == other.Quantity;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OrderCancelled) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode*397) ^ ProductId.GetHashCode();
                hashCode = (hashCode*397) ^ Quantity;
                return hashCode;
            }
        }
    }
}
