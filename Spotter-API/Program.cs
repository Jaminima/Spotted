using SpotifyAPI.Web;

var client = new Spotted_API.Services.Spotify.Client(Spotted_API.Auth.eg_key);

while (true)
{
    var state = client.CheckPlayState();
    state.Wait();

    if (state.Result.isDifferentToPreviousState)
    {
        Console.Write($"\r\n{state.Result.playingState.ToString()} - {state.Result.songChange.ToString()} - {client.currentTrack?.Name}");
    }
    else
    {
        Console.Write(".");
    }
    Thread.Sleep(5000);
}

Console.ReadLine();

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
