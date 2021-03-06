﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using BAL.Managers;
using Model.Interfaces;
using Model.ViewModels.CodeViewModels;
using Moq;
using NUnit.Framework;
using WebApp.Models;
using PageState = Model.ViewModels.CodeViewModels.PageState;

namespace BAL.Tests.ManagersTests
{
	[TestFixture]
	public class CodeManagerTests : TestInitializer
	{
        ICodeManager manager;

        [SetUp]
        protected override void Initialize()
        {
            manager = new CodeManager(mockUnitOfWork.Object, mockMapper.Object);
	        TestContext.WriteLine("Overrided");
        }

		[Test]
        public void Add_EmptyCode_ErrorResult()
        {
            var emptyCode = new CodeViewModel();
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>()))
                .Returns(new List<Code>());
            mockUnitOfWork.Setup(m => m.Save());

            var result = manager.Add(emptyCode);

            TestContext.WriteLine(result.Details);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void Add_ExistingCode_ErrorResult()
        {
            var testingCode = new CodeViewModel() {OperatorCode = "+38066" };   
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>()))
                .Returns(new List<Code> { new Code() });
            mockUnitOfWork.Setup(m => m.Save());

            var result = manager.Add(testingCode);

            TestContext.WriteLine(result.Details);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void Add_DBError_ErrorResult()
        {
            var testingCode = new CodeViewModel { OperatorCode = "+36778"};
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>()))
                .Returns(new List<Code>());
            mockUnitOfWork.Setup(m => m.Save()).Throws(new DataException());

            var result = manager.Add(testingCode);

            TestContext.WriteLine(result.Details);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void Add_CodeObject_SuccessResult()
        {
            var newCode = new CodeViewModel() {OperatorCode = "+38066" };
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>()))
                .Returns(new List<Code>());
            mockUnitOfWork.Setup(m => m.Save());

			var result = manager.Add(newCode);

            TestContext.WriteLine(result.Details);
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void GetById_NonExistingId_null()
        {
            Code nullCode = null;
            mockUnitOfWork.Setup(m => m.Codes.GetById(It.IsAny<int>())).Returns(nullCode);
            mockMapper.Setup(m => m.Map<CodeViewModel>(It.Is<Code>(x => x != null))).Returns(new CodeViewModel());

            var result = manager.GetById(42);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetById_ExistingId_Code()
        {
            var testCode = new Code();
            var testCodeViewModel = new CodeViewModel();
            const int id = 42;
            mockUnitOfWork.Setup(m => m.Codes.GetById(It.Is<int>(x => x == id))).Returns(testCode);
            mockMapper.Setup(m => m.Map<CodeViewModel>(It.Is<Code>(x => x == testCode))).Returns(testCodeViewModel);

            var result = manager.GetById(id);

            Assert.That(testCodeViewModel, Is.EqualTo(result));
        }

        [Test]
        public void Remove_NonExistingId_ErrorResult()
        {
            const int testId = 11;
            Code testCode = null;
            mockUnitOfWork.Setup(m => m.Codes.GetById(It.Is<int>(x => x == testId))).Returns(testCode);
            mockUnitOfWork.Setup(m => m.Save());

            var result = manager.Remove(testId);

            TestContext.WriteLine(result.Details);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void Remove_DBError_ErrorResult()
        {
            const int testId = 11;
            Code testCode = new Code();
            mockUnitOfWork.Setup(m => m.Codes.GetById(It.Is<int>(x => x == testId))).Returns(testCode);
            mockUnitOfWork.Setup(m => m.Save()).Throws(new System.Data.DataException());

            var result = manager.Remove(testId);

            TestContext.WriteLine(result.Details);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void Remove_ExistingId_SuccessResult()
        {
            const int testId = 11;
            Code testCode = new Code();
            mockUnitOfWork.Setup(m => m.Codes.GetById(It.Is<int>(x => x == testId))).Returns(testCode);
            mockUnitOfWork.Setup(m => m.Save());

            var result = manager.Remove(testId);

            TestContext.WriteLine(result.Details);
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void Update_NullOperatorCode_ErrorResult()
        {
            var testCodeViewModel = new CodeViewModel();
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>())).Returns(new List<Code>());
            mockUnitOfWork.Setup(m => m.Save());

            var result = manager.Update(testCodeViewModel);

            TestContext.WriteLine(result.Details);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void Update_EmptyOperatorCode_ErrorResult()
        {
            var testCodeViewModel = new CodeViewModel() {OperatorCode = ""};
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>())).Returns(new List<Code>());
            mockUnitOfWork.Setup(m => m.Save());

            var result = manager.Update(testCodeViewModel);

            TestContext.WriteLine(result.Details);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void Update_ExistingOperatorCode_ErrorResult()
        {
            var testCodeViewModel = new CodeViewModel() { OperatorCode = "+38066" };
            Code testCode = new Code();
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>()))
                .Returns(new List<Code> { testCode });
            mockUnitOfWork.Setup(m => m.Save());

            var result = manager.Update(testCodeViewModel);

            TestContext.WriteLine(result.Details);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void Update_DBError_ErrorResult()
        {
            var testCodeViewModel = new CodeViewModel() { OperatorCode = "+38066" };
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>())).Returns(new List<Code>());
            mockUnitOfWork.Setup(m => m.Save()).Throws(new DataException());

            var result = manager.Update(testCodeViewModel);

            TestContext.WriteLine(result.Details);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void Update_ValidCode_SuccessResult()
        {
            var testCodeViewModel = new CodeViewModel() { OperatorCode = "+38066" };
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>())).Returns(new List<Code>());
            mockUnitOfWork.Setup(m => m.Save());

            var result = manager.Update(testCodeViewModel);

            TestContext.WriteLine(result.Details);
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void GetPage_nullPageState_null()
        {
            PageState testPageState = null;
            var codeList = new List<Code>();
            var codeViewModelList = new List<CodeViewModel>();
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>())).Returns(codeList);
            mockUnitOfWork.Setup(m => m.Operators.GetById(It.Is<int>(x => x == testPageState.OperatorId))).Returns(new Operator { Name = "Zhora"});
            mockMapper.Setup(m => m.Map<IEnumerable<Code>,
                IEnumerable<CodeViewModel>>(It.Is<IEnumerable<Code>>(x => x.Equals(codeList)))).Returns(codeViewModelList);

            var result = manager.GetPage(testPageState);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetPage_IncorrectPageState_Page()
        {
            PageState testPageState = new PageState(){CodesOnPage = 10, LastPage = -6, OperatorId = 1, OperatorName = null, Page = 100};
            var codeList = new List<Code>() {new Code()};
            var codeViewModelList = new List<CodeViewModel>() {new CodeViewModel()};
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>())).Returns(codeList);
            mockUnitOfWork.Setup(m => m.Operators.GetById(It.Is<int>(x => x == testPageState.OperatorId))).Returns(new Operator(){Name = "Zhora"});
            mockMapper.Setup(m => m.Map<IEnumerable<Code>,
                IEnumerable<CodeViewModel>>(It.Is<IEnumerable<Code>>(x => x.Equals(codeList)))).Returns(codeViewModelList);

            var result = manager.GetPage(testPageState);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetPage_CodesOnPageLessThen1_Page()
        {
            PageState testPageState = new PageState(){CodesOnPage = -10, LastPage = -6, OperatorId = 1, OperatorName = null, Page = 100};
            var codeList = new List<Code>() {new Code()};
            var codeViewModelList = new List<CodeViewModel>() {new CodeViewModel()};
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>())).Returns(codeList);
            mockUnitOfWork.Setup(m => m.Operators.GetById(It.Is<int>(x => x == testPageState.OperatorId))).Returns(new Operator(){Name = "Zhora"});
            mockMapper.Setup(m => m.Map<IEnumerable<Code>,
                IEnumerable<CodeViewModel>>(It.Is<IEnumerable<Code>>(x => x.Equals(codeList)))).Returns(codeViewModelList);

            var result = manager.GetPage(testPageState);

            Assert.That(result.PageState.CodesOnPage, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void GetPage_SelectedPageLessThen1_Page()
        {
            PageState testPageState = new PageState() { CodesOnPage = 10, LastPage = 6, OperatorId = 1, OperatorName = null, Page = -10 };
            var codeList = new List<Code>() { new Code() };
            var codeViewModelList = new List<CodeViewModel>() { new CodeViewModel() };
            mockUnitOfWork.Setup(m => m.Codes.Get(It.IsAny<Expression<Func<Code, bool>>>(), It.IsAny<Func<IQueryable<Code>,
                IOrderedQueryable<Code>>>(), It.IsAny<string>())).Returns(codeList);
            mockUnitOfWork.Setup(m => m.Operators.GetById(It.Is<int>(x => x == testPageState.OperatorId))).Returns(new Operator() { Name = "Zhora" });
            mockMapper.Setup(m => m.Map<IEnumerable<Code>,
                IEnumerable<CodeViewModel>>(It.Is<IEnumerable<Code>>(x => x.Equals(codeList)))).Returns(codeViewModelList);

            var result = manager.GetPage(testPageState);

            Assert.That(result.PageState.Page, Is.GreaterThanOrEqualTo(1));
        }

    }
}
