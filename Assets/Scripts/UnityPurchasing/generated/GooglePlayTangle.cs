// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("qRoGfCs2q0WCt0I6Y/A8hblj3CYZdyZTTXz3NOhC4/eaDni7qPaBHQgL6iRGdFx0iuX9RgMd97GgYkf5mKn6tTpKLruGEWsuPZjVdUqMsDybEta0hdi5qC1Esizt5trSAxcUsNWoPAyas07gIUJ5Hdcung6LZwLPeeN/N1lHCKX6gAtzvFSdQuRcHSQWkiCvKkfK1By0lyZBEFPTqIp6cjEfNGd7tYWw7A3yLVu4rRGJqBm+ftOYysoKD/O4d11V+h3Kgof8Rdv64elhB010khMINImu3eO5QSgjEwGCjIOzAYKJgQGCgoNole0LLBl5swGCobOOhYqpBcsFdI6CgoKGg4CftdqN3PRmYb2bHUXWeDpPNkdRH/3FfNVxOd/4zoGAgoOC");
        private static int[] order = new int[] { 2,7,6,9,12,9,11,9,12,13,10,13,13,13,14 };
        private static int key = 131;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
