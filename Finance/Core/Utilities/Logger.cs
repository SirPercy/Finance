using System.IO;

namespace Finance.Core.Utilities
{
    public class Logger
    {
        public static void AddMessage(string message, bool appendFile = true) {
            using (var sw = new StreamWriter(System.Web.Hosting.HostingEnvironment.MapPath("~/Files/logger.txt"), true)) {
                sw.WriteLine(message);
                sw.Close();
            }
        }

    }
}