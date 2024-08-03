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

            [HttpGet]
            public ActionResult<IEnumerable<PostDTO>> GetPosts()
            {

                
                var posts = _context.Posts.Include(p => p.Comments);

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

            [HttpGet("{id}")]
            public ActionResult<PostDTO> GetPost(int id)
            {
                var post = _context.Posts
                    .Include(p => p.Comments)
                    .SingleOrDefault(p => p.Id == id);

                if (post == null)
                {
                    return NotFound();
                }

            PostDTO postDTO = new PostDTO() {

                Id = post.Id,
                authorName = post.Author.Username,
                Comments = post.Comments.Select(c => new CommentDTO
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    CommentContent = c.Text,
                    UserId = c.UserId,
                }).ToList(),
                Content = post.Content,
                Title = post.Title,
                CreationDate = post.CreationDate,
            };


                return Ok(postDTO);
            }

            [HttpPost]
            public   ActionResult<PostCreateDTO> CreatePost(PostCreateDTO post)
            {
            Post p = new Post() {
            
             Id = post.Id,
             AuthorId = post.AuthorId,
             Title = post.Title,
             Content = post.Content,
             CreationDate= post.CreationDate,
            
            
            };
                 _context.Posts.Add(p);
                 _context.SaveChanges();

                return  CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
            }

            [HttpPut("{id}")]
            public IActionResult UpdatePost(int id, Post post)
            {
                if (id != post.Id)
                {
                    return BadRequest();
                }

                _context.Entry(post).State = EntityState.Modified;
                _context.SaveChanges();

                return NoContent();
            }

            [HttpDelete("{id}")]
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

