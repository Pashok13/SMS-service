﻿using Model.ViewModels.UserViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.ViewModels.GroupViewModels
{
    /// <summary>
    /// ViewModel of ApplicationGroup
    /// </summary>
    public class GroupViewModel
    {
        public int Id { get; set; }
        [StringLength(100)]
        [Display(Name="Name")]
        public string Name { get; set; }
        public int PhoneId { get; set; }
        public List<UserViewModel> ApplicationUsers { get; set; }
    }
}
