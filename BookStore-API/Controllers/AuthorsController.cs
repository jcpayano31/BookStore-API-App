﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Authors in the book store's database    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>
        /// Get all Authors
        /// <retuns> List of Authors</retuns>
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo($"Attempted Get all Authors");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo($"Successfully got all Authors");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }


        }


        /// <summary>
        /// Get author by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A author 's recorder</returns>

        [HttpGet("{id}")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {

            try
            {
                _logger.LogInfo($"Attempte  to get author with id:{id}");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"Author with id:{id} was not found ");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"Successfully got author with id:{id}");
                return Ok(response);

            }
            catch (Exception e)
            {

                return InternalError($"{e.Message} - {e.InnerException}");
            }


        }

        /// <summary>
        /// Create an Author 
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo($"Author Submission Attempted");
                if (authorDTO == null)
                {
                    _logger.LogWarn($"Empty Request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Author Data was Incompleted");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);
                if (!isSuccess)
                {

                    return InternalError($"Author creation failed");
                }

                _logger.LogInfo("Author Created");
                return Created("Create", new { author });

            }
            catch (Exception e)
            {

                return InternalError($"{e.Message} - {e.InnerException}");
            }

        }
        /// <summary>
        /// Update an Author 
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo($"Author with id:  {id} Updated Attempted  ");
                if(id < 1 || authorDTO == null || id != authorDTO.Id) 
                {
                    _logger.LogWarn($"Authoer Update failed with bad data");
                    return BadRequest();
                }
                var isExists = await _authorRepository.isExists(id);
                if(!isExists) 
                {
                    _logger.LogWarn($"Authoer with id: {id} not found");
                    return NotFound();
                }

                if (!ModelState.IsValid) 
                {
                    _logger.LogWarn($"Authoer Data is inpomplete");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess) 
                {

                    return InternalError($"Update Opereation Failed");

                }
                return NoContent();

            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
                                
            }
                  
        
        
        
        }


        /// <summary>
        /// Remove  an Author by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id) 
        
        {
            try
            {
                _logger.LogWarn($"Author with id: {id}  Delete Attempted");
                if (id < 1) 
                {
                    _logger.LogWarn($"AuthorDelete failed with bad data");
                    return BadRequest();
                }
                var author = await _authorRepository.FindById(id);
                if(author == null) 
                {
                    _logger.LogWarn($"Authoer with id: {id} was not found");
                    return NotFound();
                }
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess) 
                {
                    return InternalError($"Author delete Failed");
                }
                _logger.LogWarn($"Author with id: {id} was bit found");
                return NoContent();

            }
            catch (Exception e)
            {

                return InternalError($"{e.Message} - {e.InnerException}");
            }

        }








        /// <summary>
        /// Internal error
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
            private ObjectResult  InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong.   Please contact the Administrator");
        }

    }
}