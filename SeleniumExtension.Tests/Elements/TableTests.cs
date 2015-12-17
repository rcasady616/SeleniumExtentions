﻿using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumExtension.Elements;
using TestWebPages.UIFramework.Pages;

namespace SeleniumExtension.Tests.Elements
{

    //[TestFixture]//(typeof(int), typeof(int), 1, 1)]
    //[NUnit.Framework.Category("Elements")]
    //public class TableTests2<T1, T2> : BaseTest
    //{
    //    public TablesPage Page;

    //    [SetUp]
    //    public void SetupTest()
    //    {
    //        Driver.Navigate().GoToUrl(TablesPage.Url);
    //        Page = new TablesPage(Driver);
    //        Assert.AreEqual(true, Page.IsPageLoaded());
    //    }

    //    //[TestCase(typeof (int), typeof (int), 1, 1, "ID")] //, Category = "Unit", Description = "Index")]
    //    [TestCase(1, 1, ExpectedResult = "ID")]
    //    public void GetCellBy(object column, object row, string cell)
    //    {
    //        //var col = (T1)(object)column.ToString();
    //        //var ro = (T2)(object)row.ToString();
    //        //var col = (T1)column>column;
    //        // var ro = (T2)(object)row.ToString();
    //        Assume.That(Driver.ElementExists(TablesPage.ByTableOne));
    //        var table = new TableElement(Driver.FindElement(TablesPage.ByTableOne));
    //        // Assert.AreEqual(cell, table.GetCell(column as typeof(T1) , <typeof(T2)>row).Text);
    //        // return table.GetCell(col , ro).Text;
    //        var col = Convert<T1>(column);
    //        var ro = Convert<T2>(row);
    //        return table.GetCell(column, row).Text;
    //    }

    //    //public T Convert<T>(object v)
    //    //{
    //    //    return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(v);
    //    //}
    //    public T Convert<T>(object v)
    //    {
    //        if (typeof(T) == typeof(string))
    //        {
    //            return (T)(object)v;
    //        }
    //        else
    //        {
    //            return (T)v;
    //        }

    //    }

    //    //public T GetType<T>(object v)
    //    //{
    //    //    return v.GetType();

    //    //}
    //}

    [TestFixture]
    [Category("Elements")]
    public class TableTests : BaseTest
    {
        public TablesPage Page;

        [SetUp]
        public void SetupTableTests()
        {
            Driver.Navigate().GoToUrl(TablesPage.Url);
            Page = new TablesPage(Driver);
            Assert.AreEqual(true, Page.IsPageLoaded());
        }

        //[TestCase(typeof(int), typeof(int), 1, 1, "ID")]//, Category = "Unit", Description = "Index")]
        //public void GetCellBy<T1, T2>(T1 column, T2 row, string cell)
        //{

        //    Assume.That(Driver.ElementExists(TablesPage.ByTableOne));
        //    var table = new TableElement(Driver.FindElement(TablesPage.ByTableOne));
        //    Assert.AreEqual(cell, table.GetCell(column, row).Text);
        //}


        #region table with header

        [TestCase(1, 1, "ID")]
        [TestCase(1, 6, "5")]
        [TestCase(2, 1, "Subject ID")]
        [TestCase(2, 6, "1007")]
        public void GetCellByIndexWithHeaderRow(int column, int row, string cell)
        {
            Assume.That(Driver.ElementExists(TablesPage.ByTableOne));
            var table = new Table(Driver.FindElement(TablesPage.ByTableOne));
            Assert.AreEqual(cell, table.GetCell(column, row).Text);
        }

        [TestCase(1, "ID", "ID", 1)]
        [TestCase(1, "5", "5", 1)]
        [TestCase(2, "ID", "Subject ID", 2)]
        [TestCase(2, "5", "1007", 2)]
        public void GetCellColumnIndexRowlabelWithHeaderRow(int column, string row, string cell, int rowIndex)
        {
            Assume.That(Driver.ElementExists(TablesPage.ByTableOne));
            var table = new Table(Driver.FindElement(TablesPage.ByTableOne), 1, rowIndex);
            Assert.AreEqual(cell, table.GetCell(column, row).Text);
        }

        [TestCase("ID", 1, "ID")]
        [TestCase("ID", 6, "5")]
        [TestCase("Subject ID", 1, "Subject ID")]
        [TestCase("Subject ID", 6, "1007")]
        public void GetCellColumnLabelRowIndexWithheaderRow(string column, int row, string cell)
        {
            Assume.That(Driver.ElementExists(TablesPage.ByTableOne));
            var table = new Table(Driver.FindElement(TablesPage.ByTableOne), 2, 1);
            Assert.AreEqual(cell, table.GetCell(column, row).Text);
        }

        [TestCase("ID", "ID", "ID")]
        [TestCase("ID", "5", "5")]
        [TestCase("Subject ID", "Subject ID", "Subject ID")]
        [TestCase("Subject ID", "1007", "1007")]
        public void GetCellColumnLabelRowLabelWithHeaderRow(string column, string row, string cell)
        {
            Assume.That(Driver.ElementExists(TablesPage.ByTableOne));
            var table = new Table(Driver.FindElement(TablesPage.ByTableOne));
            Assert.AreEqual(cell, table.GetCell(column, row).Text);
        }

        #endregion

        #region table without header row

        [TestCase(1, 1, "ID")]
        [TestCase(1, 6, "5")]
        [TestCase(2, 1, "Subject ID")]
        [TestCase(2, 6, "1007")]
        public void GetCellByIndexWithOutHeaderRow(int column, int row, string cell)
        {
            Assume.That(Driver.ElementExists(TablesPage.ByTableTwo));
            var table = new Table(Driver.FindElement(TablesPage.ByTableTwo));
            Assert.AreEqual(cell, table.GetCell(column, row).Text);
        }

        [TestCase(1, "ID", "ID")]
        [TestCase(1, "5", "5")]
        [TestCase(2, "ID", "Subject ID")]
        [TestCase(2, "5", "1007")]
        public void GetCellColumnIndexRowlabelWithOutheaderRow(int column, string row, string cell)
        {
            Assume.That(Driver.ElementExists(TablesPage.ByTableTwo));
            var table = new Table(Driver.FindElement(TablesPage.ByTableTwo));
            Assert.AreEqual(cell, table.GetCell(column, row).Text);
        }

        [TestCase("ID", 1, "ID")]
        [TestCase("ID", 6, "5")]
        [TestCase("Subject ID", 1, "Subject ID")]
        [TestCase("Subject ID", 6, "1007")]
        public void GetCellColumnLabelRowIndexWithOutheaderRow(string column, int row, string cell)
        {
            Assume.That(Driver.ElementExists(TablesPage.ByTableTwo));
            var table = new Table(Driver.FindElement(TablesPage.ByTableTwo));
            Assert.AreEqual(cell, table.GetCell(column, row).Text);
        }

        [TestCase("ID", "ID", "ID")]
        [TestCase("ID", "5", "5")]
        [TestCase("Subject ID", "Subject ID", "Subject ID")]
        [TestCase("Subject ID", "1007", "1007")]
        public void GetCellColumnLabelRowLabelWithOutHeaderRow(string column, string row, string cell)
        {
            Assume.That(Driver.ElementExists(TablesPage.ByTableTwo));
            var table = new Table(Driver.FindElement(TablesPage.ByTableTwo));
            Assert.AreEqual(cell, table.GetCell(column, row).Text);
        }
        #endregion
    }
}
