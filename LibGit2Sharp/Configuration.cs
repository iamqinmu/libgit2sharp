﻿using System;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    ///   Provides access to the '.git\config' configuration for a repository.
    /// </summary>
    public class Configuration : IDisposable
    {
        private readonly ConfigurationSafeHandle handle;

        internal Configuration(Repository repository)
        {
            Ensure.Success(NativeMethods.git_repository_config(out handle, repository.Handle, null, null));
        }

        internal ConfigurationSafeHandle Handle
        {
            get { return handle; }
        }

        #region IDisposable Members

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        ///   Delete a configuration variable (key and value).
        /// </summary>
        /// <param name = "key">The key to delete.</param>
        public void Delete(string key)
        {
            Ensure.Success(NativeMethods.git_config_delete(handle, key));
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            handle.SafeDispose();
        }

        /// <summary>
        ///   Get a configuration value for a key. Keys are in the form 'section.name'.
        ///   <para>
        ///     For example in  order to get the value for this in a .git\config file:
        /// 
        ///     [core]
        ///     bare = true
        /// 
        ///     You would call:
        /// 
        ///     bool isBare = repo.Config.Get&lt;bool&gt;("core.bare");
        ///   </para>
        /// </summary>
        /// <typeparam name = "T">The configuration value type</typeparam>
        /// <param name = "key">The key</param>
        /// <param name="defaultValue">The default value (optional)</param>
        /// <returns>The configuration value, or <c>defaultValue</c> if not set</returns>
        public T Get<T>(string key, T defaultValue = default(T))
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)GetString(key, (string)(object)defaultValue);
            }

            if (typeof(T) == typeof(bool))
            {
                return (T)(object)GetBool(key, (bool)(object)defaultValue);
            }

            if (typeof(T) == typeof(int))
            {
                return (T)(object)GetInt(key, (int)(object)defaultValue);
            }

            if (typeof(T) == typeof(long))
            {
                return (T)(object)GetLong(key, (long)(object)defaultValue);
            }

            throw new ArgumentException(string.Format("Generic Argument of type '{0}' is not supported.", typeof(T).FullName));
        }

        private bool GetBool(string key, bool defaultValue)
        {
            bool value;
            var res = NativeMethods.git_config_get_bool(handle, key, out value);
            if(res == (int)GitErrorCode.GIT_ENOTFOUND)
            {
                return defaultValue;
            }
            Ensure.Success(res);
            return value;
        }

        private int GetInt(string key, int defaultValue)
        {
            int value;
            var res = NativeMethods.git_config_get_int(handle, key, out value);
            if(res == (int)GitErrorCode.GIT_ENOTFOUND)
            {
                return defaultValue;
            }
            Ensure.Success(res);
            return value;
        }

        private long GetLong(string key, long defaultValue)
        {
            long value;
            var res = NativeMethods.git_config_get_long(handle, key, out value);
            if(res == (int)GitErrorCode.GIT_ENOTFOUND)
            {
                return defaultValue;
            }
            Ensure.Success(res);
            return value;
        }

        private string GetString(string key, string defaultValue)
        {
            IntPtr value;
            var res = NativeMethods.git_config_get_string(handle, key, out value);
            if(res == (int)GitErrorCode.GIT_ENOTFOUND)
            {
                return defaultValue;
            }
            Ensure.Success(res);
            return value.MarshallAsString();
        }

        /// <summary>
        ///   Set a configuration value for a key. Keys are in the form 'section.name'.
        ///   <para>
        ///     For example in order to set the value for this in a .git\config file:
        ///   
        ///     [test]
        ///     boolsetting = true
        ///   
        ///     You would call:
        ///   
        ///     repo.Config.Set("test.boolsetting", true);
        ///   </para>
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "key"></param>
        /// <param name = "value"></param>
        public void Set<T>(string key, T value)
        {
            if (typeof(T) == typeof(string))
            {
                Ensure.Success(NativeMethods.git_config_set_string(handle, key, (string)(object)value));
                return;
            }

            if (typeof(T) == typeof(bool))
            {
                Ensure.Success(NativeMethods.git_config_set_bool(handle, key, (bool)(object)value));
                return;
            }

            if (typeof(T) == typeof(int))
            {
                Ensure.Success(NativeMethods.git_config_set_int(handle, key, (int)(object)value));
                return;
            }

            if (typeof(T) == typeof(long))
            {
                Ensure.Success(NativeMethods.git_config_set_long(handle, key, (long)(object)value));
                return;
            }

            throw new ArgumentException(string.Format("Generic Argument of type {0} is not supported.", typeof(T)));
        }
    }
}