﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using testTask.Data;
using testTask.Models;

namespace testTask.Controllers
{
   
        [ApiController]
        [Route("api/[controller]")]
        public class PostController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public PostController(ApplicationDbContext db)
            {
                _context = db;
            }

            [HttpGet("GetPosts")]
            public ActionResult<IEnumerable<PostDTO>> GetPosts()
            {

                
             var posts = _context.Posts.Include(p => p.Comments).Include(p => p.Author);

            List<PostDTO> postDTOs = posts.Select(p => new PostDTO
            {
                Id = p.Id,
                AuthorId = p.AuthorId,
                authorName = p.Author.Username,
                Comments = p.Comments.Select(c => new CommentDTO
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    CommentContent = c.Text,
                    UserId = c.UserId,
                }).ToList(),
                Content=p.Content,
                Title=p.Title,
                CreationDate = p.CreationDate,


            }).ToList();


            return Ok (postDTOs);
            }

            [HttpGet("GetPost/{id}")]
            public ActionResult<Post> GetPost(int id)
            {
                var post = _context.Posts
                    .Include(p => p.Comments).Include(P => P.Author)
                    .SingleOrDefault(p => p.Id == id);

                if (post == null)
                {
                    return NotFound();
                }

            List<CommentDTO> commentsDTO = null;
            if (post.Comments != null)
            {
                commentsDTO = post.Comments.Select(c => new CommentDTO
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    CommentContent = c.Text,
                    UserId = c.UserId,
                }).ToList();
            }

            PostDTO postDTO = new PostDTO()
            {

                Id = post.Id,
                authorName = post.Author.Username,
                AuthorId = post.Author.Id,
                Comments = commentsDTO,
                Content = post.Content,
                Title = post.Title,
                CreationDate = post.CreationDate,
            };


            return Ok(postDTO);
            }

            [HttpPost("Create")]
            public   async Task<ActionResult<PostCreateDTO>> CreatePost(PostCreateDTO post)
            {
            Post p = new Post() {
            
             Id = post.Id,
             AuthorId = post.AuthorId,
             Title = post.Title,
             Content = post.Content,
             CreationDate= post.CreationDate,
            
            
            };
                 _context.Posts.Add(p);
              await  _context.SaveChangesAsync();

                return  CreatedAtAction(nameof(GetPost), new { id = p.Id }, post);
            }

            [HttpPut("Update/{id}")]
            public async Task<IActionResult> UpdatePost(int id, PostCreateDTO postCreateDTO)
            {
            var postUpdate = _context.Posts.AsNoTracking()
                    .Include(p => p.Comments).Include(P => P.Author)
                    .SingleOrDefault(p => p.Id == id);
            if (id != postCreateDTO.Id)
                {
                    return BadRequest();
                }

            Post post = new Post() { 
            
                Id = postUpdate.Id,
                AuthorId = postUpdate.AuthorId,
                Title = postCreateDTO.Title,
                Author = postUpdate.Author,
                Content = postCreateDTO.Content,
                CreationDate = postUpdate.CreationDate,
                Comments = postUpdate.Comments,
                
            
            };
                _context.Entry(post).State = EntityState.Modified;
               await _context.SaveChangesAsync();

                return NoContent();
            }

            [HttpDelete("Delete/{id}")]
            public async Task<IActionResult> DeletePost(int id)
            {
                var post = _context.Posts.Find(id);
                if (post == null)
                {
                    return NotFound();
                }

                _context.Posts.Remove(post);
               await _context.SaveChangesAsync();

                return NoContent();
            }
        }

    }

