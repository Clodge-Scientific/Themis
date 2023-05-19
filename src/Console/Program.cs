using Themis.Console.Common;

//< Consider using the following:

//var builder = Host.CreateDefaultBuilder(args);
//// Add services to the container
//builder.ConfigureServices(services =>
//    services.AddSingleton<Example>());
//var registrar = new TypeRegistrar(builder);

var services = new ServiceCollection();



var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(cfg =>
{

});

return app.Run(args);