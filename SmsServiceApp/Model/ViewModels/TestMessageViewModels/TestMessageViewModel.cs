﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.ViewModels.TestMessageViewModels
{
    public class TestMessageViewModel
    {
        [Required(ErrorMessage = "The Sender phone field is required.")]
        [Display(Name = "Sender phone")]
        [RegularExpression(@"^\+[0-9]{10,12}$", ErrorMessage = "Not a valid phone number")]
        public string Sender { get; set; }
        [Required(ErrorMessage = "The Reсipient phone field is required.")]
        [Display(Name = "Recipient phone")]
        [RegularExpression(@"^\+[0-9]{10,12}$", ErrorMessage = "Not a valid phone number")]
        public string Recipient { get; set; }
        [Required(ErrorMessage = "The Message field is required.")]
        [StringLength(500)]
        [Display(Name = "Message")]
        public string Message { get; set; }
    }
}
