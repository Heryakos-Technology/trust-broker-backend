using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using System;
using broker.Data;
using broker.Models;
using AutoMapper;
using broker.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Controllers
{   
    // [Authorize]
    [Route("api/city")]
    [ApiController]
    public class CityController : ControllerBase
    { 
        private readonly IRepository<City> _cityRepository;
        private readonly IMapper _mapper;
        public CityController(IRepository<City> repo, IMapper mapper)
        {
            _cityRepository = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCit()
        {
            var model = await _cityRepository.GetData();
            return Ok(_mapper.Map<IEnumerable<CityDto>>(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityById(int id)
        {
            Console.WriteLine("Returning technician of id" + id);
            var model = await _cityRepository.GetDataById(id);
            return Ok(_mapper.Map<CityDto>(model));
        }
        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCities(CityDto cityDto)
        {   
            Console.WriteLine("Creating new cities");
            var city = _mapper.Map<City>(cityDto);
            await _cityRepository.UpdateData(city);
            return Ok(cityDto);
        }
        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCities(int id)
        {
            var model = await _cityRepository.GetDataById(id);
            var cities = _mapper.Map<City>(model);

            await _cityRepository.DeleteData(cities);

            return Ok(model);
        }
        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCities(int id, CityDto cityDto)
        {
            // Console.WriteLine(technician.AccepteStatus);
            var city = _mapper.Map<City>(cityDto);
            await _cityRepository.UpdateData(city);
            return Ok(city);
        }
        
        
    }

}