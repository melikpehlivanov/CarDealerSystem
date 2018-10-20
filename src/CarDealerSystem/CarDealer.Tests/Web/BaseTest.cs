namespace CarDealer.Tests.Web
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;

    public abstract class BaseTest
    {
        protected BaseTest()
        {
            TestSetup.InitializeMapper();
        }

        protected void AddClaimsPrincipal(Controller controller, string username)
        {
            controller.ControllerContext = this.SetControllerContext(username);
        }

        protected void InitializeTempData(Controller controller)
        {
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), new Mock<ITempDataProvider>().Object);
        }

        private ControllerContext SetControllerContext(string username)
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, username)
                        }, "someAuthTypeName"))
                }
            };
        }
    }
}
