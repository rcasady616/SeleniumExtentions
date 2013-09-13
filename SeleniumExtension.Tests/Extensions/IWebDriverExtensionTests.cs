﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TestWebPages.UIFramework.Pages;

namespace SeleniumExtension.Tests.Extensions
{
    [TestFixture]
    public class IWebDriverExtensionTests
    {
        private IWebDriver _driver;
        private AjaxyControlPage page;

        [SetUp]
        public void SetupTest()
        {
            string url = string.Format(@"file:///{0}../../../..{1}", Directory.GetCurrentDirectory(), AjaxyControlPage.Url);
            _driver = IWebDriverFactory.GetBrowser(url);
            page = new AjaxyControlPage(_driver);
            Assert.AreEqual(true, page.IsPageLoaded());
        }

        [TearDown]
        public void TearDown()
        {
            if (_driver != null)
            {
                _driver.Close();
                _driver.Quit();
            }
        }

        [TestCase(true, "red")]
        [TestCase(false, "NeverGonnaGetNeverGonnaGet")]
        public void TestWaitUntilExists(bool expected, string id)
        {
            Assert.AreEqual(expected, _driver.WaitUntilExists(By.Id(id)));
        }

        [TestCase]
        public void TestWaitUntilExists()
        {
            page.GreenRadio.Click();
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            page.SubmitButton.Click();
            Assert.AreEqual(true, _driver.WaitUntilExists(AjaxyControlPage.ByLabelsDiv));
        }

        [TestCase(false, "red")]
        [TestCase(true, "NeverGonnaGetNeverGonnaGet")]
        public void TestWaitUntilNotExists(bool expected, string id)
        {
            Assert.AreEqual(expected, _driver.WaitUntilNotExists(By.Id(id)));
        }

        [TestCase(true, "red")]
        [TestCase(false, "NeverGonnaGetNeverGonnaGet")]
        public void TestWaitUntilVisible(bool expected, string id)
        {
            Assert.AreEqual(expected, _driver.WaitUntilVisible(By.Id(id)));
        }

        [TestCase]
        public void TestWaitUntilVisible()
        {
            page.GreenRadio.Click();
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            page.SubmitButton.Click();
            Assert.AreEqual(true, _driver.WaitUntilVisible(AjaxyControlPage.ByLabelsDiv));
        }

        [Test, TestCaseSource("GetAjaxyControlPageLocators")]
        public void TestWaitUntilVisiblesTrue(List<By> locators)
        {
            page.GreenRadio.Click();
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            page.SubmitButton.Click();
            Assert.AreEqual(true, _driver.WaitUntilVisible(locators));
        }

        [Test, TestCaseSource("GetAjaxyControlPageLocators")]
        public void TestWaitUntilVisiblesFalse(List<By> locators)
        {
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            Assert.AreEqual(false, _driver.WaitUntilVisible(locators));
        }

        [Test, TestCaseSource("GetAjaxyControlPageLocators")]
        public void TestWaitUntilNotVisiblesFalse(List<By> locators)
        {
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            Assert.AreEqual(false, _driver.WaitUntilNotVisible(locators));
        }

        [Test, TestCaseSource("GetAjaxyControlPageLocators")]
        public void TestWaitUntilNotVisiblesTrue(List<By> locators)
        {
            _driver.Navigate().GoToUrl("http://rickcasady.blogspot.com/2013/08/automate-like-superhero.html");
            Assert.AreEqual(true, _driver.WaitUntilNotVisible(locators));
        }

        [TestCase(true, "AjaxyControl")]
        [TestCase(false, "NeverGonnaGetNeverGonnaGet")]
        public void TestWaitUntilTitleIs(bool expected, string title)
        {
            Assert.AreEqual(expected, _driver.WaitUntilTitleIs(title));
        }

        [TestCase(true, "TestIsPageLoaded")]
        [TestCase(false, "NeverGonnaGetNeverGonnaGet")]
        public void TestWaitUntilTextEquals(bool expected, string text)
        {
            page.GreenRadio.Click();
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            page.SubmitButton.Click();
            Assert.AreEqual(expected, _driver.WaitUntilTextEquals(AjaxyControlPage.ByLabelsDiv, text));
        }

