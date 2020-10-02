using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predict;
using Predict.Controllers;

namespace Predict.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void ContactUs()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = controller.ContactUs();

            // Assert
            Assert.IsNotNull(result);
        }

    }
}
