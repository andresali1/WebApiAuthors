namespace WebApiAuthors.Services
{
    public class WriteToFile : IHostedService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string fileName = "ArchivoTest.txt";
        private Timer timer;

        public WriteToFile(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Write("Proceso Iniciado");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            Write("Proceso Finalizado");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Write("Proceso en ejecución: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        private void Write(string message)
        {
            var path = $@"{_env.ContentRootPath}\wwwroot\{fileName}";
            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                writer.WriteLine(message);
            }
        }
    }
}
