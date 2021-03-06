﻿using Model.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmail(EmailDTO emailDTO);
        void SendEmails(IEnumerable<EmailDTO> emails);
    }
}
