using System;
using HelloEventStore.Domain.Commands;
using HelloEventStore.Domain.Events;
using HelloEventStore.Domain.Exceptions;
using HelloEventStore.Domain.Services;
using HelloEventStore.Infrastructure.Exceptions;
using NUnit.Framework;

namespace HelloEventStore.Tests
{
    [TestFixture]
	public class OrderTests : TestBase
	{
		public void PlaceOrder_OrderPlaced_IfUserExist()
		{
			var userId = Guid.NewGuid();
			var productId = Guid.NewGuid();
			var quantity = 5;
//            var placeOrder = new PlaceOrder()
		}
	}

	[TestFixture]
	public class UserTests : TestBase
	{
		[Test]
		public void CreateUser_UserCreated_IfNotExist()
		{
			Guid id = Guid.NewGuid();
			string userName = "userName";
			string name = "name";
			IdGenerator.GuidGenerator = () => id;

			var createUser = new CreateUser(userName, name);

			When(createUser);
			Then(new UserCreated(id, userName, name));
		}

		[Test]
		public void CreateUser_ThrowsException_IfUserNameAlreadyTaken()
		{
			var id = Guid.NewGuid();
			var takenName = "takenName";
			var name = "name";
			UserView.Insert(id, takenName);

			var createUser = new CreateUser(takenName, name);
			WhenThrows<UserNameTakenException>(createUser);
		}

		[Test]
		public void ChangeName_ThrowsException_IfUserDoesNotExist()
		{
			var newName = "newName";
			var id = Guid.NewGuid();
			var changeName = new ChangeName(id, newName);

			WhenThrows<AggregateNotFoundException>(changeName);
		}

		[Test]
		public void ChangeName_ChangedName_IfUserExistAndNameIsDifferent()
		{
			var newName = "newName";
			var id = Guid.NewGuid();
			var changeName = new ChangeName(id, newName);

			Given(id, new UserCreated(id, "userName", "oldName"));
			When(changeName);
			Then(new NameChanged(id, newName));
		}
	}
}
