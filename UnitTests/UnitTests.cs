using System;
//using System.Moles;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Food;
using UIAutoTest;

namespace UnitTests
{
    [TestClass]
    public class UnitTestsUI
    {
        private static Application _application;
        private static Window _mainWindow;

        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            //App app = new App();
            
            //app.PrevWindow
            _application = _application ?? UI.Run(() => new App { MainWindow = new Autorization() });

            _mainWindow = _mainWindow ?? _application.Get(x => x.MainWindow);
        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            _application.Invoke(x => x.Shutdown());
        }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        [TestMethod]
        public void TestMethod1()
        {
        }
    }

    [TestClass]
    public class UnitTestsServer
    {
        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            // _application = _application ?? UI.Run(() => new App { MainWindow = new MainWindow() });

            //_mainWindow = _mainWindow ?? _application.Get(x => x.MainWindow);
        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            //_application.Invoke(x => x.Shutdown());
        }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        [TestMethod]
        public void TestMethod1()
        {
        }
    }

    [TestClass]
    public class UnitTestsLogic
    {
        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            // _application = _application ?? UI.Run(() => new App { MainWindow = new MainWindow() });

            //_mainWindow = _mainWindow ?? _application.Get(x => x.MainWindow);
        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            //_application.Invoke(x => x.Shutdown());
        }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
