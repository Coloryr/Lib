using System.Text;

namespace ColorLib.Log
{
    public class Logs : ILog
    {
        private readonly string file;
        private readonly string local;
        private readonly object lockobject = new();

        public Logs(string local, string file = "logs.log")
        {
            this.local = local;
            this.file = local + file;

            if (!File.Exists(local + file))
                File.Create(local + file);
        }

        private void LogWrite(string a)
        {
            lock (lockobject)
            {
                try
                {
                    var date = DateTime.Now;
                    string year = date.ToShortDateString().ToString();
                    string time = date.ToLongTimeString().ToString();
                    string write = $"[{year}][{time}]{a}";
                    File.AppendAllText(local + file, write + Environment.NewLine, Encoding.UTF8);
                    Console.WriteLine(write);
                }
                catch
                { }
            }
        }

        public void LogError(Exception a)
        {
            Task.Run(() =>
            {
                LogWrite($"[Error]{a.Message}\n{a.StackTrace}");
            });
            Exception? temp = a.InnerException;
            while (temp != null)
            {
                Task.Run(() =>
                {
                    LogWrite($"{temp.Message}\n{temp.StackTrace}");
                });
                temp = temp.InnerException;
            }
        }

        public void LogError(string a)
        {
            Task.Run(() =>
            {
                LogWrite($"[Error]{a}");
            });
        }

        public void LogOut(string a)
        {
            Task.Run(() =>
            {
                LogWrite($"[Log]{a}");
            });
        }
    }
}