﻿using Model.ViewModels.RecipientViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.ViewModels.CompanyViewModels
{
    /// <summary>
    /// View model for Send info to company
    /// </summary>
    public class SendViewModel
    {
        public int Id { get; set; }
        public int TariffId { get; set; }
        [Required(ErrorMessage = "The Tariff field is required.")]
        [Display(Name = "Tariff")]
        public string Tariff { get; set; }
        [Required(ErrorMessage = "The Message field is required.")]
        [StringLength(500)]
        [Display(Name ="Message")]
        public string Message { get; set; }
        [Display(Name = "Time for send")]
        [DataType(DataType.DateTime)]
        public DateTime SendingTime { get; set; }
        [Display(Name = "Recipients")]
        public int RecipientsCount { get; set; }
    }
}
