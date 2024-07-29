// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("uzL2lKX4mYgNZJIMzcb68iM3NJA2sgCPCmfq9DyUtwZhMHPziKpaUok6JlwLFotlopdiGkPQHKWZQ/wG2sHJQSdtVLIzKBSpjv3DmWEIAzOTIaKBk66lqokl6yVUrqKioqajoPWIHCy6k27AAWJZPfcOvi6rRyLvv5X6rfzURkGduz1l9lgabxZncT85VwZzbVzXFMhiw9e6LlibiNahPbiJ2pUaag6bpjFLDh249VVqrJAcET8UR1uVpZDMLdINe5iNMamIOZ5e87jq6iov05hXfXXaPeqip9xl+yGirKOTIaKpoSGioqNItc0rDDlZWcNfF3lnKIXaoCtTnHS9YsR8PQQoK8oEZlR8VKrF3WYjPdeRgEJn2d3lXPVRGf/Y7qGgoqOi");
        private static int[] order = new int[] { 12,5,2,10,12,10,8,7,9,12,13,13,13,13,14 };
        private static int key = 163;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
