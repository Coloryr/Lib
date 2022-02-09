using Newtonsoft.Json;
using System.Text;

namespace ColorLib.ASP
{
    public class ServerUtils
    {
        public static byte[] BuildPack(object obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }
        public static ValueTask Send(Stream OutputStream, object obj)
        {
            return OutputStream.WriteAsync(BuildPack(obj));
        }
        public static async Task<string> ReadAsString(HttpRequest request) 
        {
            var data = new byte[2000000];
            long la = request.ContentLength ?? 0;
            StringBuilder builder = new();
            while (la > 0)
            {
                int a = await request.Body.ReadAsync(data);
                la -= a;
                builder.Append(Encoding.UTF8.GetString(data), 0, a);
            }
            string data1 = builder.ToString();
            if (!data1.EndsWith("}"))
            {
                int pos = data1.LastIndexOf('}') + 1;
                data1 = data1[..pos];
            }

            return data1;
        }
    }
}
