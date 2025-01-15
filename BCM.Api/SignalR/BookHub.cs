﻿using Microsoft.AspNetCore.SignalR;

namespace BCM.Api.SignalR;

public class BookHub : Hub
{
    public async Task SendBookUpdate() => await Clients.All.SendAsync("BooksUpdated");
}