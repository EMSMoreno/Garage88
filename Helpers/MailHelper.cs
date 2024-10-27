using Garage88.Data.Repositories;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using System.Net.Mail;
using System.Net;

namespace Garage88.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IClientRepository _clientRepository;
        private readonly IMechanicRepository _mechanicRepository;
        private readonly ILogger<MailHelper> _logger;

        public MailHelper(IConfiguration configuration, IClientRepository clientRepository, IMechanicRepository mechanicRepository, ILogger<MailHelper> logger)
        {
            _configuration = configuration;
            _clientRepository = clientRepository;
            _mechanicRepository = mechanicRepository;
            _logger = logger;
        }

        public List<SelectListItem> Destinations()
        {
            var options = new List<SelectListItem>
            {
                new SelectListItem { Text = "[Insert To Destination]", Value = "0" },
                new SelectListItem { Text = "Clients", Value = "1" },
                new SelectListItem { Text = "Mechanics", Value = "2" }
            };

            return options;
        }

        public async Task<Response> SendAnnouncementAsync(int to, string subject, string body, string path)
        {
            if (to > 0 && to < 3)
            {
                if (to == 1) // Clients
                {
                    var clients = _clientRepository.GetAll().ToList(); // Ensure you materialize the query

                    if (clients != null && clients.Any())
                    {
                        foreach (var client in clients)
                        {
                            if (!string.IsNullOrEmpty(client.Email)) // Ensure email is valid
                            {
                                var response = await SendEmail(client.Email, subject, body, string.IsNullOrEmpty(path) ? null : path);

                                if (!response.IsSuccess)
                                {
                                    return response; // Return the error response
                                }
                            }
                        }

                        return new Response { IsSuccess = true };
                    }

                    return new Response { IsSuccess = false, Message = "No clients found." };
                }

                if (to == 2) // Mechanics
                {
                    var mechanics = _mechanicRepository.GetAll().ToList(); // Ensure you materialize the query

                    if (mechanics != null && mechanics.Any())
                    {
                        foreach (var mechanic in mechanics)
                        {
                            if (!string.IsNullOrEmpty(mechanic.Email)) // Ensure email is valid
                            {
                                var response = await SendEmail(mechanic.Email, subject, body, string.IsNullOrEmpty(path) ? null : path);

                                if (!response.IsSuccess)
                                {
                                    return response; // Return the error response
                                }
                            }
                        }

                        return new Response { IsSuccess = true };
                    }

                    return new Response { IsSuccess = false, Message = "No mechanics found." };
                }

                return new Response { IsSuccess = false, Message = "Invalid destination specified." };
            }

            return new Response { IsSuccess = false, Message = "Invalid destination specified." };
        }

        public async Task<Response> SendContactEmailAsync(string email, string subject, string message, string custName)
        {
            var nameTo = _configuration["Mail:NameFrom"];
            var to = _configuration["Mail:From"];
            var smtp = _configuration["Mail:Smtp"];
            var port = _configuration["Mail:Port"];
            var password = _configuration["Mail:Password"];

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(custName, email));
            mimeMessage.To.Add(new MailboxAddress(nameTo, to));
            mimeMessage.Subject = subject;

            var bodybuilder = new BodyBuilder
            {
                HtmlBody = message,
            };

            mimeMessage.Body = bodybuilder.ToMessageBody();

            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port), false);
                    client.Authenticate(to, password);
                    await client.SendAsync(mimeMessage);
                    client.Disconnect(true);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error sending contact email"); // Log the error
                return new Response { IsSuccess = false, Message = "An error occurred while sending the email." };
            }

            return new Response { IsSuccess = true };
        }

        public async Task<Response> SendEmail(string to, string subject, string body, string attachmentPath)
        {
            Response response = new Response();

            try
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.sapo.pt", 587);
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("garage88.support@sapo.pt", "&w_Rq25@!=A6000S");

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("garage88.support@sapo.pt");
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = body;

                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    Attachment attachment = new Attachment(attachmentPath);
                    mailMessage.Attachments.Add(attachment);
                }

                // Envia o e-mail
                await client.SendMailAsync(mailMessage); // Use SendMailAsync para melhor tratamento de exceções

                // Retorna sucesso
                response.Message = "E-mail sent successfully!";
            }
            catch (SmtpFailedRecipientException ex)
            {
                response.Message = $"There was an error: {ex.FailedRecipient}";
                _logger.LogError($"Failed to send email to {to}: {ex.Message}"); // Log detalhado
            }
            catch (Exception ex)
            {
                response.Message = "An unexpected error occurred while sending the email.";
                _logger.LogError($"Unexpected error: {ex.Message}"); // Log para outros erros
            }

            return response;
        }

    }
}
