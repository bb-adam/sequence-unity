using System;
using System.Text;

namespace Sequence.Utils.SecureStorage
{
    public static class SecureStorageFactory
    {
        public static ISecureStorage CreateSecureStorage()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new iOSKeychainStorage();
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return new MacOSKeychainStorage();
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            return new WindowsProtectedDataStorage();
#else
            return new NotImplementedStorage();
#endif
        }

        public static bool IsSupportedPlatform()
        {
            ISecureStorage secureStorage = CreateSecureStorage();
            return secureStorage is not NotImplementedStorage;
        }
    }
}