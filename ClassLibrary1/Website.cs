namespace ClassLibrary1
{
    public class Website
    {
        public Website()
        {
        }

        public Website(string name)
        {
            Name = name;
        }

        public Website(string name, string ipaddr, int port)
        {
            this.Name = name;
            this.Ipaddr = ipaddr;
            this.Port = port;
        }
        public string Name { get; set; }
        public int Port { get; set; }
        public string Ipaddr { get; set; } = "*";

    }    }