﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using BAL.Managers;
using BAL.Notifications.Infrastructure;
using BAL.Tests;
using Model.Interfaces;
using Model.ViewModels.CompanyViewModels;
using Model.ViewModels.GroupViewModels;
using Moq;
using NUnit.Framework;
using WebApp.Models;

namespace BAL.Tests.ManagersTests
{
    [TestFixture]
    public  class CompanyManagerTests : TestInitializer
    {
   
        private static Mock<INotificationsGenerator<Company>> mockNotification = new Mock<INotificationsGenerator<Company>>();
        private CompanyManager manager = new CompanyManager(mockUnitOfWork.Object, mockMapper.Object, mockNotification.Object );


        [SetUp]
        public void SetUp()
        {
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockMapper = new Mock<IMapper>();
            mockNotification= new Mock<INotificationsGenerator<Company>>();
            manager = new CompanyManager(mockUnitOfWork.Object, mockMapper.Object, mockNotification.Object);
        
        }

        [Test]
        public void Insert_EmptyObject_ReturnsFalse()
        {
            CompanyViewModel item = new CompanyViewModel();
            mockMapper.Setup(m => m.Map<CompanyViewModel, Company>(item))
                .Returns(new Company());
            var result = manager.Insert(item);
            Assert.IsFalse(result);
        }

        [Test]
        public void Insert_NewObject_ReturnsTrue()
        {
            CompanyViewModel item = new CompanyViewModel() { Name = "Test", Description = "Test", PhoneId = 2, TariffId = 1, Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false};
            mockMapper.Setup(m => m.Map<CompanyViewModel, Company>(item))
                .Returns(new Company() { Name = "Test", Description = "Test", PhoneId = 2, TariffId = 1, Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false });
            mockUnitOfWork.Setup(u => u.Companies.Insert(new Company() { Name = "Test", Description = "Test", PhoneId = 2, TariffId = 1, Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false }));
            var result = manager.Insert(item);
            Assert.IsTrue(result);
        }

        [Test]
        public void Insert_ExistingObject_ReturnsTrue()
        {
            CompanyViewModel item = new CompanyViewModel() { Id = 1, Name = "Test", Description = "Test", PhoneId = 2, TariffId = 1, Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false };
            mockMapper.Setup(m => m.Map<CompanyViewModel, Company>(item))
                .Returns(new Company() { Id = 1, Name = "Test", Description = "Test", PhoneId = 2, TariffId = 1, Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false });
            var result = manager.Insert(item);
            Assert.IsFalse(result);
        }

        [Test]
        public void Update_EmptyCompany_ThrowsException()
        {
            CompanyViewModel item = new CompanyViewModel();
            mockMapper.Setup(m => m.Map<CompanyViewModel, Company>(item))
                .Returns(new Company());
            Assert.Throws<NullReferenceException>(() => manager.Update(item));
        }

       

         [Test]
        public void Update_CompanyWithoutPhone_ReturnFalse()
        {
            CompanyViewModel item = new CompanyViewModel() { Id=1, Name = "Test", Description = "Test", TariffId = 1, Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false };
            mockMapper.Setup(m => m.Map<CompanyViewModel, Company>(item))
                .Returns(new Company(){ Id =1, Name = "Test", Description = "Test", TariffId = 1, Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false });
            mockUnitOfWork.Setup(u => u.Companies.GetById(1))
                .Returns(new Company(){Id=1, Name = "Test", Description = "Test", TariffId = 1, Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false });
            mockUnitOfWork.Setup(u => u.Save()).Throws(new Exception());
            var result = manager.Update(item);
            Assert.IsFalse(result);
        }

        [Test]
        public void Update_CompanyWithoutTariffId_ReturnFalse()
        {
            CompanyViewModel item = new CompanyViewModel() { Id = 1, Name = "Test", Description = "Test", Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false };
            mockMapper.Setup(m => m.Map<CompanyViewModel, Company>(item))
                .Returns(new Company() { Id = 1, Name = "Test", Description = "Test",  Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false });
            mockUnitOfWork.Setup(u => u.Companies.GetById(1))
                .Returns(new Company() { Id = 1, Name = "Test", Description = "Test", Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false });
            mockUnitOfWork.Setup(u => u.Save()).Throws(new Exception());
            var result = manager.Update(item);
            Assert.IsFalse(result);
        }

        [Test]
        public void Delete_NotExistId_ReturnFalse()
        {
            mockUnitOfWork.Setup(u => u.Companies.GetById(0));
            var result = manager.Delete(0);
            Assert.IsFalse(result);
        }

        [Test]
        public void Delete_ExistingObject_ReturnTrue()
        {
            mockUnitOfWork.Setup(u => u.Companies.GetById(1))
                .Returns(new Company() { Id = 1, Name = "Test", Description = "Test", Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false });
            var result = manager.Delete(1);
            Assert.IsTrue(result);
        }

        [Test]
        public void Get_NotExistingId_ReturnNull()
        {
            mockUnitOfWork.Setup(u => u.Companies.GetById(0));
            var result = manager.Get(0);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Get_ExistingId_ReturnCompanyViewModel()
        {
            CompanyViewModel item = new CompanyViewModel() { Id = 1, Name = "Test", Description = "Test", Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false };
            mockUnitOfWork.Setup(u => u.Companies.GetById(1))
                .Returns(new Company() { Id = 1, Name = "Test", Description = "Test", Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false });
            mockMapper.Setup(m => m.Map<CompanyViewModel, Company>(item))
                .Returns(new Company() { Id = 1, Name = "Test", Description = "Test", Type = CompanyType.Send, ApplicationGroupId = 2, IsPaused = false });
            var result = manager.Get(1);
            Assert.That(result, Is.EqualTo(result));
        }
    }
}