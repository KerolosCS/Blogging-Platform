﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testTask.Data;
using testTask.DTOs;
using testTask.Models;

namespace testTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext db)
        {
            _context = db;
        }

        [HttpGet("GetComments")]


        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        
        public ActionResult<IEnumerable<CommentDTO>> GetComments()
        {
            var comments = _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                ;
            List<CommentDTO> commentDTOs = comments.Select(c => new CommentDTO { 
            Id = c.Id,
            CommentContent = c.Text,
            PostId = c.PostId,
            UserId  = c.UserId,
            CommenterEmail = c.User.Username,
            Name = c.User.Username,
            Created = c.CreationDate,
            
            }).ToList();



            return Ok(commentDTOs);
        }

        [HttpGet("GetComment/{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CommentDTO> GetComment(int id)
        {
            var comment = _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                .SingleOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound();
            }


            CommentDTO commentDTO = new CommentDTO()
            {
                UserId = id,
                CommentContent = comment.Text,
                PostId = comment.PostId,
                Id = comment.Id,
                CommenterEmail = comment.User.Username,
                Name = comment.User.Username,
                Created = comment.CreationDate,
            };

            
            return Ok(commentDTO);
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task< ActionResult<Comment>> CreateComment(CommentCreateDTO commentDto)


        {



            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Comment comment =  new Comment()
            {
                Id = commentDto.Id,
                PostId= commentDto.PostId,
                UserId= commentDto.UserId,
                Text = commentDto.CommentContent,
                CreationDate = DateTime.Now,
               
            };
            _context.Comments.Add(comment);
           await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        [HttpPut("Update/{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateComment(int id, CommentCreateDTO commentDto  , int userID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != commentDto.Id)
            {
                return BadRequest();
            }
            if (userID != commentDto.UserId)
            {
                return BadRequest("You Can only edit your comment");
            }
            Comment comment = new Comment()
            {
                Id = commentDto.Id,
                PostId = commentDto.PostId,
                UserId = commentDto.UserId,
                Text = commentDto.CommentContent,
                CreationDate = DateTime.Now,

            };


            _context.Entry(comment).State = EntityState.Modified;
           await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = _context.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
         await   _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
