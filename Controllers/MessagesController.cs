using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPayments.Email;
using DigitalPayments.Helpers;
using DigitalPayments.Models;
using DigitalPayments.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DigitalPayments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {

        private readonly IDigitalPaymentsRepository<Message, int> messageRepository;

        public MessagesController(IDigitalPaymentsRepository<Message, int> messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        [HttpPost("[action]")]
        public IActionResult ContactUs([FromBody] ContactUsViewModel model)
        {

            List<string> errorList = new List<string>();

            if (!ModelState.IsValid)
            {
                errorList = Operations.ConvertModelStateToErrorsList(ModelState);

                return BadRequest(new JsonResult(errorList));
            }

            var message = new Message
            {
                Email = model.Email,
                Subject = model.Subject, 
                Name = model.Name, 
                MessageContent = model.Message
            };

            messageRepository.Add(message);


       
            return Ok(message);


        }

        [HttpGet("[action]")]
        [Authorize(Roles = "admin")]
        public IActionResult GetAllMessages()
        {
            var messages = messageRepository.List().ToList();
            return Ok(messages);

        }
    }
}