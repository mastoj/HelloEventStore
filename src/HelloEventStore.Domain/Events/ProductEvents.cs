using System;

namespace HelloEventStore.Domain.Events
{
    public class OutOfProduct
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public OutOfProduct(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        protected bool Equals(OutOfProduct other)
        {
            return Id.Equals(other.Id) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OutOfProduct) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode()*397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }


    public class ProductQuantityDecreased : ProductQuantityChanged
    {
        public ProductQuantityDecreased(Guid id, int quantityChange, int initialQuantity) : base(id, quantityChange, initialQuantity)
        {
        }
    }

    public class ProductQuantityIncreased : ProductQuantityChanged
    {
        public ProductQuantityIncreased(Guid id, int quantityChange, int initialQuantity) : base(id, quantityChange, initialQuantity)
        {
        }
    }
    public abstract class ProductQuantityChanged
    {
        public int InitialQuantity { get; private set; }
        public int QuantityChange { get; private set; }
        public Guid Id { get; private set; }

        public ProductQuantityChanged(Guid id, int quantityChange, int initialQuantity)
        {
            Id = id;
            QuantityChange = quantityChange;
            InitialQuantity = initialQuantity;
        }

        protected bool Equals(ProductQuantityChanged other)
        {
            return InitialQuantity == other.InitialQuantity && QuantityChange == other.QuantityChange && Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProductQuantityChanged)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = InitialQuantity;
                hashCode = (hashCode * 397) ^ QuantityChange;
                hashCode = (hashCode * 397) ^ Id.GetHashCode();
                return hashCode;
            }
        }
    }

    public class ProductAddedToInventory
    {
        public Guid Id { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }

        public ProductAddedToInventory(Guid id, string productName, int quantity)
        {
            Id = id;
            ProductName = productName;
            Quantity = quantity;
        }

        protected bool Equals(ProductAddedToInventory other)
        {
            return Id.Equals(other.Id) && string.Equals(ProductName, other.ProductName) && Quantity == other.Quantity;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProductAddedToInventory) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode*397) ^ (ProductName != null ? ProductName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Quantity;
                return hashCode;
            }
        }
    }
}