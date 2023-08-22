using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Tests.UnitTests
{
    [TestClass]
    public class FirstCapitalLetterAttributeTest
    {
        /// <summary>
        /// Test to verify if the method returns error if the string has first lower letter
        /// </summary>
        [TestMethod]
        public void FirstLowerLetter_ReturnsError()
        {
            // Preparation
            var firstCapitalLetter = new FirstCapitalLetterAttribute();
            string value = "andres";
            var valContext = new ValidationContext(new { Name = value });

            // Execution
            var result = firstCapitalLetter.GetValidationResult(value, valContext);

            // Verification
            Assert.AreEqual("La primera letra debe ser mayúscula", result.ErrorMessage);
        }

        /// <summary>
        /// Test to verify if the method doesn't return error if the string is null
        /// </summary>
        [TestMethod]
        public void NullValue_DoesntReturnError()
        {
            // Preparation
            var firstCapitalLetter = new FirstCapitalLetterAttribute();
            string value = null;
            var valContext = new ValidationContext(new { Name = value });

            // Execution
            var result = firstCapitalLetter.GetValidationResult(value, valContext);

            // Verification
            Assert.IsNull(result);
        }

        /// <summary>
        /// Test to verify if the method doesn't return error if the string has first capital letter
        /// </summary>
        [TestMethod]
        public void FirstCapitalLetter_DoesntReturnError()
        {
            // Preparation
            var firstCapitalLetter = new FirstCapitalLetterAttribute();
            string value = "Andres";
            var valContext = new ValidationContext(new { Name = value });

            // Execution
            var result = firstCapitalLetter.GetValidationResult(value, valContext);

            // Verification
            Assert.IsNull(result);
        }
    }
}