namespace CarDealer.Tests.Web
{
    using Microsoft.AspNetCore.Identity;
    using Models;
    using Moq;

    public class MockGenerator
    {
        public static Mock<UserManager<User>> UserManagerMock
            => new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        public static Mock<RoleManager<IdentityRole>> RoleManagerMock
            => new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
    }
}