        [TestCase(false, "TestIsPageLoaded")]
        [TestCase(true, "NeverGonnaGetNeverGonnaGet")]
        public void TestWaitUntilTextNotEquals(bool expected, string text)
        {
            page.GreenRadio.Click();
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            page.SubmitButton.Click();
            Assert.AreEqual(expected, _driver.WaitUntilTextNotEquals(AjaxyControlPage.ByLabelsDiv, text));
        }

        [TestCase(true, "TestIsPageLoaded")]
        [TestCase(true, "Load")]
        [TestCase(false, "NeverGonnaGetNeverGonnaGet")]
        public void TestWaitUntilTextContains(bool expected, string text)
        {
            page.GreenRadio.Click();
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            page.SubmitButton.Click();
            Assert.AreEqual(expected, _driver.WaitUntilTextContains(AjaxyControlPage.ByLabelsDiv, text));
        }

        [TestCase(false, "TestIsPageLoaded")]
        [TestCase(false, "Load")]
        [TestCase(true, "NeverGonnaGetNeverGonnaGet")]
        public void TestWaitUntilTextNotContains(bool expected, string text)
        {
            page.GreenRadio.Click();
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            page.SubmitButton.Click();
            Assert.AreEqual(expected, _driver.WaitUntilTextNotContains(AjaxyControlPage.ByLabelsDiv, text));
        }

        [Test, TestCaseSource("GetLocators")]
        public void TestClickWaitForCondition(By locator)
        {
            page.GreenRadio.Click();
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            page.SubmitButton.ClickWaitForCondition(_driver, ExpectedConditions.ElementExists(locator));
            Assert.AreEqual(true, _driver.ElementExists(locator));
        }

        [Test, TestCaseSource("GetLocators")]
        public void TestClickWaitUnilVisable(By locator)
        {
            page.GreenRadio.Click();
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            page.SubmitButton.ClickWaitUnilVisable(_driver, locator);
            Assert.AreEqual(true, _driver.ElementExists(locator));
        }

        [Test, TestCaseSource("GetAjaxyControlPageLocators")]
        public void TestClickWaitForConditions(List<By> locators)
        {
            var ex = locators.Select(ExpectedConditions.ElementExists).ToList();

            page.GreenRadio.Click();
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            page.SubmitButton.ClickWaitForConditions(_driver, ex);

            foreach (var locator in locators)
            {
                Assert.AreEqual(true, _driver.ElementExists(locator));
            }
        }

        [Test, TestCaseSource("GetAjaxyControlPageLocators")]
        public void TestClickWaitUnilVisables(List<By> locators)
        {
            page.GreenRadio.Click();
            page.NewLabelText.SendKeys("TestIsPageLoaded");
            page.SubmitButton.ClickWaitUnilVisables(_driver, locators);

            foreach (var locator in locators)
            {
                Assert.AreEqual(true, _driver.ElementExists(locator));
            }
        }

        #region testdata

        public static List<List<By>> GetAjaxyControlPageLocators()
        {
            var ret = new List<List<By>>();
            ret.Add(new List<By>() { AjaxyControlPage.ByLabelsDiv });
            ret.Add(new List<By>() { AjaxyControlPage.ByLabelsDiv, AjaxyControlPage.ByGreenRadio });
            ret.Add(new List<By>() { AjaxyControlPage.ByLabelsDiv, AjaxyControlPage.ByGreenRadio, AjaxyControlPage.ByRedRadio });
            ret.Add(new List<By>() { AjaxyControlPage.ByLabelsDiv, AjaxyControlPage.ByGreenRadio, AjaxyControlPage.ByRedRadio });
            ret.Add(new List<By>() { AjaxyControlPage.ByLabelsDiv, AjaxyControlPage.ByGreenRadio, AjaxyControlPage.ByRedRadio, AjaxyControlPage.BySubmitButton });
            ret.Add(new List<By>() { AjaxyControlPage.ByLabelsDiv, AjaxyControlPage.ByGreenRadio, AjaxyControlPage.ByRedRadio, AjaxyControlPage.BySubmitButton, AjaxyControlPage.ByNewLableText });
            return ret;
        }

        public static List<By> GetLocators()
        {
            return new List<By>
                {
                    AjaxyControlPage.ByLabelsDiv,
                    AjaxyControlPage.ByGreenRadio,
                    AjaxyControlPage.ByRedRadio,
                    AjaxyControlPage.BySubmitButton,
                    AjaxyControlPage.ByNewLableText
                };
        }

        #endregion
    }
}
