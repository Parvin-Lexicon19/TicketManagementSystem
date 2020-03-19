using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketManagementSystem.Core.Models;
using TicketManagementSystem.Data;

namespace TicketManagementSystem.Controllers
{
    /*COMMENTS CAN ONLY BE CREATED AND RELATED COMMENTS LIST CAN BE SEEN INSIDE THEIR RELATED TICKET. 
     NO OTHER ACTION CAN BE DONE ON THEM, IF YOU NEED OTHER ACTIONS, UNCOMMENT BELOW COMMENTED ACTIONS AND DO REQUIRED CHANGES*/
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comments.Include(c => c.ApplicationUser).Include(c => c.Ticket);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["CommentBy"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Problem");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CommentTime,CommentBy,CommentText,TicketId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CommentBy"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", comment.CommentBy);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Problem", comment.TicketId);
            return View(comment);
        }

        //// GET: Comments/Details/5
        //public async Task<IActionResult> Details(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var comment = await _context.Comments
        //        .Include(c => c.ApplicationUser)
        //        .Include(c => c.Ticket)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (comment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(comment);
        //}

        //// GET: Comments/Edit/5
        //public async Task<IActionResult> Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var comment = await _context.Comments.FindAsync(id);
        //    if (comment == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CommentBy"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", comment.CommentBy);
        //    ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Problem", comment.TicketId);
        //    return View(comment);
        //}

        //// POST: Comments/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, [Bind("Id,CommentTime,CommentBy,CommentText,TicketId")] Comment comment)
        //{
        //    if (id != comment.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(comment);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CommentExists(comment.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CommentBy"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", comment.CommentBy);
        //    ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Problem", comment.TicketId);
        //    return View(comment);
        //}

        //// GET: Comments/Delete/5
        //public async Task<IActionResult> Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var comment = await _context.Comments
        //        .Include(c => c.ApplicationUser)
        //        .Include(c => c.Ticket)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (comment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(comment);
        //}

        //// POST: Comments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id)
        //{
        //    var comment = await _context.Comments.FindAsync(id);
        //    _context.Comments.Remove(comment);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CommentExists(long id)
        //{
        //    return _context.Comments.Any(e => e.Id == id);
        //}
    }
}
