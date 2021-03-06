﻿using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Models;
using System.Linq.Expressions;
using System.Linq;
using AutoMapper;
using Model.ViewModels.ContactViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace BAL.Managers
{
    /// <summary>
    /// Manager for Contacts, include all methods needed to work with Contact storage.
    /// Inherited from BaseManager and have additional methods.
    /// </summary>
    public class ContactManager : BaseManager, IContactManager
    {   

        public ContactManager(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        /// <summary>
        /// Method for getting Contact by id
        /// </summary>
        /// <param name="ContactId">Id of contact</param>
        /// <returns>View model of Contact</returns>
        public ContactViewModel GetContact(int ContactId)
        {
            return mapper.Map<ContactViewModel>(unitOfWork.Contacts.GetById(ContactId));
        }

        /// <summary>
        /// Method for getting all Contacts which belong to specified group on current page
        /// </summary>
        /// <param name="groupId">Id of group</param>
        /// <param name="pageNumber">Number of current page</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns>List with view models of contacts</returns>
        public List<ContactViewModel> GetContact(int groupId, int pageNumber, int pageSize)
        {
                var contacts = unitOfWork.Contacts.GetContactsByPageNumber(pageNumber, pageSize, filter: item => item.ApplicationGroupId == groupId);
                foreach (var contact in contacts)
                {
                    contact.Phone = unitOfWork.Phones.GetById(contact.PhoneId);
                }
                return mapper.Map<IEnumerable<Contact>, List<ContactViewModel>>(contacts);
        }

        /// <summary>
        /// Method for getting all Contacts which belong to specified group on current page with search value
        /// </summary>
        /// <param name="groupId">Id of group</param>
        /// <param name="pageNumber">Number of current page</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="searchValue">Search value</param>
        /// <returns>List with view models of contacts</returns>
        public List<ContactViewModel> GetContactBySearchValue(int groupId, int pageNumber, int pageSize,
            string searchValue)
        {
                var contacts = unitOfWork.Contacts.GetAll();
                foreach (var contact in contacts)
                {
                    contact.Phone = unitOfWork.Phones.GetById(contact.PhoneId);
                }
                contacts = contacts.Where(item => item.ApplicationGroupId == groupId &&
                        (item.Phone.PhoneNumber == searchValue ||
                        item.Name == searchValue || item.Surname == searchValue ||
                        item.Name + " " + item.Surname == searchValue || item.KeyWords.Contains(searchValue)))
                        .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                return mapper.Map<IEnumerable<Contact>, List<ContactViewModel>>(contacts);
        }

        /// <summary>
        /// Method for getting count of contacts that belong to group
        /// </summary>
        /// <param name="groupId">Id of group</param>
        /// <returns></returns>
        public int GetContactCount(int groupId)
        {
                List<Contact> contacts = unitOfWork.Contacts.Get(
                    filter: item => item.ApplicationGroupId == groupId).ToList();
                return contacts.Count;
        }

        /// <summary>
        /// Method for getting count of contacts that belong to group with search value
        /// </summary>
        /// <param name="groupId">Id of group</param>
        /// <param name="searchValue">Search value</param>
        /// <returns></returns>
        public int GetContactCountBySearchValue(int groupId, string searchValue)
        {
                List<Contact> contacts = unitOfWork.Contacts.Get(
                        filter: item => item.ApplicationGroupId == groupId).ToList();
                foreach (var contact in contacts)
                {
                    contact.Phone = unitOfWork.Phones.GetById(contact.PhoneId);
                }
                contacts = contacts.Where(item => item.Phone.PhoneNumber == searchValue ||
                        item.Name == searchValue || item.Surname == searchValue ||
                        item.Name + " " + item.Surname == searchValue || item.KeyWords.Contains(searchValue)).ToList();

                return contacts.Count;
        }

        /// <summary>
        /// Method for inserting new company to db
        /// </summary>
        /// <param name="contactModel">View model of contact</param>
        /// <param name="groupId">Id of Group wich create this contact</param>
        /// <returns></returns>
        public bool CreateContact(ContactViewModel contactModel, int groupId)
        {
                Contact newContact = mapper.Map<Contact>(contactModel);
                newContact.ApplicationGroupId = groupId;
                List<Phone> phone = unitOfWork.Phones.Get
                    (filter: item => item.PhoneNumber == contactModel.PhonePhoneNumber).ToList();
                if (phone.Count == 0)
                {
                    Phone newPhone = new Phone();
                    newPhone.PhoneNumber = contactModel.PhonePhoneNumber;
                    unitOfWork.Phones.Insert(newPhone);
                    unitOfWork.Save();
                    newContact.Phone = newPhone;
                }
                else
                {
                    List<Contact> contact = unitOfWork.Contacts.Get(filter: item => item.PhoneId == phone[0].Id && item.ApplicationGroupId == groupId).ToList();
                    if (contact.Count != 0)
                    {
                        return false;
                    }
                    newContact.Phone = phone[0];
                }
                unitOfWork.Contacts.Insert(newContact);
                unitOfWork.Save();
                return true;
        }

        /// <summary>
        /// Delete Contact by Id
        /// </summary>
        /// <param name="id">Id of contact</param>
        public void DeleteContact(int id)
        {
                Contact contact = unitOfWork.Contacts.GetById(id);
                unitOfWork.Contacts.Delete(contact);
                unitOfWork.Save();
        }

        /// <summary>
        /// Update Company in db
        /// </summary>
        /// <param name="contactModel">View model of contact</param>
        /// <param name="groupId">Id of Group wich update this contact</param>
        /// <returns></returns>
        public bool UpdateContact(ContactViewModel contactModel, int groupId)
        {
                Contact contact = mapper.Map<Contact>(contactModel);
                contact.ApplicationGroupId = groupId;
                List<Phone> phone = unitOfWork.Phones.Get(filter: item => item.PhoneNumber == contactModel.PhonePhoneNumber).ToList();
                if (phone.Count == 0)
                {
                    Phone newPhone = new Phone();
                    newPhone.PhoneNumber = contactModel.PhonePhoneNumber;
                    unitOfWork.Phones.Insert(newPhone);
                    unitOfWork.Save();
                    contact.Phone = newPhone;
                }
                else
                {
                    contact.Phone = phone[0];
                }
                unitOfWork.Contacts.SetStateModified(contact);
                unitOfWork.Contacts.Update(contact);
                unitOfWork.Save();
                return true;
        }

        

        public bool TranslateToContacts(string contacts, string userId)
        {
           var user = unitOfWork.ApplicationUsers.Get(x => x.Id == userId).FirstOrDefault();
            if (user == null)
                return false;            

                var splitedContactsR = string.IsNullOrEmpty(contacts) ? null : contacts.Split("\r\n").ToList();
            var result = new List<Contact>();
            foreach (var item in splitedContactsR.Skip(1))
            {
                var tempList = string.IsNullOrEmpty(item) ? null : item.Split(',').ToList();
                if (tempList!= null)
                {
                    
                        var temp = new Contact();
                        temp.ApplicationGroupId = user.ApplicationGroupId;
                        var tempPhone = unitOfWork.Phones.Get(x => x.PhoneNumber == tempList.ElementAt(0)).FirstOrDefault();

                        if (tempPhone == null)
                        {
                            temp.Phone = new Phone() {PhoneNumber = tempList.ElementAt(0)};
                            temp.BirthDate = DateTime.ParseExact((tempList.ElementAt(1)), "yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture);
                            temp.Gender = Convert.ToByte(tempList.ElementAt(2));
                            unitOfWork.Contacts.Insert(temp);

                        }
                        else
                         {
                            temp.PhoneId = tempPhone.Id;
                            temp.Gender = temp.Gender;
                            temp.BirthDate = temp.BirthDate;
                            unitOfWork.Contacts.Update(temp);
                        }
                }
            }
            unitOfWork.Save();
            return true;
        }

       
    }
}
