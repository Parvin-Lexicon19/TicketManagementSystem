using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketManagementSystem.Data;

namespace TicketManagementSystem.Controllers
{
    public class DocumentsController : Controller
    {

        private readonly ApplicationDbContext _context;


        public DocumentsController(ApplicationDbContext context)
        {
            _context = context;
        }
   

        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filepath = _context.Documents.Where(d => d.Id == id).Select(d => d.Path).FirstOrDefault();

            var memory = new MemoryStream();
            using (var stream = new FileStream(filepath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(filepath), Path.GetFileName(filepath));
        }

        public async Task<IActionResult> RemoveFile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = _context.Documents.Where(d => d.Id == id).FirstOrDefault();
            var ticketid = document.TicketId;
            if (!System.IO.File.Exists(document.Path))
            {
                return NotFound();
            }
            else
            {
                try
                {
                    System.IO.File.Delete(document.Path);
                }
                catch (Exception)
                {

                    throw;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Remove(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.Id))
                    {
                        return NotFound();
                    }

                }
            }


            return RedirectToAction("edit", "Tickets", new { id = ticketid, });
        }

        

                private bool DocumentExists(long id)
                {
                    return _context.Documents.Any(e => e.Id == id);
                }


                private string GetContentType(string path)
                {
                    var types = GetMimeTypes();
                    var ext = Path.GetExtension(path).ToLowerInvariant();
                    return types[ext];
            }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats,officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".jfif","image/jfif" },
                {".csv", "text/csv"}
            };
        }

    }
}