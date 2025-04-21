using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using System;
using broker.Data;
using broker.Models;
using AutoMapper;
using broker.Dto;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Cryptography.KeyDerivation; // For PBKDF2
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer; // For salt generation
// using BCrypt.Net; 
namespace Controllers
{   
    //   [Authorize]
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<User> _userRepository;

        private readonly IMapper _mapper;
         private static IWebHostEnvironment _environment;
        public CustomerController(IRepository<Customer> repo,  IRepository<User> userRepo, IMapper mapper, IWebHostEnvironment environment)
        {
            _customerRepository = repo;
            _mapper = mapper;
            _environment=environment;
            _userRepository = userRepo;
        }
        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles = "Broker, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var model = await _customerRepository.GetData();
            return Ok(_mapper.Map<IEnumerable<CustomerDto>>(model));
        }

        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetCustomerById(int id)
        // {
        //     Console.WriteLine("Returning technician of id" + id);
        //     var model = await _customerRepository.GetDataById(id);
        //     return Ok(_mapper.Map<CustomerDto>(model));
        // }
        // get customers by email
        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles = "Broker, Admin")]
        [HttpGet("{phone}")]
        public async Task<IActionResult> GetCustomerByPhone(string phone)
        {
            Console.WriteLine("Returning customer of email" + phone);
            var model = await _customerRepository.GetByEmail(phone);
            return Ok(_mapper.Map<CustomerDto>(model));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerDto  customerDto)
        {
            var existingUserByEmail = await _userRepository.GetByEmail(customerDto.User.Email);
            if (existingUserByEmail != null)
            {
                return BadRequest(new { message = "Email is already registered" });
            }

            // Check if the phone is already registered
            var existingUserByPhone = await _userRepository.GetByPhone(customerDto.User.Phone);
            if (existingUserByPhone != null)
            {
                return BadRequest(new { message = "Phone number is already registered" });
            }
            Console.WriteLine("Creating customers");
            var customer = _mapper.Map<Customer>(customerDto);
            customer.User.Password = BCrypt.Net.BCrypt.HashPassword(customerDto.User.Password);
            Console.WriteLine("Creating Users");
            // var user = _mapper.Map<User>(userDto);
            //  Console.WriteLine("Entered tot he image upload");

            // string fName = customerDto.User.Picture.FileName;
            // Console.WriteLine(fName);
            // string path = Path.Combine(_environment.ContentRootPath, "Images/" + fName);
            // using (var stream = new FileStream(path, FileMode.Create))
            // { 
            //     await customerDto.User.Picture.CopyToAsync(stream);
            // }
            // // return file.FileName;
            // customer.User.Picture=customerDto.User.Picture.FileName;
            await _customerRepository.InsertData(customer);
            return Ok(customerDto);
        }
        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles = "Admin")]
        // [Authorize(Roles = RoleEntity.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var model = await _customerRepository.GetDataById(id);
            var customer = _mapper.Map<Customer>(model);
            await _customerRepository.DeleteData(customer);
            return Ok(model);
        }
        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles = "Admin, Customer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerDto  customerDto)
        {
            // Console.WriteLine(technician.AccepteStatus);
            var customer = _mapper.Map<Customer>(customerDto);
            await _customerRepository.UpdateData(customer);
            return Ok(customer);
        }
        //   public async Task<IActionResult> GetPaginatedCustomers(int pageNumber,int pageSize, string orderBy,string search)
        // {   
        //     // ServiceRepository _service= new ServiceRepository();
        //      Console.WriteLine("These are the comming constriant");
        //      Console.WriteLine(pageNumber);
        //      Console.WriteLine(orderBy);
        //      Console.WriteLine(search);
       
        //    var model = await _brokerRepository.GetPaginatedData(pageNumber,pageSize,orderBy,search);

        // var totalPage= await _brokerRepository.GetTotalPage(pageSize,search);
        // BrokerData broker= new BrokerData(totalPage,model);
             
        //      return Ok(broker);
        // }
        
    }

}