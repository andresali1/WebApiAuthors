using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiAuthors.Controllers.V1;
using WebApiAuthors.Tests.Mocks;

namespace WebApiAuthors.Tests.UnitTests
{
    [TestClass]
    public class RootControllerTest
    {
        /// <summary>
        /// Test to verify if the links returned are equal to 4, when user is an admin
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task IfUserIsAdmin_WeGet4Links()
        {
            // Preparation
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Result = AuthorizationResult.Success();

            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            // Execution
            var result = await rootController.Get();

            // Verification
            Assert.AreEqual(4, result.Value.Count());
        }

        /// <summary>
        /// Test to verify if the links returned are equal to 2, when user is not an admin
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task IfUserIsNotAdmin_WeGet2Links()
        {
            // Preparation
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Result = AuthorizationResult.Failed();

            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            // Execution
            var result = await rootController.Get();

            // Verification
            Assert.AreEqual(2, result.Value.Count());
        }

        /// <summary>
        /// Test to verify if the links returned are equal to 2, when user is not an admin but using mocks
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task IfUserIsNotAdmin_WeGet2Links_UsingMoq()
        {
            // Preparation
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
            )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()
            )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.Link(
                It.IsAny<string>(),
                It.IsAny<object>()
            )).Returns(string.Empty);

            var rootController = new RootController(mockAuthorizationService.Object);
            rootController.Url = mockUrlHelper.Object;

            // Execution  
            var result = await rootController.Get();

            // Verification
            Assert.AreEqual(2, result.Value.Count());
        }
    }
}
