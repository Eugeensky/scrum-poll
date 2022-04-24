using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ScrumPoll.Hubs;

namespace ScrumPoll.Controllers;

[Authorize]
public class PollController : Controller
{
    private readonly Database db;
    private readonly IHubContext<PollHub> hubContext;
    public PollController(Database _db, IHubContext<PollHub> _hubContext)
    {
        db = _db;
        hubContext = _hubContext;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(User.IsInRole("Master"));
    }

    [Authorize(Roles = "Master")]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Master")]
    [HttpPost]
    public async Task<IActionResult> Create(string description, int timeInMinutes, string[] options)
    {
        var newId = await db.Poll.Add(description, timeInMinutes, options);
        await hubContext.Clients.All.SendAsync("AddNew", new { id = newId, description });
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> ShowActive()
    {
        return View(await db.Poll.GetActivePollDescriptionList(User.Identity?.Name));
    }

    [HttpGet]
    public async Task<IActionResult> Open(Guid id)
    {
        var pollSession = await db.PollSession.GetOrCreatePollSession(User.Identity?.Name, id);
        if (pollSession is null) return RedirectToAction("ShowActive");
        await hubContext.Clients.All.SendAsync("PollNotification", $"{User.Identity?.Name} join to the poll");

        var finishSessionDelay = pollSession.TimeInMinutes * 60000 + 5000; // +5 sec delay
        Task.Delay(finishSessionDelay).ContinueWith(t => db.PollSession.FinishPollSession(pollSession.Id));

        return View(pollSession);
    }

    [HttpPost]
    public async Task<bool> SubmitAnswer(Guid sessionId, Guid optionId) 
    {
        if (sessionId != Guid.Empty && optionId != Guid.Empty)
        {
            var result = await db.PollSession.SubmitPollAnswer(sessionId, optionId);
            await hubContext.Clients.All.SendAsync("PollNotification", $"{User.Identity?.Name} just voted");
            return result;
        }
        return false;
    }

    [Authorize(Roles = "Master")]
    [HttpGet]
    public async Task<IActionResult> ShowUnpublished()
    {
        return View(await db.Poll.GetAllUnpublished());
    }

    [Authorize(Roles = "Master")]
    [HttpGet]
    public async Task<IActionResult> Publish(Guid id)
    {
        if(await db.Poll.Publish(id))
        {
            await hubContext.Clients.All.SendAsync("Publish", new { id, description = await db.Poll.GetPollDescription(id) });
            return RedirectToAction("Result", new { id });
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> ShowPublished()
    {
        return View(await db.Poll.GetAllPublished());
    }

    [HttpGet]
    public async Task<IActionResult> Result(Guid id)
    {
        return View(await db.Poll.GetResult(id));
    }
}
